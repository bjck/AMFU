using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphHelper {

	public static bool UseGrid = true;

	public static Graph CurrentGraph
	{
		get 
		{
			if (UseGrid)
				return _currentGridGraph;
			else
				return _currentNavMeshGraph;
		}
	}

	private static Graph _currentNavMeshGraph;
	private static Graph _currentGridGraph;

	public static Graph GenerateGraph0(List<NavMeshNode> nodes, List<NavMeshEdge> edges)
	{
		List<HierarchicalNode> graphNodes = new List<HierarchicalNode>();
		foreach(NavMeshNode node in nodes)
		{
			HierarchicalNode graphNode = new HierarchicalNode(0, node.Centroid, null, true, node.Vertices);
			graphNodes.Add(graphNode);
		}

		foreach(HierarchicalNode node in graphNodes)
		{
			Dictionary<HierarchicalNode, float> connections = new Dictionary<HierarchicalNode, float>();
		 	foreach(NavMeshEdge edge in edges.FindAll(x => x.StartNode.Centroid == node._graphPos))
			{
				HierarchicalNode toNode = graphNodes.Find(x=>x._graphPos == edge.EndNode.Centroid);
				if (!connections.ContainsKey(toNode))
				{
					connections.Add(graphNodes.Find(x=>x._graphPos == edge.EndNode.Centroid), edge.Distance);
				}
			}
			node.SetConnections(connections);
		}

		_currentNavMeshGraph = new Graph(graphNodes);
		return _currentNavMeshGraph;
	}

	public static Graph GenerateGraph0(bool[,] obstructionGrid, Vector3 startPos, Vector2 tileSize, bool useDiagonalMoves)
	{
		int graphWidth = obstructionGrid.GetLength(0);
		int graphLength = obstructionGrid.GetLength(1);
		HierarchicalNode[,] graph = new HierarchicalNode[graphWidth, graphLength];
		
		for (int x = 0; x < graphWidth; x++)
		{
			for (int y = 0; y < graphLength; y++)
			{
				//TODO Calculate position of node with size of grid and tiles in mind
				graph[x,y] = new HierarchicalNode(0, startPos + new Vector3(x * tileSize.x, 0, y * tileSize.y) + new Vector3(tileSize.x / 2, 0, tileSize.y / 2), null, !obstructionGrid[x,y]);
			}
		}

		_currentGridGraph = new Graph(graph, startPos, tileSize, useDiagonalMoves);
		return _currentGridGraph;
	}
}
