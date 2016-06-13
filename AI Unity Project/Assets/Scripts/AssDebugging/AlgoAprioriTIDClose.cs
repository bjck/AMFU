using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using patterns.itemset;
using patterns.itemsets;
using AssRules;
using System.IO;
namespace frequent.patterns
{
    public class AlgoAprioriTIDClose {

	// object for writing to file if the user choose to write to a file
	StreamWriter writer = null;
	
	// variable to store the result if the user choose to save to memory instead of a file
	protected Itemsets patterns = null;

	// the number of transactions
	private int databaseSize = 0;
	
	// the current level
	protected int k; 

	// variables for counting support of items
	Dictionary<int, HashSet<int>> mapItemTIDS = new Dictionary<int, HashSet<int>>();

	// the minimum support threshold
	int minSuppRelative;

	// Special parameter to set the maximum size of itemsets to be discovered
	int maxItemsetSize = Int32.MaxValue;

	public long startTimestamp = 0; // start time of latest execution
    public long startTimeStamp { get{return startTimestamp;} set{startTimestamp = value;} }
	long endTimestamp = 0; // end time of latest execution
	
	int itemsetCount = 0; // number of closed itemset found

	/**
	 * Default constructor
	 */
	public AlgoAprioriTIDClose() {
		
	}

	/**
	 * Run the algorithm
	 * @param minsupp the minsup threshold
	 * @param outputFile an output file path, if the result should be saved otherwise
	 *    leave it null and this method will keep the result into memory and return it.
	 * @return the set of itemsets found if the user chose to save the result to memory
	 * @throws IOException  exception if error writing the output file
	 */
	public Itemsets runAlgorithm(TransactionDatabase database, double minsupp, String outputFile) {
		// record start time
		//startTimestamp = AlgoClosedRules.CurrentMillis();
		
		// reset number of itemsets found
		itemsetCount = 0;
		
		// if the user want to keep the result into memory
		if(outputFile == null){
			writer = null;
			patterns =  new Itemsets("FREQUENT CLOSED ITEMSETS");
	    }else{ // if the user want to save the result to a file
			patterns = null;
			writer = new StreamWriter(outputFile); 
		}

		this.minSuppRelative = (int) Math.Ceiling(minsupp * database.size());
		if (this.minSuppRelative == 0) { // protection
			this.minSuppRelative = 1;
		}

		// (1) count the tid set of each item in the database in one database
		// pass
		mapItemTIDS = new Dictionary<int, HashSet<int>>(); 
		// key : item   value: tidset of the item 

		// for each transaction
		for (int j = 0; j < database.getTransactions().Count(); j++) {
			List<int> transaction = database.getTransactions().ElementAt(j);
			// for each item in the transaction
			for (int i = 0; i < transaction.Count(); i++) {
				// update the tidset of the item
                HashSet<int> ids;
                if (!mapItemTIDS.TryGetValue(transaction.ElementAt(i), out ids))
                {
                    ids = new HashSet<int>();
					mapItemTIDS.Add(transaction.ElementAt(i), ids);
				}
				ids.Add(j);
			}
		}
		
		// save the database size
		databaseSize = database.getTransactions().Count();

		// To build level 1, we keep only the frequent items.
		// We scan the database one time to calculate the support of each
		// candidate.
		k = 1;
		List<Itemset> level = new List<Itemset>();
		// For each item
        
		
        //Udkommenteret indtil videre
        /*Iterator<Entry<Integer, Set<Integer>>> iterator = mapItemTIDS.entrySet().iterator();
		while (iterator.hasNext()) {
			Map.Entry<Integer, Set<Integer>> entry = (Map.Entry<Integer, Set<Integer>>) iterator.next();

			if (entry.getValue().size() >= minSuppRelative) { // if the item is
																// frequent
				Integer item = entry.getKey();
				Itemset itemset = new Itemset(item);
				itemset.setTIDs(mapItemTIDS.get(item));
				level.add(itemset);
			} else {
				iterator.remove(); // if the item is not frequent we don't
				// need to keep it into memory.
			}
		}

		// sort itemsets of size 1 according to lexicographical order.
		Collections.sort(level, new IComparer<Itemset>() {
			public int compare(Itemset o1, Itemset o2) {
				return o1.get(0) - o2.get(0);
			}
		});*/


        IEnumerator<KeyValuePair<int, HashSet<int>>> iterator = mapItemTIDS.GetEnumerator();
        while (iterator.MoveNext())
        {
            KeyValuePair<int, HashSet<int>> entry = (KeyValuePair<int, HashSet<int>>)iterator.Current;

            if (entry.Value.Count() >= minSuppRelative) // if the item is
            {
                // frequent
                int item = entry.Key;
                Itemset itemset = new Itemset(item);
                itemset.setTIDs(mapItemTIDS[item]);
                level.Add(itemset);
            }
            else
            {
                //iterator.Dispose(); // if the item is not frequent we don't
                // need to keep it into memory.
            }
        }

        // sort itemsets of size 1 according to lexicographical order.
        //Collections.sort(level, new IComparerAnonymousInnerClassHelper(this));
        level.Sort(
            delegate(Itemset o1, Itemset o2)
            {
                return o1.get(0) - o2.get(0);
            });

		
		// Generate candidates with size k = 1 (all itemsets of size 1)
		k = 2;
		// While the level is not empty
        //TODO: Tjek om 10 ikke skal ændres
        //setMaxItemsetSize(Int32.MaxValue);
		while (level.Any() && k <= maxItemsetSize) {

			// We build the level k+1 with all the candidates that have
			// a support higher than the minsup threshold.
			List<Itemset> levelK = generateCandidateSizeK(level);

			// We check all sets of level k-1 for closure
			checkIfItemsetsK_1AreClosed(level, levelK);

			level = levelK; // We keep only the last level...
			k++;
		}

		// save end time
		endTimestamp = AlgoClosedRules.CurrentTimeMillis();
		
		// close the output file if the result was saved to a file
		if(writer != null){
			writer.Close();
		}
		return patterns; // Return all frequent itemsets found!
	}

