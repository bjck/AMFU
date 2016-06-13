using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// http://www.matthughson.com/2013/04/07/dynamic-hpa-part-2/
/// </summary>

public class HierarchicalAStar {
	
	public enum NodeStatus { Open, Closed };
	private static Dictionary<HierarchicalPathNode, NodeStatus> _nodeStatus =
		new Dictionary<HierarchicalPathNode, NodeStatus>();
	
	private const float StraightCost = 1.0f;
	private const float DiagonalCost = 1.5f;
	
	private static List<HierarchicalPathNode> _openList =
		new List<HierarchicalPathNode>();
	public static Dictionary<HierarchicalPathNode, NodeStatus> NodeStatuses { get { return _nodeStatus; } }
	
	private static Dictionary<HierarchicalPathNode, float> _nodeCosts =
		new Dictionary<HierarchicalPathNode, float>();

	private static bool InSight(Vector3 fromNode, Vector3 toNode)
	{
		RaycastHit hit = new RaycastHit();
		Vector3 direction = toNode - fromNode;
		Physics.CapsuleCast(fromNode - new Vector3(0, 0.5f, 0), fromNode + new Vector3(0, 0.5f, 0), 0.5f, direction.normalized, out hit, direction.magnitude);
		if (hit.collider != null)
			return false;
		else
			return true;
	}

	//TODO return vectors instead
	public static List<Vector3> FindPath(Graph level, Vector3 start, Vector3 end)
	{
		if (!level.GetNode(start).walkable || !level.GetNode(end).walkable)
		{
			return null;
		}
		
		_openList.Clear();
		_nodeStatus.Clear();
		_nodeCosts.Clear();
		
		HierarchicalPathNode startNode;
		HierarchicalPathNode endNode;
		
		endNode = new HierarchicalPathNode(null, null, level.GetNode(end), 0);
		startNode = new HierarchicalPathNode(null, endNode, level.GetNode(start), 0);

		if(InSight(start, end))
		{
			return new List<Vector3>(){start, end};
		}
		
		AddNodeToOpenList(startNode);
		
		while (_openList.Count > 0)
		{
			HierarchicalPathNode currentNode = _openList[_openList.Count - 1];
			
			if (currentNode.EqualsPathNode(endNode))
			{
				List<Vector3> bestPath = new List<Vector3>();
				while(currentNode != null)
				{
					//Check with end pos, not end node
					bestPath.Insert(0, currentNode.Node._graphPos);
					/*while ((currentNode.ParentNode != null && currentNode.ParentNode.ParentNode != null) && InSight(currentNode.Node._graphPos, currentNode.ParentNode.ParentNode.Node._graphPos))
					{
						currentNode.ParentNode = currentNode.ParentNode.ParentNode;
					}*/
					currentNode = currentNode.ParentNode;
				}
				//Check with start pos, not start node
				return bestPath;
			}
			
			_openList.Remove(currentNode);
			_nodeCosts.Remove(currentNode);

			foreach(HierarchicalPathNode possibleNode in GetConnectingNodes(currentNode, endNode))
			{
				if (_nodeStatus.ContainsKey(possibleNode))
				{
					if (_nodeStatus[possibleNode] ==
					    NodeStatus.Closed)
					{
						continue;
					}
					else
					{
						if (possibleNode.TotalCost >=
						    _nodeCosts[possibleNode])
						{
							continue;
						}
					}
				}
				AddNodeToOpenList(possibleNode);
			}
			_nodeStatus[currentNode] = NodeStatus.Closed;
		}
		
		return null;
	}

	private static void AddNodeToOpenList(HierarchicalPathNode node)
	{
		int index = 0;
		float cost = node.TotalCost;
		
		while ((_openList.Count > index) &&
		       (cost < _openList[index].TotalCost))
		{
			index++;
		}
		
		_openList.Insert(index, node);
		_nodeCosts[node] = node.TotalCost;
		_nodeStatus[node] = NodeStatus.Open;
	}

	private static List<HierarchicalPathNode> GetConnectingNodes(HierarchicalPathNode currentNode, HierarchicalPathNode endNode)
	{
		List<HierarchicalPathNode> connectingNodes = new List<HierarchicalPathNode>();

		foreach(HierarchicalNode node in currentNode.Node.ConnectingNodes.Keys)
		{
			connectingNodes.Add(new HierarchicalPathNode(currentNode, endNode, node, currentNode.Node.ConnectingNodes[node] + currentNode.DirectCost));
		}

		return connectingNodes;
	}
}
