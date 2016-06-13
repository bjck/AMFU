using UnityEngine;
using System.Collections;

public class SeekBehaviour{

	public static Vector3 GetDirectionVector(Vector3 playerPos, Vector3 seekPos)
	{
		return (seekPos - playerPos).normalized;
	}
}
