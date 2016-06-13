using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFollowBehaviour {
	
	public static Vector3 GetDirectionVector(TestingSteering playerSteering)
	{
		if (GridGenerator.level != null && GridGenerator._currentEnd != null)
		{
			Debug.Log("Finding path");
			List<Vector3> path = HierarchicalAStar.FindPath(GridGenerator.level, playerSteering.CurrentPosition - new Vector3(0, 1, 0), GridGenerator._currentEnd.transform.position);
			Debug.Log(path[1]);
			Vector3 nodePos = path[1];
			if (nodePos == Vector3.zero)
			{
				nodePos = path[2];
				if (nodePos == Vector3.zero)
					nodePos = path[3];
			}
			Vector3 seekPosition = new Vector3(nodePos.x, playerSteering.CurrentPosition.y, nodePos.y);
			return SeekBehaviour.GetDirectionVector(playerSteering.CurrentPosition, seekPosition);
		}
		return Vector3.zero;
	}
}
