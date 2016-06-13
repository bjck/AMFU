using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Behaviours
{
	Pursuit,
	Evade,
	Follow,
	Formation,
	PathFinding
}

public static class VectorExtension
{
	public static Vector3 Truncate(this Vector3 vector, float maxSpeed)
	{
		float i = maxSpeed / vector.magnitude;
		i = i < 1.0f ? i : 1.0f;
		
		return vector * i;
	}
}

public class Steering : MonoBehaviour {
	#region Properties
	public static Behaviours CurrentBehavior (Transform transform) { return _currentBehaviour[transform]; }
	public static Vector3 CurrentVelocity (Transform transform) { return _currentVelocity[transform]; }
	public static float MoveMaxSpeed { get { return _moveMaxSpeed; } }
	#endregion

	#region Variables
	private static Dictionary<Transform, Behaviours> _currentBehaviour = new Dictionary<Transform, Behaviours>();

	private static Dictionary<Transform, bool> _steeringActive = new Dictionary<Transform, bool>();
	
	public static bool GetSteeringActive (Transform transform) 
	{ 
		if (!_steeringActive.ContainsKey(transform))
		{
			NewUnit(transform);
		}
		return _steeringActive[transform]; }
	public static void SetSteeringActive (Transform transform, bool value) 
	{
		if (!_steeringActive.ContainsKey(transform))
		{
			NewUnit(transform);
		}

		_steeringActive[transform] = value; 

		if (!value) 
		{
			_currentToPos[transform] = transform.position;
			_currentEndPos[transform] = transform.position; 
			_currentVelocity[transform] = Vector3.zero; 
		} 
	} 

	private static float _moveMaxSpeed = 5.0f;
	private static float _moveAcceleration = 5.0f;

	private static float _slowingDistance = 5.0f;

	private static float _maxSeeAhead = 2.0f;
	private static float _maxAvoidanceForce = 500.0f;

	private static Dictionary<Transform, Vector3> _formationOffsets = new Dictionary<Transform, Vector3>();
	
	private static Dictionary<Transform, Vector3> _currentVelocity = new Dictionary<Transform, Vector3>();
	
	private static float _seperationDistance = 1.0f;
	private static float _maxSeperationDistance = 3.0f;
	
	private static List<Transform> _otherUnits = new List<Transform>();
	
	static Dictionary <Transform, Vector3> _currentToPos = new Dictionary<Transform, Vector3>();
	
	static Dictionary <Transform, List<Vector3>> _currentPath = new Dictionary<Transform, List<Vector3>>();
	static Dictionary <Transform, int> _currentPathIndex = new Dictionary<Transform, int>();
	static Dictionary <Transform, Vector3> _currentEndPos = new Dictionary<Transform, Vector3>();
	static float _distanceToNext = 2f;
	static Dictionary <Transform, Vector3> nextPos = new Dictionary<Transform, Vector3>();
	#endregion

	#region Unity functions
	// Use this for initialization
	private static void NewUnit (Transform transform) {
		Debug.Log("New Unit");

		_steeringActive.Add(transform, false);
		_currentBehaviour.Add(transform, Behaviours.PathFinding);

		_currentVelocity.Add(transform, Vector3.zero);
		_currentToPos.Add(transform, transform.position);
		_currentEndPos.Add(transform, transform.position); 

		_otherUnits.Add(transform);

		_currentPath.Add(transform, null);
		_currentPathIndex.Add(transform, 0);
		nextPos.Add(transform, transform.position);

		//_formationOffsets[transform] = transform.position - _followSteering.CurrentPosition;
		/*
		if (_otherUnits == null)
		{
			_otherUnits = new List<TestingSteering>(GameObject.FindObjectsOfType<TestingSteering>());
		}*/
	}
	