	/**
	 * Remove items that at not frequent from the transaction database
	 * @param database
	 * @return a map indicating the tidset of each item (key: item  value: tidset)
	 */
	private Dictionary<int, HashSet<int>> removeItemsThatAreNotFrequent(
			TransactionDatabase database) {
		// (1) count the support of each item in the database in one database
		// pass
		// Map with (key: item  value: tidset)
		mapItemTIDS = new Dictionary<int, HashSet<int>>(); 

		// for each transaction
		for (int j = 0; j < database.getTransactions().Count(); j++) {
			List<int> transaction = database.getTransactions().ElementAt(j);
			// for each item
			for (int i = 0; i < transaction.Count(); i++) {
				// update the support count of the item
				HashSet<int> ids = mapItemTIDS[transaction.ElementAt(i)];
				if (ids == null) {
					ids = new HashSet<int>();
					mapItemTIDS.Add(transaction.ElementAt(i), ids);
				}
				ids.Add(j);
			}
		}
		Console.Out.WriteLine("NUMBER OF DIFFERENT ITEMS : " + mapItemTIDS.Count());
		// (2) remove all items that are not frequent from the database

		// for each transaction
		for (int j = 0; j < database.getTransactions().Count(); j++) {
			List<int> transaction = database.getTransactions().ElementAt(j);

			// for each item in the transaction
            //Udkommenteret da det kan skrives smartere
			IEnumerator<int> iter = transaction.GetEnumerator();
            
			while (iter.MoveNext()) {
                int nextItem = iter.Current;
				// if the item is not frequent
				HashSet<int> ids = mapItemTIDS[nextItem];
				if (ids.Count() < minSuppRelative) {
					// remove it!
					iter.Dispose();
				}
			}




		}
		return mapItemTIDS;
	}

	/**
	 * Checks if all the itemsets of size K-1 are closed by comparing
	 * them with itemsets of size K.
	 * @param levelKm1 itemsets of size k-1
	 * @param levelK itemsets of size k
	 * @throws IOException exception if error writing output file
	 */
	private void checkIfItemsetsK_1AreClosed(List<Itemset> levelKm1,
			List<Itemset> levelK) {
		// for each itemset of size k-1
		//for (Itemset itemset : levelKm1) {
            foreach (Itemset itemset in levelKm1)
	        {
		 
			// consider it is closed
			bool isClosed = true;
			// compare this itemset with all itemsets of size k
			//for (Itemset itemsetK : levelK) {
                foreach (Itemset itemsetK in levelK)
	            {
		 
				// if an itemset has the same support and contain the itemset of size k-1 ,
				// then the itemset of size k-1 is not closed
				if (itemsetK.getAbsoluteSupport() == itemset.getAbsoluteSupport() && itemsetK.containsAll(itemset)) {
					isClosed = false;
					break;
				}
			}
			// if itemset of size k-1 is closed
			if (isClosed) {
				// save the itemset of of size k-1  to file
				saveItemset(itemset);
			}
		}
	}
	
