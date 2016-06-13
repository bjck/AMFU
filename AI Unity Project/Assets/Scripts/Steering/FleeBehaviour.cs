using UnityEngine;
using System.Collections;

public class FleeBehaviour{

	public static Vector3 GetDirectionVector(Vector3 playerPos, Vector3 seekPos)
	{
		return (playerPos - seekPos).normalized;
	}
}
