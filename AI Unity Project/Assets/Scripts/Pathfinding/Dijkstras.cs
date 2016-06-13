using System.Collections;
using System.Collections.Generic;

public class Dijkstras
{
	private double[] distance;
	private int[] path;

	public double[] Distance{get{return distance;}}
	public int[] Path{get{return path;}}

	//Holds the Queue for the nodes that need to be evaluated
	private List<int> queue = new List<int>();

	private void Initilize(int startingIndex, int length)
	{
		distance = new double[length];
		path = new int[length];
		for(int i = 0; i < length; i++)
		{
			distance[i] = double.PositiveInfinity;
			queue.Add(i);
		}
		//Distance to start is zero and no previous node
		distance[startingIndex] = 0;
		path[startingIndex] = -1;
	}

	private int GetNextNode()
	{
		int node = -1;
		//Setting it to infinite to find nodes not visited
		double minimum = double.PositiveInfinity;

		foreach(int i in queue)
		{
			if(distance[i] <= minimum)
			{
				minimum = distance[i];
				node = i;
			}
		}
		queue.Remove(node);
		return node;
	}
	
	public Dijkstras(double[,] adjacencyMatrix, int startIndex)
	{
		int length = adjacencyMatrix.GetLength (0);

		Initilize(startIndex, length);

		while(queue.Count > 0)
		{
			int node = GetNextNode ();
			for(int tempNode = 0; tempNode < length; tempNode++)
			{
				//check if there is an egde between node 
				if(adjacencyMatrix[node, tempNode] > 0)
				{
					//relaxation
					if(distance[tempNode] > distance[node] + adjacencyMatrix[node, tempNode])
					{
						distance[tempNode] = distance[node] + adjacencyMatrix[node, tempNode];
						path[tempNode] = node;
					}
				}
			}
		}
	}
}
