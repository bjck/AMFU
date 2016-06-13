using UnityEngine;
using System.Collections;

public class GridGenerator : MonoBehaviour {

	GameObject[,] _matrix;
	bool[,] _walkableMatrix;
	[SerializeField]
	GameObject _cube;
	[SerializeField]
	GameObject _base;

	public static Graph level;

	// Use this for initialization
	void Start () {
		GenerateGrid(50, 50, 1);
		//level = GraphHelper.GenerateGraph0(_walkableMatrix, true);
	}

	GameObject _currentCube;
	bool[,] GenerateGrid(int width, int length, int height)
	{
		_matrix = new GameObject[width, length];
		_walkableMatrix = new bool[width, length];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < length; y++)
			{
				_currentCube = (GameObject)Instantiate(_cube, new Vector3(x, -1f, y), Quaternion.identity);
				_currentCube.hideFlags = HideFlags.HideInHierarchy;
				_currentCube.name = "Node";

				if (Random.Range(0, 10) < 7)
				{
					_currentCube.renderer.material.color = Color.white;
				}
				else
				{
					_currentCube.renderer.material.color = Color.black;
					_currentCube.transform.localScale += new Vector3(0,2,0);
				}

				_currentCube.transform.parent = _base.transform;

				_matrix[x,y] = _currentCube;
				_walkableMatrix[x,y] = _currentCube.renderer.material.color != Color.black;
			}
		}

		return _walkableMatrix;
	}

	GameObject _currentStart = null;
	public static GameObject _currentEnd = null;
	System.Collections.Generic.List<PathNode> path;
	System.Collections.Generic.List<Vector3> hierarchicalPath;
	void Update()
	{    
		if (Input.GetKeyUp(KeyCode.Z))
		{
			if (_currentStart != null && _currentEnd != null)
			{
				if (path != null)
				{
					foreach (System.Collections.Generic.KeyValuePair<Vector3, AStar.NodeStatus> node in AStar.NodeStatuses)
					{
						if (path.Exists(x => x.Position == node.Key))
						{
							continue;
						}
						else
						{
							_matrix[(int)node.Key.x, (int)node.Key.y].renderer.material.color = Color.white;
						}
					}
					foreach(PathNode node in path)
					{
						if (node.Position == _currentStart.transform.position ||
						    node.Position == _currentEnd.transform.position)
						{
							continue;
						}
						
						_matrix[(int)node.Position.x, (int)node.Position.y].renderer.material.color = Color.white;
					}
				}
				
				path = AStar.FindPath(_walkableMatrix, _currentStart.transform.position, _currentEnd.transform.position);
				
				if (path != null)
				{
					Debug.Log("Path found");
					
					foreach(PathNode node in path)
					{
						if (node.Position == _currentStart.transform.position ||
						    node.Position == _currentEnd.transform.position)
						{
							continue;
						}
						
						_matrix[(int)node.Position.x, (int)node.Position.y].renderer.material.color = Color.gray;
					}
					
					foreach (System.Collections.Generic.KeyValuePair<Vector3, AStar.NodeStatus> node in AStar.NodeStatuses)
					{
						if (path.Exists(x => x.Position == node.Key))
						{
							continue;
						}
						else
						{
							if (node.Value == AStar.NodeStatus.Closed)
								_matrix[(int)node.Key.x, (int)node.Key.y].renderer.material.color = Color.blue;
							else if (node.Value == AStar.NodeStatus.Open)
								_matrix[(int)node.Key.x, (int)node.Key.y].renderer.material.color = Color.cyan;
						}
					}
				}
				else
				{
					Debug.Log("Path 'NOT' found");
				}
			}
		}
		else if (Input.GetKeyUp(KeyCode.X))
		{
			if (_currentStart != null && _currentEnd != null)
			{
				hierarchicalPath = HierarchicalAStar.FindPath(level, _currentStart.transform.position, _currentEnd.transform.position);

				if (hierarchicalPath != null)
				{
					Debug.Log("Path found");

					foreach(Vector3 node in hierarchicalPath)
					{
						if (node == _currentStart.transform.position ||
						    node == _currentEnd.transform.position)
						{
							continue;
						}
						
						_matrix[(int)node.x, (int)node.y].renderer.material.color = Color.gray;
					}

					foreach (System.Collections.Generic.KeyValuePair<HierarchicalPathNode, HierarchicalAStar.NodeStatus> node in HierarchicalAStar.NodeStatuses)
					{
						if (hierarchicalPath.Exists(x => x == node.Key.Node._graphPos))
						{
							continue;
						}
						else
						{
							if (node.Value == HierarchicalAStar.NodeStatus.Closed)
								_matrix[(int)node.Key.Node._graphPos.x, (int)node.Key.Node._graphPos.y].renderer.material.color = Color.blue;
							else if (node.Value == HierarchicalAStar.NodeStatus.Open)
								_matrix[(int)node.Key.Node._graphPos.x, (int)node.Key.Node._graphPos.y].renderer.material.color = Color.cyan;
						}
					}
				}
				else
				{
					Debug.Log("Path not found");
				}
			}
		}
		else if ( Input.GetMouseButtonUp(0)){
			Debug.Log("Mouse 0");
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 100.0f)){
				Debug.Log(hit.collider.name);
				if (hit.collider.name == "Node")
				{
					if (_matrix[(int)hit.collider.transform.position.x, 
					            (int)hit.collider.transform.position.z].renderer.material.color != Color.black)
					{
						if (_currentStart != null)
							_currentStart.renderer.material.color = Color.white;
						
						hit.collider.renderer.material.color = Color.green;
						
						_currentStart = hit.collider.gameObject;
					}
				}
			}
		}
		else if ( Input.GetMouseButtonUp(1)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 100.0f)){
				if (hit.collider.name == "Node")
				{
					if (_matrix[(int)hit.collider.transform.position.x, 
					            (int)hit.collider.transform.position.z].renderer.material.color != Color.black)
					{
						if (_currentEnd != null)
							_currentEnd.renderer.material.color = Color.white;
						
						hit.collider.renderer.material.color = Color.red;
						
						_currentEnd = hit.collider.gameObject;
					}
				}
			}
		}
	}
}
