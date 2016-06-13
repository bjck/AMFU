using UnityEngine;
using System.Collections;

public class NavMeshNode {
	public Vector3[] Vertices;
	public Vector3 Centroid;

	public NavMeshNode(Vector3 v1, Vector3 v2, Vector3 v3) {
		Vertices = new Vector3[]{v1, v2, v3};

		float centroidX = (v1.x + v2.x + v3.x) / 3;
		float centroidY = (v1.y + v2.y + v3.y) / 3;
		float centroidZ = (v1.z + v2.z + v3.z) / 3;

		Centroid = new Vector3(centroidX, centroidY, centroidZ);
	}
}
