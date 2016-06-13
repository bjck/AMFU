using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using patterns.itemset;
using patterns.itemsets;
using patterns;
using System.Globalization;
namespace AssRules
{
    public class AlgoClosedRules
    {
        // parameters
        private Itemsets closedItemsets;   // closed itemsets
        private double minconf;    // minimum confidence threshold

        // closed association rules generated
        private ClosedRules rules;

        // for statistics
        long startTimestamp = 0; // last execution start time
        long endTimeStamp = 0;   // last execution end time
        private int ruleCount; // the number of rules found

        // object to write the output file if the user wish to write to a file
        System.IO.StreamWriter writer = null;


        public AlgoClosedRules(){
		
        }
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis() {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
        public ClosedRules runAlgorithm(Itemsets closedItemsets, double minconf, string outputFile)
        {
            this.closedItemsets = closedItemsets;

            // if the user want to keep the result into memory
            if (outputFile == null)
            {
                writer = null;
                rules = new ClosedRules("Closed association rules");
            }
            else
            {
                // if the user want to save the result to a file
                //rules = null;
                writer = new System.IO.StreamWriter(outputFile);
            }

            startTimestamp = CurrentTimeMillis();

            this.minconf = minconf;

            //For each frequent itemset of size >=2
            for (int k = 2; k < closedItemsets.getLevels().Count; k++)
            {
                foreach (Itemset lk in closedItemsets.getLevels().ElementAt(k))
                {

                    //}
                    //for(Itemset lk : closedItemsets.getLevels().get(k)){
                    // create H1
                    HashSet<Itemset> H1 = new HashSet<Itemset>();

                    foreach (int item in lk.getItems())
                    {
                        //for(Integer item : lk.getItems()){  // THIS PART WAS CHANGED
                        Itemset itemset = new Itemset(item);
                        H1.Add(itemset);

                    }

                    HashSet<Itemset> H1_for_recursion = new HashSet<Itemset>();
                    foreach (Itemset hm_P_1 in H1)
                    {
                        //for(Itemset hm_P_1 : H1){
                        Itemset itemset_Lk_minus_hm_P_1 = lk.cloneItemSetMinusAnItemset(hm_P_1);
                        //WRONG TODO
                        int supLkMinus_hm_P_1 = calculateSupport(itemset_Lk_minus_hm_P_1);   // THIS COULD BE DONE ANOTHER WAY ?
                        int supLk = calculateSupport(lk);                                            // IT COULD PERHAPS BE IMPROVED....
                        double conf = ((double)supLk) / ((double)supLkMinus_hm_P_1);

                        if (conf >= minconf)
                        {
                            ClosedRule rule = new ClosedRule(itemset_Lk_minus_hm_P_1, hm_P_1, lk.getAbsoluteSupport(), conf);
                            save(rule);
                            H1_for_recursion.Add(hm_P_1);// for recursion
                        }
                    }

                    // call apGenRules
                    apGenrules(k, 1, lk, H1_for_recursion);
                }
            }

            endTimeStamp = CurrentTimeMillis();

            // if the user chose to save to a file, we close the file.
            if (writer != null)
            {
                writer.Close();
            }


            return rules;
        }

        private void apGenrules(int k, int m, Itemset lk, HashSet<Itemset> Hm)
        {
            //		System.out.println(" " + lk.toString() + "  " + Hm.toString());
            if (k > m + 1)
            {
                HashSet<Itemset> Hm_plus_1 = generateCandidateSizeK(Hm);
                HashSet<Itemset> Hm_plus_1_for_recursion = new HashSet<Itemset>();
                foreach (Itemset hm_P_1 in Hm_plus_1)
                {
                    //for(Itemset hm_P_1 : Hm_plus_1){
                    Itemset itemset_Lk_minus_hm_P_1 = lk.cloneItemSetMinusAnItemset(hm_P_1);

                    //				calculateSupport(hm_P_1);   
                    int supLkMinus_hm_P_1 = calculateSupport(itemset_Lk_minus_hm_P_1);   // THIS COULD BE DONE ANOTHER WAY ?
                    int supLk = calculateSupport(lk);                                            // IT COULD PERHAPS BE IMPROVED....
                    double conf = ((double)supLk) / ((double)supLkMinus_hm_P_1);

                    if (conf >= minconf)
                    {
                        ClosedRule rule = new ClosedRule(itemset_Lk_minus_hm_P_1, hm_P_1, lk.getAbsoluteSupport(), conf);
                        save(rule);
                        Hm_plus_1_for_recursion.Add(hm_P_1);
                    }
                }
                apGenrules(k, m + 1, lk, Hm_plus_1_for_recursion);
            }
        }

        private int calculateSupport(Itemset itemsetToTest)
        {  // THIS WAS CHANGED
            foreach (List<Itemset> list in closedItemsets.getLevels())
            {
                //for(List<Itemset> list : closedItemsets.getLevels()){
                if (list.Count() == 0 || list.ElementAt(0).size() < itemsetToTest.size())
                {
                    continue; // it is not useful to consider itemsets that are smaller  
                    // than itemsetToTest.size
                }
                foreach (Itemset itemset in list)
                {
                    //			for(Itemset itemset : list){
                    if (itemset.containsAll(itemsetToTest))
                    {
                        return itemset.getAbsoluteSupport();
                    }
                }
            }
            return 0;
        }

        protected HashSet<Itemset> generateCandidateSizeK(HashSet<Itemset> levelK_1)
        {
            //Set<Itemset> candidates = new HashSet<Itemset>();
            HashSet<Itemset> candidates = new HashSet<Itemset>();

            // For each itemset I1 and I2 of level k-1
            foreach (Itemset itemset1 in levelK_1)
            {
                //for(Itemset itemset1 : levelK_1){
                foreach (Itemset itemset2 in levelK_1)
                {
                    //for(Itemset itemset2 : levelK_1){
                    // If I1 is smaller than I2 according to lexical order and
                    // they share all the same items except the last one.
                    int missing = itemset1.allTheSameExceptLastItem(itemset2);
                    if (missing != 0)
                    {
                        // Create a new candidate by combining itemset1 and itemset2
                        int[] newItemset = new int[itemset1.size() + 1];
                        Array.Copy(itemset1.itemset, 0, newItemset, 0, itemset1.size());
                        newItemset[itemset1.size()] = itemset2.getItems()[itemset2.size() - 1];
                        Itemset candidate = new Itemset(newItemset);

                        //					System.out.println(" " + itemset1.toString() + " + " + itemset2.toString() + " = " + candidate.toString());

                        // The candidate is tested to see if its subsets of size k-1 are included in
                        // level k-1 (they are frequent).
                        if (allSubsetsOfSizeK_1AreFrequent(candidate, levelK_1))
                        {
                            candidates.Add(candidate);
                        }
                    }
                }
            }
            return candidates;
        }

        protected bool allSubsetsOfSizeK_1AreFrequent(Itemset candidate, HashSet<Itemset> levelK_1)
        {
            // To generate all the set of size K-1, we will proceed
            // by removing each item, one by one.
            if (candidate.size() == 1)
            {
                return true;
            }
            foreach (int item in candidate.getItems())
            {
                //		for(Integer item : candidate.getItems()){
                Itemset subset = candidate.cloneItemSetMinusOneItem(item);
                bool found = false;
                foreach (var itemset in levelK_1)
                {
                    //			for(Itemset itemset : levelK_1){
                    if (itemset.isEqualTo(subset))
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    return false;
                }
            }
            return true;
        }


        private void save(ClosedRule rule)
        {
            // increase the number of rule found
            ruleCount++;

            // if the result should be saved to a file
            if (rule.getConfidence() <= 1)
            {
                if (writer != null)
                {
					AssRules.AssRulesResult result = new AssRules.AssRulesResult();
                    //StringBuffer buffer = new StringBuffer();
                    System.Text.StringBuilder buffer = new StringBuilder();
                    // write itemset 1
                    if (rule.getItemset1().size() == 0)
                    {
                        buffer.Append("__");
                    }
                    else
                    {
                        for (int i = 0; i < rule.getItemset1().size(); i++)
                        {
                            buffer.Append(rule.getItemset1().get(i));
							//adding to rules
							result.input.Add (rule.getItemset1().get(i));
                            if (i != rule.getItemset1().size() - 1)
                            {
                                buffer.Append(" ");
                            }
                        }
                    }
                    // write separator
                    buffer.Append(" ==> ");
                    // write itemset 2
                    for (int i = 0; i < rule.getItemset2().size(); i++)
                    {
                        buffer.Append(rule.getItemset2().get(i));
						result.output.Add(rule.getItemset2 ().get (i));
                        if (i != rule.getItemset2().size() - 1)
                        {
                            buffer.Append(" ");
                        }
                    }
                    // write separator
                    buffer.Append(" #SUP: ");
                    // write support
                    buffer.Append(rule.getAbsoluteSupport());
					result.support = rule.getAbsoluteSupport();
                    // write separator
                    buffer.Append(" #CONF: ");
                    // write confidence
                    buffer.Append(rule.getConfidence());
					result.confidence = (rule.getConfidence());

                    writer.Write(buffer.ToString());
                    writer.WriteLine("");
                    writer.Flush();
					List<AssRulesResult> results = new List<AssRulesResult>();
					if(File.Exists("AIarules.bin"))
					{
						using(Stream stream = File.Open ("AIarules.bin", FileMode.Open))
						{
							BinaryFormatter bin = new BinaryFormatter();
							
							results = (List<AssRulesResult>)bin.Deserialize(stream);

						}
					}
					if(results.Count == 0)
					{
						results = new List<AssRulesResult>();
					}
					results.Add(result);
					using (Stream stream = File.Open("AIarules.bin", FileMode.OpenOrCreate))
					{
						BinaryFormatter bin = new BinaryFormatter();
						bin.Serialize(stream, results);
					}

                }// otherwise the result is kept into memory
                else
                {
                    rules.addRule(rule);
                }
            }
        }

        /**
         * Convert a double value to a string with only five decimal
         * @param value  a double value
         * @return a string
         */
        string doubleToString(double value) {
            // convert it to a string with two decimals
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            
            /*DecimalFormat format = new DecimalFormat();
            format.setMinimumFractionDigits(0); 
            format.setMaximumFractionDigits(5); 
            return format.format(value);*/
            return value.ToString(format);
        }


        /**
         * Print statistics about the algorithm execution to System.out.
         */
        public void printStatistics() {
            Console.WriteLine("============= CLOSED ASSOCIATION RULE GENERATION - STATS =============");
            Console.WriteLine(" Number of association rules generated : "
                               + ruleCount);
            Console.WriteLine(" Total time ~ " + (endTimeStamp - startTimestamp)
                               + " ms");
            Console.WriteLine("===================================================");
        }
    }
}
