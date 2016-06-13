using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssRules
{
    public class TransactionDatabase
    {
        private HashSet<int> items = new HashSet<int>();


        // the list of transactions
	private List<List<int>> transactions = new List<List<int>>();

	/**
	 * Method to add a new transaction to this database.
	 * @param transaction  the transaction to be added
	 */
	public void addTransaction(List<int> transaction) {
		transactions.Add(transaction);
		items.UnionWith(transaction);
	}

	/**
	 * Method to load a file containing a transaction database into memory
	 * @param path the path of the file
	 * @throws IOException exception if error reading the file
	 */
	/*public void loadFile(String path) {
		String thisLine; // variable to read each line
        StreamReader myInput = null;
		//BufferedReader myInput = null; // object to read the file
		try {
			StreamReader fin = new StreamReader(path);
			myInput = new StreamReader(new StreamReader(fin));
			// for each line
			while ((thisLine = myInput.ReadLine()) != null) {
				// if the line is not a comment, is not empty or is not other
				// kind of metadata
				if (thisLine.isEmpty() == false &&
						thisLine.charAt(0) != '#' && thisLine.charAt(0) != '%'
						&& thisLine.charAt(0) != '@') {
					// split the line according to spaces and then
					// call "addTransaction" to process this line.
					addTransaction(thisLine.split(" "));
				}
			}
		} catch (Exception e) {
			Console.WriteLine(e.StackTrace.ToString());
		} finally {
			if (myInput != null) {
				myInput.Close();
			}
		}
	}*/
    public void loadFile(String path)
    {
        try
        {

            string[] lines = File.ReadAllLines(path);


            foreach (string line in lines)
            {

                if (line[0] != '#' && line[0] != '%' && line[0] != '@')
                {
                    // split the line according to spaces and then
                    // call "addTransaction" to process this line.
                    addTransaction(line.Split(' '));
                }


            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace.ToString());
        }

    }

	/**
	 * This method process a line from a file that is read.
	 * @param tokens the items contained in this line
	 */
	private void addTransaction(String[] itemsString) {
		// create an empty transaction
		List<int> itemset = new List<int>();
		// for each item in this line
		foreach(string attribute in itemsString)
	    {
		 
       // for (String attribute : itemsString) {
			// convert from string to int
			int item = int.Parse(attribute);
			// add the item to the current transaction
			itemset.Add(item); 
			// add the item to the set of all items in this database
			items.Add(item);
		}
		// add the transactions to the list of all transactions in this database.
		transactions.Add(itemset);
	}

	/**
	 * Method to print the content of the transaction database to the console.
	 */
	public void printDatabase() {
		Console.WriteLine("===================  TRANSACTION DATABASE ===================");
		int count = 0; 
		// for each transaction
		foreach (List<int> itemset in transactions)
	    {
		 
        //for (List<Integer> itemset : transactions) { // pour chaque objet
			Console.WriteLine("0" + count + ":  ");
			print(itemset); // print the transaction 
			Console.WriteLine("");
			count++;
		}
	}
	
	/**
	 * Method to print a transaction to System.out.
	 * @param itemset a transaction
	 */
	private void print(List<int> itemset){
		StringBuilder r = new StringBuilder();
		// for each item in this transaction
		foreach (int item in itemset)
	    {
        //for (Integer item : itemset) {
			// append the item to the stringbuffer
			r.Append(item.ToString());
			r.Append(' ');
		}
		Console.WriteLine(r); // print to System.out
	}

	/**
	 * Get the number of transactions in this transaction database.
	 * @return the number of transactions.
	 */
	public int size() {
		return transactions.Count();
	}

	/**
	 * Get the list of transactions in this database
	 * @return A list of transactions (a transaction is a list of Integer).
	 */
	public List<List<int>> getTransactions() {
		return transactions;
	}

	/**
	 * Get the set of items contained in this database.
	 * @return The set of items.
	 */
	public HashSet<int> getItems() {
		return items;
	}


    }
}
