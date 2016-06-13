using UnityEngine;
using System.Collections;

public class PathNode {
	public Vector3 Position { get; private set; }

	public PathNode ParentNode;
	public PathNode EndNode;

	public float TotalCost;
	public float DirectCost;

	public bool Walkable = true;

	public PathNode(PathNode parentNode, 
	                 PathNode endNode, 
	                 Vector3 position, 
	                 float cost)
	{
		ParentNode = parentNode;
		EndNode = endNode;
		Position = position;
		DirectCost = cost;

		if (endNode!= null) TotalCost = DirectCost + Distance(endNode);
	}

	public float Distance(PathNode otherNode)
	{
		return Vector3.Distance(this.Position, otherNode.Position);
	}

	public bool EqualsPathNode (PathNode node)
	{
		return (node.Position == this.Position);
	}
}
