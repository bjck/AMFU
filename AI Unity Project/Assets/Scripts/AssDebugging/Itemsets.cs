using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using patterns.itemset;
namespace patterns.itemsets
{
    public class Itemsets
    {
        /** We store the itemsets in a list named "levels".
         Position i in "levels" contains the list of itemsets of size i */
        private List<List<Itemset>> levels = new List<List<Itemset>>();
        /** the total number of itemsets */
        private int itemsetsCount = 0;
        /** a name that we give to these itemsets (e.g. "frequent itemsets") */
        private string name;

        /**
         * Constructor
         * @param name the name of these itemsets
         */
        public Itemsets(string name)
        {
            this.name = name;
            levels.Add(new List<Itemset>()); // We create an empty level 0 by
            // default.
        }

        /**
         * Print all itemsets to System.out, ordered by their size.
         * @param nbObject The number of transaction/sequence in the database where
         * there itemsets were found.
         */
        public void printItemsets(int nbObject)
        {
            Console.WriteLine(" ------- " + name + " -------");
            int patternCount = 0;
            int levelCount = 0;
            // for each level (a level is a set of itemsets having the same number of items)
            foreach (List<Itemset> level in levels)
            {
                //for (List<Itemset> level : levels) {
                // print how many items are contained in this level
                Console.WriteLine("  L" + levelCount + " ");
                // for each itemset
                foreach (Itemset itemset in level)
                {
                    //for (Itemset itemset : level) {
                    // print the itemset
                    Console.WriteLine("  pattern " + patternCount + ":  ");
                    itemset.print();
                    // print the support of this itemset
                    Console.WriteLine("support :  "
                                 + itemset.getRelativeSupportAsString(nbObject));
                    patternCount++;
                    Console.WriteLine("");
                }
                levelCount++;
            }
            Console.WriteLine(" --------------------------------");
        }

        /** 
         * Add an itemset to this structure
         * @param itemset the itemset
         * @param k the number of items contained in the itemset
         */
        public void addItemset(Itemset itemset, int k)
        {
            while (levels.Count <= k)
            {
                levels.Add(new List<Itemset>());
            }
            levels.ElementAt(k).Add(itemset);
            itemsetsCount++;
        }

        /**
         * Get all itemsets.
         * @return A list of list of itemsets.
         * Position i in this list is the list of itemsets of size i.
         */
        public List<List<Itemset>> getLevels()
        {
            return levels;
        }

        /**
         * Get the total number of itemsets
         * @return the number of itemsets.
         */
        public int getItemsetsCount()
        {
            return itemsetsCount;
        }
    }
}
