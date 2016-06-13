using System.Collections;
using System.Collections.Generic;

//Only for Vectors
using UnityEngine;

public class AStar {

	public enum NodeStatus { Open, Closed };
	private static Dictionary<Vector3, NodeStatus> _nodeStatus =
		new Dictionary<Vector3, NodeStatus>();

	private const float StraightCost = 1.0f;
	private const float DiagonalCost = 1.5f;

	private static List<PathNode> _openList =
		new List<PathNode>();
	public static Dictionary<Vector3, NodeStatus> NodeStatuses { get { return _nodeStatus; } }

	private static Dictionary<Vector3, float> _nodeCosts =
		new Dictionary<Vector3, float>();

	private static int width;
	private static int height;

	private static bool[,] _walkableMatrix;

	public static List<PathNode> FindPath(bool[,] walkableMatrix, Vector3 start, Vector3 end)
	{
		if (!walkableMatrix[(int)start.x, (int)start.y] || !walkableMatrix[(int)end.x, (int)end.y])
		{
			return null;
		}

		width = walkableMatrix.GetLength(0);
		height = walkableMatrix.GetLength(1);
		_walkableMatrix = walkableMatrix;

		_openList.Clear();
		_nodeStatus.Clear();
		_nodeCosts.Clear();

		PathNode startNode;
		PathNode endNode;

		endNode = new PathNode(null, null, end, 0);
		startNode = new PathNode(null, endNode, start, 0);

		AddNodeToOpenList(startNode);

		while (_openList.Count > 0)
		{
			PathNode currentNode = _openList[_openList.Count - 1];

			if (currentNode.EqualsPathNode(endNode))
			{
				List<PathNode> bestPath = new List<PathNode>();
				while(currentNode != null)
				{
					bestPath.Insert(0, currentNode);
					currentNode = currentNode.ParentNode;
				}
				return bestPath;
			}

			_openList.Remove(currentNode);
			_nodeCosts.Remove(currentNode.Position);
		
			foreach(PathNode possibleNode in 
			        FindConnectingNodes(currentNode, endNode))
			{
				if (_nodeStatus.ContainsKey(possibleNode.Position))
				{
					if (_nodeStatus[possibleNode.Position] ==
					    NodeStatus.Closed)
					{
						continue;
					}
					else
					{
						if (possibleNode.TotalCost >=
						    _nodeCosts[possibleNode.Position])
						{
							continue;
						}
					}
				}
				AddNodeToOpenList(possibleNode);
			}
			_nodeStatus[currentNode.Position] = NodeStatus.Closed;
		}

		return null;
	}

	private static List<PathNode> FindConnectingNodes(
		PathNode currentNode, PathNode endNode)
	{
		int X = (int)currentNode.Position.x;
		int Y = (int)currentNode.Position.y;

		List<PathNode> connectingNodes = new List<PathNode>();

		bool upLeft = true;
		bool upRight = true;
		bool downLeft = true;
		bool downRight = true;

		if ((X > 0) && (_walkableMatrix[X - 1, Y]))
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector3(X - 1, Y, 0), StraightCost + currentNode.DirectCost));
		}
		else
		{
			upLeft = false;
			downLeft = false;
		}
		if ((X < (width - 1)) && (_walkableMatrix[X + 1, Y]))
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X + 1, Y), StraightCost + currentNode.DirectCost));
		}
		else
		{
			upRight = false;
			downRight = false;
		}
		if ((Y > 0) && (_walkableMatrix[X, Y - 1]))
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X, Y - 1), StraightCost + currentNode.DirectCost));
		}
		else
		{
			upLeft = false;
			upRight = false;
		}
		if ((Y < (height - 1)) && _walkableMatrix[X, Y + 1])
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X, Y + 1), StraightCost + currentNode.DirectCost));
		}
		else
		{
			downLeft = false;
			downRight = false;
		}
		if ((upLeft) && _walkableMatrix[X - 1, Y - 1])
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X - 1, Y - 1), DiagonalCost + currentNode.DirectCost));
		}
		if ((upRight) && _walkableMatrix[X + 1, Y - 1])
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X + 1, Y - 1), DiagonalCost + currentNode.DirectCost));
		}
		if ((downLeft) && _walkableMatrix[X - 1, Y + 1])
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X - 1, Y + 1), DiagonalCost + currentNode.DirectCost));
		}
		if ((downRight) && _walkableMatrix[X + 1, Y + 1])
		{
			connectingNodes.Add(new PathNode(currentNode, endNode, new Vector2(X + 1, Y + 1), DiagonalCost + currentNode.DirectCost));
		}

		return connectingNodes;
	}

	private static void AddNodeToOpenList(PathNode node)
	{
		int index = 0;
		float cost = node.TotalCost;

		while ((_openList.Count > index) &&
		       (cost < _openList[index].TotalCost))
		{
			index++;
		}

		_openList.Insert(index, node);
		_nodeCosts[node.Position] = node.TotalCost;
		_nodeStatus[node.Position] = NodeStatus.Open;
	}

	private float Distance(PathNode n1, PathNode n2)
	{
		//Heuristic function
		//Currently just distance between positions.
		return n1.Distance(n2);
	}
}
