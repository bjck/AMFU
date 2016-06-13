using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace patterns.itemset
{
    public class Itemset : AbstractOrderedItemset
    {
        	/** The list of items contained in this itemset, ordered by 
	 lexical order */
	public int[] itemset; 
	/** The set of transactions/sequences id containing this itemset */
	public HashSet<int> transactionsIds = new HashSet<int>();
	
	/**
	 * Constructor
	 */
	public Itemset() {

	}
	/**
	 * Constructor 
	 * @param item an item that should be added to the new itemset
	 */
	public Itemset(int item){
		itemset = new int[]{item};
	}
	
	/**
	 * Constructor 
	 * @param items an array of items that should be added to the new itemset
	 */
	public Itemset(int [] items){
		this.itemset = items;
	}
	
	/**
	 * Get the support of this itemset (as an integer)
	 */
	public override int getAbsoluteSupport() {
		return transactionsIds.Count();
	}
	
	
	/**
	 * Get the items in this itemset as a list.
	 * @return the items.
	 */
	public int[] getItems() {
		return itemset;
	}
	
	/**
	 * Get the item at a given position.
	 * @param index the position
	 * @return the item
	 */
	public override int get(int index) {
		return itemset[index];
	}
	
	/**
	 * Set the list of transaction/sequence ids containing this itemset
	 * @param listTransactionIds  the list of transaction/sequence ids
	 */
	public void setTIDs(HashSet<int> listTransactionIds) {
		this.transactionsIds = listTransactionIds;
	}
	
	/**
	 * Get the size of this itemset.
	 */
	public override int size() {
        //return itemset.Length;
        return itemset.Count();
	}
	
	/**
	 * Get the list of sequence/transaction ids containing this itemset.
	 * @return the list of transaction ids.
	 */
	public HashSet<int> getTransactionsIds() {
		return transactionsIds;
	}
	
	/**
	 * Make a copy of this itemset but exclude a set of items
	 * @param itemsetToNotKeep the set of items to be excluded
	 * @return the copy
	 */
	public Itemset cloneItemSetMinusAnItemset(Itemset itemsetToNotKeep) {
		// create a new itemset
		int[] newItemset = new int[itemset.Length - itemsetToNotKeep.size()];
		int i=0;
		// for each item of this itemset
		for(int j =0; j < itemset.Length; j++){
			// copy the item except if it is not an item that should be excluded
			if(itemsetToNotKeep.contains(itemset[j]) == false){
				newItemset[i++] = itemset[j];
			}
		}
		return new Itemset(newItemset); // return the copy
	}
	
	/**
	 * Make a copy of this itemset but exclude a given item
	 * @param itemsetToRemove the given item
	 * @return the copy
	 */
	public Itemset cloneItemSetMinusOneItem(int itemsetToRemove) {
		// create the new itemset
		int[] newItemset = new int[itemset.Length -1];
		int i=0;
		// for each item in this itemset
		for(int j =0; j < itemset.Length; j++){
			// copy the item except if it is the item that should be excluded
			if(itemset[j] != itemsetToRemove){
				newItemset[i++] = itemset[j];
			}
		}
		return new Itemset(newItemset); // return the copy
	}
	
	}
    
}
