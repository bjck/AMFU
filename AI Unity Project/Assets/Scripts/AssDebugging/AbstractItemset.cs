﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace patterns
{
    public abstract class AbstractItemset
    {
        public AbstractItemset() : base() { }
	
	/**
	 * Get the size of this itemset
	 * @return the size of this itemset
	 */
	public abstract int size();
	
	/**
	 * Get this itemset as a string
	 * @return a string representation of this itemset
	 */
	public abstract string toString();
	
	
	/**
	 * print this itemset to System.out.
	 */
	public void print() {
		Console.WriteLine(toString());
	}
	
	
	/**
	 * Get the support of this itemset
	 * @return the support of this itemset
	 */
	public abstract int getAbsoluteSupport();
	
	/**
	 * Get the relative support of this itemset (a percentage) as a double
	 * @param nbObject  the number of transactions in the database where this itemset was found
	 * @return the relative support of the itemset as a double
	 */
	public abstract double getRelativeSupport(int nbObject);
	
	/**
	 * Get the relative support of this itemset as a string
	 * @param nbObject  the number of transactions in the database where this itemset was found
	 * @return the relative support of the itemset as a string
	 */
	public String getRelativeSupportAsString(int nbObject) {
		// get the relative support
		double frequence = getRelativeSupport(nbObject);
		// convert it to a string with two decimals

        NumberFormatInfo format = new NumberFormatInfo();
        format.NumberDecimalSeparator = ".";

        /*DecimalFormat format = new DecimalFormat();
		format.setMinimumFractionDigits(0); 
		format.setMaximumFractionDigits(5); 
		return format.format(frequence);*/

        return frequence.ToString(format);
	}
	
	
	/**
	 * Check if this itemset contains a given item.
	 * @param item  the item
	 * @return true if the item is contained in this itemset
	 */
	public abstract bool contains(int item);
	
	}
    }
