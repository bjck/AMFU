using UnityEngine;
using System.Collections;

public class PursuitBehaviour : MonoBehaviour {
	
	public static Vector3 GetDirectionVector(TestingSteering PlayerSteering, TestingSteering PursuitSteering)
	{
		Vector3 distance = PursuitSteering.CurrentPosition - PlayerSteering.CurrentPosition;
		float futureTime = distance.magnitude / PlayerSteering.MoveMaxSpeed;
		Vector3 futurePosition = PursuitSteering.CurrentPosition + PursuitSteering.CurrentVelocity * futureTime;
		return SeekBehaviour.GetDirectionVector(PlayerSteering.CurrentPosition, futurePosition);
	}
}
