using System.Collections;
using patterns.itemset;

public class ClosedRule
{
    private Itemset itemset1; // antecedent
    private Itemset itemset2; // consequent
    private int transactionCount; // absolute support
    private double confidence;

    /**
     * Constructor
     * @param itemset1 an itemset that is the left side of the rule
     * @param itemset2 an itemset that is the right side of the rule
     * @param transactionCount the support of the rule (integer)
     * @param confidence the confidence of the rule
     */
    public ClosedRule(Itemset itemset1, Itemset itemset2, int transactionCount, double confidence)
    {
        this.itemset1 = itemset1;
        this.itemset2 = itemset2;
        this.transactionCount = transactionCount;
        this.confidence = confidence;
    }

    //test på enumerator
    public void GetEnumerator(){
    }


    /**
     * Get the relative support of this rule (percentage)
     * @return the relative support
     */
    public double getRelativeSupport(int objectCount)
    {
        return ((double)transactionCount) / ((double)objectCount);
    }

    /**
     * Get the absolute support of this rule (integer)
     * @return the support
     */
    public int getAbsoluteSupport()
    {
        return transactionCount;
    }

    /**
     * Get the confidence of the rule.
     * @return the confidence (double)
     */
    public double getConfidence()
    {
        return confidence;
    }

    /**
     * Print this rule to System.out
     */
    public void print()
    {
        System.Console.WriteLine(toString());
    }

    /**
     * Return a string representation of this rule
     */
    public string toString()
    {
        return itemset1.toString() + " ==> " + itemset2.toString();
    }

    /**
     * Get the left side of the rule.
     * @return an itemset
     */
    public Itemset getItemset1()
    {
        return itemset1;
    }

    /**
     * Get the right side of the rule.
     * @return an itemset
     */
    public Itemset getItemset2()
    {
        return itemset2;
    }


}
