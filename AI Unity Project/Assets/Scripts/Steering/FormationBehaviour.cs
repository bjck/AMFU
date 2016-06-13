using UnityEngine;
using System.Collections;

public class FormationBehaviour{
	
	public static Vector3 GetDirectionVector(TestingSteering playerSteering, TestingSteering leaderSteering, Vector3 positionOffset)
	{
		Vector3 leadRot = leaderSteering.transform.rotation.eulerAngles;
		float rot = leadRot.y * Mathf.Deg2Rad;

		float cs = Mathf.Cos(rot);
		float sn = Mathf.Sin(rot);
		
		float newX = ((cs * positionOffset.x) - (sn * positionOffset.x));
		float newZ = ((sn * positionOffset.z) + (cs * positionOffset.z));

		Vector3 tmpPos = new Vector3(newX, 0, newZ);
		tmpPos += leaderSteering.CurrentPosition;

		//Vector3 tmpPos = leaderSteering.CurrentPosition + positionOffset;
		return SeekBehaviour.GetDirectionVector(playerSteering.CurrentPosition, tmpPos);
	}
}
