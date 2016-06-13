using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AssRules
{
    public class ClosedRules
    {
        
	// the set of rules
	public List<ClosedRule> rules = new List<ClosedRule>();  // rules
	
	// the name of this set of rules
	private string name;
	
	/**
	 * The constructor.
	 * @param name the name of this set of rules
	 */
	public ClosedRules(string name){
		this.name = name;
	}
	
	/**
	 * Print the rules to System.out
	 * @param databaseSize  the number of transactions in the database where the rules were found.
	 */
	public void printRules(int databaseSize){
       
		Console.WriteLine(" ------- " + name + " -------");
		int i=0;
		foreach (ClosedRule rule in rules) {
            //A small hack, to avoid INF - Should sort the transactions in the right order to properly avoid
            if (rule.getConfidence() <= 1)
            {
                //for(ClosedRule rule : rules){
                Console.WriteLine("  rule " + i + ":  " + rule.toString());
                Console.WriteLine("support :  " + rule.getRelativeSupport(databaseSize) +
                                 " (" + rule.getAbsoluteSupport() + "/" + databaseSize + ") ");
                Console.WriteLine("confidence :  " + rule.getConfidence());
                Console.WriteLine("");
                i++;
            }
		}
		Console.WriteLine(" --------------------------------");
	}
	
	/**
	 * Return a string representation of the rules.
	 * @param databaseSize  the number of transactions in the database where the rules were found.
	 */
	public string toString(int databaseSize){
		StringBuilder buffer = new StringBuilder(" ------- ");
		buffer.Append(name);
		buffer.Append(" -------\n");
		int i=0;
        foreach (ClosedRule rule in rules)
        {
            if (rule.getConfidence() <= 1)
            {
                //for(ClosedRule rule : rules){
                //			System.out.println("  L" + j + " ");
                buffer.Append("   rule ");
                buffer.Append(i);
                buffer.Append(":  ");
                buffer.Append(rule.toString());
                buffer.Append("support :  ");
                buffer.Append(rule.getRelativeSupport(databaseSize));

                buffer.Append(" (");
                buffer.Append(rule.getAbsoluteSupport());
                buffer.Append("/");
                buffer.Append(databaseSize);
                buffer.Append(") ");
                buffer.Append("confidence :  ");
                buffer.Append(rule.getConfidence());
                buffer.Append("\n");
                i++;
            }
        }
		return buffer.ToString();
	}
	
	/**
	 * Add a rule.
	 * @param rule a rule
	 */
	public void addRule(ClosedRule rule){
		rules.Add(rule);
	}
	
	/**
	 * Get the number of rules.
	 * @return the number of rules (integer)
	 */
	public int getRulesCount(){
		return rules.Count;
	}
	
	/**
	 * Get the list of rules.
	 * @return a list of rules.
	 */
	public List<ClosedRule> getRules() {
		return rules;
	}
}

    
}