	// Update is called once per frame
	public static void SteeringUpdate (Transform transform) 
	{
		if (GetSteeringActive(transform))
		{
			if (_currentBehaviour[transform] != Behaviours.PathFinding)
				return;

			_currentVelocity[transform] = GetVelocity(transform);

			Vector3 tmpVec = _currentVelocity[transform];
			tmpVec.y = 0;
			_currentVelocity[transform] = tmpVec;

			transform.position += _currentVelocity[transform] * Time.deltaTime;
			transform.LookAt(transform.position + CurrentVelocity(transform));


			_leftMoveLength -= (_currentVelocity[transform] * Time.deltaTime).magnitude;
			
			if (_leftMoveLength <= 0)
			{
				_leftMoveLength = 0;

				Steering.SetSteeringActive(transform, false);
			}
		}
	}

	static float _leftMoveLength = 0.0f;
	public static void MoveTo(Transform transform, Vector3 toPos, bool useNavMesh = false)
	{
		if (useNavMesh)
		{
			if (NavMeshHelper.levelGraph.GetNode(toPos) == null)
			{
				Debug.Log("Out of bounds end pos: " + toPos);
				//SetSteeringActive(transform, false);

				NavMeshHit hit;
				
				//Find nearest edge on navmesh, and use that instead.
				NavMesh.FindClosestEdge(toPos, out hit, -1);

				Debug.Log(toPos + " -> " + hit.position);
				toPos = hit.position;
				toPos.y = 0;

				Debug.Log(NavMeshHelper.levelGraph.GetNode(toPos));
			}
		}
		else
		{
			if (GridManager.LevelGraph.GetNode(toPos) == null)
			{
				//TODO Calculate nearest or do it in GetNode
			}
			else
			{
				toPos = GridManager.LevelGraph.GetNode(toPos)._graphPos;
			}
		}

		//TODO Use movelength as nr of tiles moveable

		_leftMoveLength = GameRules.MaxMovableDistance;
		SetSteeringActive(transform, true);

		_currentBehaviour[transform] = Behaviours.PathFinding;

		toPos.y = transform.position.y;

		_currentToPos[transform] = toPos;
	}
	#endregion

	#region Moving functions
	private static Vector3 GetDirection(Transform transform)
	{
		switch (_currentBehaviour[transform])
		{/*
		case Behaviours.Pursuit:
			return GetPursuitDirection(transform, _followSteering);
		case Behaviours.Evade:
			return GetEvadeDirection(transform, _followSteering);
		case Behaviours.Follow:
			return GetFollowDirection(transform, _followSteering);
		case Behaviours.Formation:
			return GetFormationDirection(transform, _followSteering, _formationOffsets[transform]);*/
		case Behaviours.PathFinding:
			return GetPathDirection(transform, _currentToPos[transform], GraphHelper.CurrentGraph);
		default:
			return Vector3.zero;
		}
	}

	static Vector3 GetVelocity(Transform transform)
	{
		Vector3 desiredDirection = GetDirection(transform);
		desiredDirection += Seperation(transform);
		desiredDirection += CollisionCheck(transform);

		if (desiredDirection.magnitude < _slowingDistance)
		{
			desiredDirection.Normalize();
			desiredDirection *= _moveMaxSpeed;
			desiredDirection *= (Vector3.Distance(transform.position, nextPos[transform]) / _slowingDistance);
		}
		else
		{
			desiredDirection.Normalize();
			desiredDirection *= _moveMaxSpeed;
		}


		Vector3 steering = desiredDirection - _currentVelocity[transform];

		steering = steering.Truncate(_moveAcceleration * Time.deltaTime);

		Vector3 newVelocity = (_currentVelocity[transform] + steering).Truncate(_moveMaxSpeed);
		return newVelocity;
	}
	#endregion

	#region Move helper functions
	private static float _collisionWidth = 1f;
	private static Vector3 CollisionCheck(Transform transform)
	{
		_maxSeeAhead = (CurrentVelocity(transform).magnitude * 10f) / _moveMaxSpeed;
		
		Vector3 ahead = transform.position + CurrentVelocity(transform).normalized * _maxSeeAhead;

		transform.collider.enabled = false;
		Ray collisionRay = new Ray(transform.position, CurrentVelocity(transform).normalized);
		RaycastHit Rayhit = new RaycastHit();
		Physics.SphereCast(collisionRay, _collisionWidth, out Rayhit, _maxSeeAhead);
		transform.collider.enabled = true;

		Debug.DrawRay(transform.position, CurrentVelocity(transform).normalized * _maxSeeAhead, Color.black);
		
		if (Rayhit.collider != null)
		{
			Vector3 avoidanceForce = ahead - Rayhit.transform.position;
			avoidanceForce = avoidanceForce.normalized * _maxAvoidanceForce;
			
			return avoidanceForce;
		}
		else
		{
			return Vector3.zero;
		}
	}
	
