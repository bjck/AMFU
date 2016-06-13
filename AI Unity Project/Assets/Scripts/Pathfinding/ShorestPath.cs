using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShorestPath
{
	//Contains all nodes and egdes of the graph
	private List<Node> Nodes = new List<Node>();
	private List<Edge> Edges = new List<Edge>();
	private double[,] adjacencyMatrix;


	private int uniqueIDCounter = 0;

	public void CreateEgde(int source, int goal, double distance)
	{
		Edge egde = new Edge(source, goal, distance);
		Edges.Add(egde);
	}

	/// <summary>
	/// Creates the node.
	/// </summary>
	/// <returns>The node ID.</returns>
	/// <param name="position">Position.</param>
	public int CreateNode(Vector3 position)
	{
		Node node = new Node(position, uniqueIDCounter);
		Nodes.Add (node);
		uniqueIDCounter++;
		return uniqueIDCounter-1;
	}

	private void CreateMatrix()
	{
		adjacencyMatrix = new double[Nodes.Count, Edges.Count]; 

		foreach(Edge egde in Edges)
		{
			adjacencyMatrix[egde.Source, egde.Goal] = egde.Distance;
		}
	}

	public void Dijkstra(int startNode)
	{
		Dijkstras dijkstra = new Dijkstras(adjacencyMatrix, startNode);
//		path = dijkstra.Path;
	}

	public int[] GetShortestPathTo(int nodeID)
	{
		throw new NotImplementedException();
	}

}
