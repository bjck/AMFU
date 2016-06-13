using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshHelper : MonoBehaviour {
	
	private Mesh navMesh;
	private List<NavMeshNode> navMeshNodes = new List<NavMeshNode>();
	private List<NavMeshEdge> navMeshEdges = new List<NavMeshEdge>();
	public static Graph levelGraph;

	//private Vector3 startPos = Vector3.zero;
	public static Vector3 endPos = Vector3.zero;

	void Start() {
		ConvertNavMeshToGraph();

		levelGraph = GraphHelper.GenerateGraph0(navMeshNodes, navMeshEdges);
	}

	//Test
	void Update() {
		/*
		if (Input.GetMouseButtonDown(0)){ // if left button pressed...
			RaycastHit hit = new RaycastHit();
			Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit) && hit.transform.name == "Floor"){
				Debug.Log ("test");
				DrawNodeFromPosition(hit.point);
				startPos = hit.point;
			}
		}
		else if (Input.GetMouseButtonDown(1)){ // if left button pressed...
			RaycastHit hit = new RaycastHit();
			Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit) && hit.transform.name == "Floor"){
				DrawNodeFromPosition(hit.point);
				endPos = hit.point;
			}
		}
		else if (Input.GetKeyUp(KeyCode.X))
		{
			List<HierarchicalPathNode> path = HierarchicalAStar.FindPath(levelGraph, startPos, endPos);
			if (path != null)
			{
				for (int i = 0; i < path.Count - 1; i++)
				{
					Debug.DrawLine(path[i].Node._graphPos, path[i+1].Node._graphPos, Color.cyan, 99f);
				}
			}
			Debug.Log(path.Count);
		}*/
	}

	//Converts a scenes navmesh into a graph
	void ConvertNavMeshToGraph() {
		navMesh = new Mesh();

		NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

		navMesh.vertices = navMeshData.vertices;
		navMesh.triangles = navMeshData.indices;

		CreateNavMeshNodes();
		CreateNavMeshEdges();

//		ShowGraphInEditor();
	}

	//Creates nodes from the scenes navmesh
	private void CreateNavMeshNodes() {
		Vector3[] tmpVertices = new Vector3[3];
		for(int i = 0,j = 0; i < navMesh.triangles.Length; i++) {
			tmpVertices[j] = navMesh.vertices[navMesh.triangles[i]];
			j++;
			if (j > 2) {
				navMeshNodes.Add (new NavMeshNode(tmpVertices[0],tmpVertices[1],tmpVertices[2]));
				j = 0;
			}
		}
	}

	//Connects the nodes
	private void CreateNavMeshEdges() {
		for (int i = 0; i < (navMeshNodes.Count - 1); i++) {
			NavMeshNode tmpNode = navMeshNodes[i];
		
			for(int j = i+1; j < navMeshNodes.Count; j++) {
				int verticeMatchCount = 0;

				foreach (Vector3 v1 in tmpNode.Vertices) {
					foreach (Vector3 v2 in navMeshNodes[j].Vertices) {
						if(v1 == v2)
							verticeMatchCount++;
					}
				}

				//create edge if 2 vertices is shared
				if (verticeMatchCount == 2) {
					//edge is created in both directions, to enable one way routes
					navMeshEdges.Add(new NavMeshEdge(tmpNode, navMeshNodes[j]));
					navMeshEdges.Add(new NavMeshEdge(navMeshNodes[j], tmpNode));
				}
			}
		}
	}

	//test function to see if stuff works
	public void ShowGraphInEditor() {
		foreach (NavMeshEdge edge in navMeshEdges) {

			Debug.Log (edge.StartNode.Centroid + " to " + edge.EndNode.Centroid);
			Debug.DrawLine(edge.StartNode.Centroid, edge.EndNode.Centroid,Color.red, 20f, false);
		}
	}

	public void DrawNodeFromPosition(Vector3 pos) {
		HierarchicalNode node = levelGraph.GetNode(pos);
		if(node != null) {
			Debug.DrawLine(node.Vertices[0], node.Vertices[1], Color.white,99f, false);
			Debug.DrawLine(node.Vertices[1], node.Vertices[2], Color.white,99f, false);
			Debug.DrawLine(node.Vertices[2], node.Vertices[0],Color.white,99f, false);
		}
	}
	
}
