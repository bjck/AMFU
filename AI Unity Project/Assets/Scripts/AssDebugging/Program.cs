using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using patterns.itemsets;
using patterns.itemset;
using frequent.patterns;
namespace AssRules
{
    public class Program
    {
        public static void RunAss()
        {
            string input = "contextZart.txt";
            string output = "jegeroutfilen.txt";

            double minsupp = 0.6;
            double minconf = 0.6;

            TransactionDatabase database = new TransactionDatabase();
            try
            {
                database.loadFile(input);
            }
            catch(Exception e) {
                Console.WriteLine(e.StackTrace.ToString());
            }


            AlgoAprioriTIDClose aclose = new AlgoAprioriTIDClose();
            Itemsets patterns = aclose.runAlgorithm(database, minsupp, null);
            aclose.printStats();
            AlgoClosedRules algoClosedrules = new AlgoClosedRules();
            AssRules.ClosedRules rules = algoClosedrules.runAlgorithm(patterns, minconf, output);
            algoClosedrules.printStatistics();
            //Console.WriteLine("Printer reglerne:");
            
            if (rules != null)rules.printRules(database.size());
        }
    }
}
