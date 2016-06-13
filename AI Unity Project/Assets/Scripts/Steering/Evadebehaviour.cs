using UnityEngine;
using System.Collections;

public class Evadebehaviour {
	
	public static Vector3 GetDirectionVector(TestingSteering PlayerSteering, TestingSteering EvadeSteering)
	{
		Vector3 distance = EvadeSteering.CurrentPosition - PlayerSteering.CurrentPosition;
		float futureTime = distance.magnitude / PlayerSteering.MoveMaxSpeed;
		Vector3 futurePosition = EvadeSteering.CurrentPosition + EvadeSteering.CurrentVelocity * futureTime;
		return FleeBehaviour.GetDirectionVector(PlayerSteering.CurrentPosition, futurePosition);
	}
}
