using UnityEngine;
using System.Collections;

public class NavMeshEdge {
	public float Distance;
	public NavMeshNode StartNode;
	public NavMeshNode EndNode;

	public NavMeshEdge(NavMeshNode startNode, NavMeshNode endNode) {
		StartNode = startNode;
		EndNode = endNode;
		Distance = Vector3.Distance(startNode.Centroid, endNode.Centroid);
	}
}
