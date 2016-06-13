using UnityEngine;
using System.Collections;

public class Node
{
	public int id;
	public int distance;
	public Vector3 position;

	public Node(Vector3 position, int id)
	{
		this.position = position;
		this.id = id;
	}
}
