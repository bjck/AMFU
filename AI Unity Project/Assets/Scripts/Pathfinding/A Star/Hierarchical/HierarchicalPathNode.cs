using UnityEngine;
using System.Collections;

public class HierarchicalPathNode {
	public HierarchicalNode Node { get; private set; }
	
	public HierarchicalPathNode ParentNode;
	public HierarchicalPathNode EndNode;
	
	public float TotalCost;
	public float DirectCost;
	
	public bool Walkable = true;
	
	public HierarchicalPathNode(HierarchicalPathNode parentNode, 
	                            HierarchicalPathNode endNode, 
	                            HierarchicalNode node, 
	                			float cost)
	{
		ParentNode = parentNode;
		EndNode = endNode;
		Node = node;
		DirectCost = cost;
		
		if (endNode!= null) TotalCost = DirectCost + Distance(endNode);
	}
	
	public float Distance(HierarchicalPathNode otherNode)
	{
		//TODO find distance
		return Vector3.Distance(this.Node._graphPos, otherNode.Node._graphPos);
	}
	
	public bool EqualsPathNode (HierarchicalPathNode otherNode)
	{
		return (otherNode.Node == this.Node);
	}

	public override bool Equals (object obj)
	{
		return EqualsPathNode(obj as HierarchicalPathNode);
	}

	public override int GetHashCode ()
	{
		return Node.GetHashCode();
	}
}