	private static Vector3 Seperation(Transform transform)
	{
		Vector3 force = new Vector3();
		int neighborCount = 0;
		
		foreach(Transform unit in _otherUnits)
		{
			if (unit != transform && 
			    Vector3.Distance(unit.position, transform.position) <= _seperationDistance)
			{
				force += unit.position - transform.position;
				
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
	#endregion

	#region Steering functions
	private static Vector3 GetSeekDirection(Vector3 playerPos, Vector3 seekPos)
	{
		return (seekPos - playerPos).normalized;
	}

	private static Vector3 GetPursuitDirection(Transform transform, Transform pursuitTransform)
	{
		Vector3 distance = pursuitTransform.position - transform.position;
		float futureTime = distance.magnitude / MoveMaxSpeed;
		Vector3 futurePosition = pursuitTransform.position + CurrentVelocity(pursuitTransform) * futureTime;
		return GetSeekDirection(transform.position, futurePosition);
	}

	private static Vector3 GetFleeDirection(Vector3 playerPos, Vector3 seekPos)
	{
		return (playerPos - seekPos).normalized;
	}

	private static Vector3 GetEvadeDirection(Transform transform, Transform evadeTransform)
	{
		Vector3 distance = evadeTransform.position - transform.position;
		float futureTime = distance.magnitude / MoveMaxSpeed;
		Vector3 futurePosition = evadeTransform.position + CurrentVelocity(evadeTransform) * futureTime;
		return GetFleeDirection(transform.position, futurePosition);
	}

	static float _followDistance = 2.0f;
	private static Vector3 GetFollowDirection(Transform transform, Transform leaderTransform)
	{
		Vector3 tv = CurrentVelocity(leaderTransform) * -1;
		tv = tv.normalized * _followDistance;
		Vector3 followPosition =  leaderTransform.position + tv;
		
		Vector3 force = GetSeekDirection(transform.position, followPosition);
		//force += Seperation(playerSteering);
		
		return force;
	}
	/*
	public static Vector3 GetFormationDirection(Transform transform, TestingSteering leaderSteering, Vector3 positionOffset)
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
	}*/
	private static Vector3 GetPathDirection(Transform transform, Vector3 endPos, Graph graph)
	{
		if (_currentEndPos[transform] != endPos)
		{
			if (graph != null)
			{
				_currentEndPos[transform] = endPos;
				_currentPath[transform] = HierarchicalAStar.FindPath(graph, transform.position, _currentEndPos[transform]);
				_currentPathIndex[transform] = 0;
			}
		}
		if (_currentPath[transform] != null && _currentPath[transform].Count > 1 && _currentPathIndex[transform] < _currentPath[transform].Count)
		{
			
			nextPos[transform] = _currentPath[transform][_currentPathIndex[transform]];
			Vector3 tmp = nextPos[transform];
			tmp.y = transform.position.y;
			nextPos[transform] = tmp;
			if (Vector3.Distance(transform.position, nextPos[transform]) < _distanceToNext)
			{
				if (_currentPathIndex[transform] < _currentPath[transform].Count - 1)
				{
					_currentPathIndex[transform]++;
					nextPos[transform] = _currentPath[transform][_currentPathIndex[transform]];
				}/*
				else
				{
					nextPos[transform] = endPos;
				}*/
				tmp = nextPos[transform];
				tmp.y = transform.position.y;
				nextPos[transform] = tmp;
			}
			return GetSeekDirection(transform.position, nextPos[transform]);
		}
		return CurrentVelocity(transform).normalized;
	}
	#endregion
}
