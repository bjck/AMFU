using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestingSteering : MonoBehaviour {
	#region Properties
	public Behaviours CurrentBehavior { get { return _currentBehaviour; } }
	public Vector3 CurrentVelocity { get { return _currentVelocity; } }
	public Vector3 CurrentPosition { get { return transform.position; } }
	public float MoveMaxSpeed { get { return _moveMaxSpeed; } }
	#endregion

	#region Variables
	[SerializeField]
	private Transform _followTransform;
	[SerializeField]
	private TestingSteering _followSteering;
	[SerializeField]
	private Behaviours _currentBehaviour = Behaviours.Evade;

	private bool _steeringActive = true;

	public bool SteeringActive { get { return _steeringActive; } set { _steeringActive = value; if (!_steeringActive) { _currentToPos = CurrentPosition; _currentEndPos = CurrentPosition; _currentVelocity = Vector3.zero; } } }

	[SerializeField]
	private float _moveMaxSpeed = 5.0f;
	private float _moveAcceleration = 5.0f;

	private float _slowingDistance = 5.0f;
	
	[SerializeField]
	private float _maxSeeAhead = 2.0f;
	[SerializeField]
	private float _maxAvoidanceForce = 500.0f;

	private Vector3 _formationOffset = Vector3.zero;
	
	private Vector3 _currentVelocity = Vector3.zero;
	
	private float _seperationDistance = 1.0f;
	private float _maxSeperationDistance = 3.0f;
	
	private List<TestingSteering> _otherUnits;
	#endregion

	#region Unity functions
	// Use this for initialization
	void Start () {
		_currentVelocity = Vector3.zero;

		if (_followSteering == null && _followTransform != null)
		{
			_followSteering = _followTransform.GetComponent<TestingSteering>();
			//_formationOffset = _followSteering.CurrentPosition - this.CurrentPosition;
			_formationOffset = this.CurrentPosition - _followSteering.CurrentPosition;
		}
		if (_otherUnits == null)
		{
			_otherUnits = new List<TestingSteering>(GameObject.FindObjectsOfType<TestingSteering>());
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (SteeringActive)
		{
			if ((_followTransform == null || _followSteering == null) && _currentBehaviour != Behaviours.PathFinding)
				return;

			_currentVelocity = GetVelocity();
			_currentVelocity.Scale(new Vector3(1,0,1));

			this.transform.position += _currentVelocity * Time.deltaTime;
			this.transform.LookAt(CurrentPosition + CurrentVelocity);
		}
	}

	Vector3 _currentToPos;
	public void MoveTo(Vector3 toPos)
	{
		SteeringActive = true;

		_currentBehaviour = Behaviours.PathFinding;

		_currentToPos = toPos;
	}
	#endregion

	#region Moving functions
	private Vector3 GetDirection()
	{
		switch (_currentBehaviour)
		{
		case Behaviours.Pursuit:
			return GetPursuitDirection(this, _followSteering);
		case Behaviours.Evade:
			return GetEvadeDirection(this, _followSteering);
		case Behaviours.Follow:
			return GetFollowDirection(this, _followSteering);
		case Behaviours.Formation:
			return GetFormationDirection(this, _followSteering, _formationOffset);
		case Behaviours.PathFinding:
			return GetPathDirection(this, _currentToPos, NavMeshHelper.levelGraph);
		default:
			return Vector3.zero;
		}
	}

	Vector3 GetVelocity()
	{
		Vector3 desiredVelocity = GetDirection();
		desiredVelocity += Seperation(this);

		if (desiredVelocity.magnitude < _slowingDistance)
		{
			desiredVelocity.Normalize();
			desiredVelocity *= _moveMaxSpeed;
			if (CurrentBehavior == Behaviours.PathFinding)
				desiredVelocity *= (Vector3.Distance(CurrentPosition, nextPos) / _slowingDistance);
			else
				desiredVelocity *= (Vector3.Distance(CurrentPosition, _followTransform.position) / _slowingDistance);
		}
		else
		{
			desiredVelocity.Normalize();
			desiredVelocity *= _moveMaxSpeed;
		}


		Vector3 steering = desiredVelocity - _currentVelocity;
		steering += CollisionCheck();

		steering = steering.Truncate(_moveAcceleration * Time.deltaTime);

		Vector3 newVelocity = (_currentVelocity + steering).Truncate(_moveMaxSpeed);
		return newVelocity;
	}
	#endregion

	#region Move helper functions
	[SerializeField]
	private float _collisionWidth = 1f;
	private Vector3 CollisionCheck()
	{
		_maxSeeAhead = (CurrentVelocity.magnitude * 10f) / _moveMaxSpeed;
		
		Vector3 ahead = CurrentPosition + CurrentVelocity.normalized * _maxSeeAhead;

		this.collider.enabled = false;
		Ray collisionRay = new Ray(CurrentPosition, CurrentVelocity.normalized);
		RaycastHit Rayhit = new RaycastHit();
		Physics.SphereCast(collisionRay, _collisionWidth, out Rayhit, _maxSeeAhead);
		this.collider.enabled = true;

		Debug.DrawRay(CurrentPosition, CurrentVelocity.normalized * _maxSeeAhead, Color.black);
		
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
	
	private Vector3 Seperation(TestingSteering playerSteering)
	{
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
	#endregion

	#region Steering functions
	private Vector3 GetSeekDirection(Vector3 playerPos, Vector3 seekPos)
	{
		return (seekPos - playerPos).normalized;
	}

	private Vector3 GetPursuitDirection(TestingSteering PlayerSteering, TestingSteering PursuitSteering)
	{
		Vector3 distance = PursuitSteering.CurrentPosition - PlayerSteering.CurrentPosition;
		float futureTime = distance.magnitude / PlayerSteering.MoveMaxSpeed;
		Vector3 futurePosition = PursuitSteering.CurrentPosition + PursuitSteering.CurrentVelocity * futureTime;
		return SeekBehaviour.GetDirectionVector(PlayerSteering.CurrentPosition, futurePosition);
	}

	private Vector3 GetFleeDirection(Vector3 playerPos, Vector3 seekPos)
	{
		return (playerPos - seekPos).normalized;
	}

	private Vector3 GetEvadeDirection(TestingSteering PlayerSteering, TestingSteering EvadeSteering)
	{
		Vector3 distance = EvadeSteering.CurrentPosition - PlayerSteering.CurrentPosition;
		float futureTime = distance.magnitude / PlayerSteering.MoveMaxSpeed;
		Vector3 futurePosition = EvadeSteering.CurrentPosition + EvadeSteering.CurrentVelocity * futureTime;
		return FleeBehaviour.GetDirectionVector(PlayerSteering.CurrentPosition, futurePosition);
	}

	float _followDistance = 2.0f;
	private Vector3 GetFollowDirection(TestingSteering playerSteering, TestingSteering leaderSteering)
	{
		Vector3 tv = leaderSteering.CurrentVelocity * -1;
		tv = tv.normalized * _followDistance;
		Vector3 followPosition =  leaderSteering.CurrentPosition + tv;
		
		Vector3 force = SeekBehaviour.GetDirectionVector(playerSteering.CurrentPosition, followPosition);
		//force += Seperation(playerSteering);
		
		return force;
	}

	public static Vector3 GetFormationDirection(TestingSteering playerSteering, TestingSteering leaderSteering, Vector3 positionOffset)
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

	List<Vector3> _currentPath;
	int _currentPathIndex = 0;
	Vector3 _currentEndPos;
	[SerializeField]
	float _distanceToNext = 2f;
	Vector3 nextPos;
	private Vector3 GetPathDirection(TestingSteering playerSteering, Vector3 endPos, Graph graph)
	{
		//Should have a to Vector3 instead of using gridgenerator

		//Should maybe remember index of path, and only search again if end position changes
		//If not path is found, return old direction

		//Check if there is a direct path
		//Smooth path

		//Break pathfinding. Start from new, or continue on current path
		if (_currentEndPos != endPos)
		{
			if (graph != null)
			{
				_currentEndPos = endPos;
				_currentPath = HierarchicalAStar.FindPath(graph, this.CurrentPosition, _currentEndPos);
				_currentPathIndex = 0;	
			}
		}
		if (_currentPath != null && _currentPath.Count > 1 && _currentPathIndex < _currentPath.Count)
		{
			
			nextPos = _currentPath[_currentPathIndex];
			nextPos.y = this.CurrentPosition.y;
			if (Vector3.Distance(CurrentPosition, nextPos) < _distanceToNext)
			{
				if (_currentPathIndex < _currentPath.Count - 1)
				{
					_currentPathIndex++;
					nextPos = _currentPath[_currentPathIndex];
				}
				else
				{
					nextPos = endPos;
				}
				nextPos.y = this.CurrentPosition.y;
			}
			return GetSeekDirection(this.CurrentPosition, nextPos);
		}
		return CurrentVelocity.normalized;
	}
	#endregion
}