	/**
	 * Save a frequent itemset to the output file or memory,
	 * depending on what the user chose.
	 * @param itemset the itemset
	 * @throws IOException exception if error writing the output file.
	 */
	void saveItemset(Itemset itemset) {
		itemsetCount++;
		
		// if the result should be saved to a file
		if(writer != null){
			writer.Write(itemset.toString() + " #SUP: "
					+ itemset.getTransactionsIds().Count() );
			writer.WriteLine();
		}// otherwise the result is kept into memory
		else{
			patterns.addItemset(itemset, itemset.size());
		}
	}
	
	/**
	 * Method to generate itemsets of size k from frequent itemsets of size K-1.
	 * @param levelK_1  frequent itemsets of size k-1
	 * @return itemsets of size k
	 */
	protected List<Itemset> generateCandidateSizeK(List<Itemset> levelK_1) {
		// create a variable to store candidates
		List<Itemset> candidates = new List<Itemset>();

		// For each itemset I1 and I2 of level k-1
		//LOOP 1
        
        for (int i = 0; i < levelK_1.Count(); i++) 
        {
            bool loop1 = false;

			Itemset itemset1 = levelK_1.ElementAt(i);
            //LOOP 2
			for (int j = i + 1; j < levelK_1.Count(); j++) 
            {
				Itemset itemset2 = levelK_1.ElementAt(j);

				// we compare items of itemset1 and itemset2.
				// If they have all the same k-1 items and the last item of
				// itemset1 is smaller than
				// the last item of itemset2, we will combine them to generate a
				// candidate
				for (int k = 0; k < itemset1.size(); k++) 
                {
					// if they are the last items
					if (k == itemset1.size() - 1) 
                    {
						// the one from itemset1 should be smaller (lexical
						// order)
						// and different from the one of itemset2
						if (itemset1.getItems()[k] >= itemset2.get(k)) 
                        {

                            //goto loop1;
                            loop1 = true;
                            break;
						}
					}
					// if they are not the last items, and
                    else if (itemset1.getItems()[k] < itemset2.get(k))
                    {
                        //goto loop2; // we continue searching
                        break;
					} else if (itemset1.getItems()[k] > itemset2.get(k)) {

                        loop1 = true;
                        break;
						//goto loop1; // we stop searching: because of lexical
                        //continue;
										// order
                        //break;
					}
				}

                //TODO: Nyt
                if (loop1) {
                    break;
                }


				// NOW COMBINE ITEMSET 1 AND ITEMSET 2
				// create list of common tids
				HashSet<int> list = new HashSet<int>();
				//for (Integer val1 : itemset1.getTransactionsIds()) {
                    foreach (int val1 in itemset1.getTransactionsIds())
	                {
					if (itemset2.getTransactionsIds().Contains(val1)) {
						list.Add(val1);
					}
				}
				
				// if the combination of itemset1 and itemset2 is frequent
				if (list.Count() >= minSuppRelative) {
					// Create a new candidate by combining itemset1 and itemset2
					int[] newItemset = new int[itemset1.size()+1];
                    Array.Copy(itemset1.itemset, 0, newItemset, 0, itemset1.size());
					newItemset[itemset1.size()] = itemset2.getItems()[itemset2.size() -1];
					Itemset candidate = new Itemset(newItemset);
					candidate.setTIDs(list);
					
					candidates.Add(candidate);
//					frequentItemsets.addItemset(candidate, k);
				}

			}
		}
		return candidates;
	}


	/**
	 * Get the frequent closed itemsets found by the latest execution.
	 * @return Itemsets
	 */
	public Itemsets getFrequentClosed() {
		return patterns;
	}

	/**
	 * Set the maximum itemset size of itemsets to be found
	 * @param maxItemsetSize maximum itemset size.
	 */
	public void setMaxItemsetSize(int maxItemsetSize) {
		this.maxItemsetSize = maxItemsetSize;
	}
	
	/**
	 * Print statistics about the algorithm execution to System.out.
	 */
	public void printStats() {
		Console.Out.WriteLine("=============  APRIORI-CLOSE - STATS =============");
		long temps = endTimestamp - startTimestamp;
		// System.out.println(" Total time ~ " + temps + " ms");
		Console.Out.WriteLine(" Transactions count from database : "+ databaseSize);
		Console.Out.WriteLine(" The algorithm stopped at size " + (k - 1)+ ", because there is no candidate");
		Console.Out.WriteLine(" Frequent closed itemsets count : "+ itemsetCount);
		Console.Out.WriteLine(" Total time ~ " + temps + " ms");
		Console.Out.WriteLine("===================================================");
	}
}

}
