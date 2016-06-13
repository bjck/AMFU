using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderFollow {

	static List<TestingSteering> _otherUnits;
	
	static float _followDistance = 2.0f;
	static float _seperationDistance = 1.0f;
	static float _maxSeperationDistance = 3.0f;

	static void Init()
	{
		if (_otherUnits == null)
		{
			_otherUnits = new List<TestingSteering>(GameObject.FindObjectsOfType<TestingSteering>());
		}
	}
	public static Vector3 GetDirectionVector(TestingSteering playerSteering, TestingSteering leaderSteering)
	{
		Vector3 followPosition = GetFollowPosition(leaderSteering);

		Vector3 force = SeekBehaviour.GetDirectionVector(playerSteering.CurrentPosition, followPosition);
		//force += Seperation(playerSteering);

		return force;
	}

	private static Vector3 GetFollowPosition(TestingSteering leaderSteering)
	{
		Vector3 tv = leaderSteering.CurrentVelocity * -1;
		tv = tv.normalized * _followDistance;
		return leaderSteering.CurrentPosition + tv;
	}

	private static Vector3 Seperation(TestingSteering playerSteering)
	{
		//Init();

		Vector3 force = new Vector3();
		int neighborCount = 0;

		foreach(TestingSteering unit in _otherUnits)
		{
			if (unit != playerSteering && 
			    Vector3.Distance(unit.CurrentPosition, playerSteering.CurrentPosition) <= _seperationDistance)
			{
				force += unit.CurrentPosition - playerSteering.CurrentPosition;
				
				neighborCount++;
			}
		}
		
		if (neighborCount != 0)
		{
			force /= neighborCount;
			
			force *= -1;
		}
		
		force.Normalize();
		force *= _maxSeperationDistance;

		return force;
	}
}
