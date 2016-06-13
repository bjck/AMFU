using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HierarchicalNode {
	int _level;
	public Vector3 _graphPos;

	public Graph InnerGraph { get; private set; }

	public Dictionary<HierarchicalNode, float> ConnectingNodes { get; private set; }

	public readonly bool walkable;

	public readonly Vector3[] Vertices;

	public HierarchicalNode(int level, Vector3 graphPos, Graph innerGraph, bool _walkable)
	{
		walkable = _walkable;
		_level = level;
		InnerGraph = innerGraph;
		_graphPos = graphPos;
	}

	public HierarchicalNode(int level, Vector3 graphPos, Graph innerGraph, bool _walkable, Vector3[] vertices) : 
		this (level, graphPos, innerGraph, _walkable)
	{
		Vertices = vertices;
	}

	public HierarchicalNode(HierarchicalNode copy)
	{
		this.walkable = copy.walkable;
		this._level = copy._level;
		this.InnerGraph = copy.InnerGraph;
		this._graphPos = copy._graphPos;
	}

	public void SetConnections(Dictionary<HierarchicalNode, float> nodes)
	{
		ConnectingNodes = nodes;
	}

	public override bool Equals (object obj)
	{
		if (obj.GetType() == typeof(HierarchicalNode))
		{
			return this._graphPos == ((HierarchicalNode)obj)._graphPos && this._level == ((HierarchicalNode)obj)._level;
		}
		return base.Equals (obj);
	}
	public override int GetHashCode ()
	{
		return (int)(_graphPos.x * 10000 + _graphPos.y);
	}
}
