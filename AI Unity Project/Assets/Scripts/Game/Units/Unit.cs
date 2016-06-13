using UnityEngine;
using System.Collections;

public enum UnitActions
{
	Move,
	Attack,
	Guard,
	Heal
}

public class Unit : MonoBehaviour {

	public float CurrentHealth { get { return _currentHealth; } }

	[SerializeField]
	private Teams _team;
	public Teams Team { get { return _team; } }
	
	[SerializeField]
	private float _maxHealth = 100;
	public float _currentHealth;
	

	
	[SerializeField]
	private float _maxMoveLength = 25;
	private float _leftMoveLength;
	
	[SerializeField]
	private uint _maxMoves = 2;
	private uint _leftMoves;
	
	public uint LeftMoves { get { return _leftMoves; } }
	
	[SerializeField]
	private float _maxHealthPacks = 2;
	private float _leftHealthPacks;
	
	public float HealthPacks { get { return _leftHealthPacks; } }

	[SerializeField]
	private float _healthPackHealAmount = 1;
	
	[SerializeField]
	private float _attackRange = 25;
	[SerializeField]
	private float _attackDamage = 50;
	
	[SerializeField]
	private float _baseHitChance = 80;
	[SerializeField]
	private float _baseDodgeChance = 20;
	
	[SerializeField]
	private bool _alive = false;

	public bool Alive { get { return _alive; } }

	[SerializeField]
	private bool _active = false;
	[HideInInspector]
	public GameObject parent;

	public bool Active { get{ return _active; } 
		set { 
			if (!value) { _moveRangeEmitter.renderer.enabled = false; _attackRangeEmitter.renderer.enabled = false; }
			else if (_moving) { _moveRangeEmitter.renderer.enabled = true; }
			else if (_attacking) { _attackRangeEmitter.renderer.enabled = true; }
			_active = value; } }
	
	[SerializeField]
	private bool _moving = false;
	
	public bool Moving { get{ return _moving; } set { _moving = value; _moveRangeEmitter.renderer.enabled = (_active && _moving); } }
	
	private bool _attacking = false;
	
	public bool Attacking { get{ return _attacking; } set { _attacking = value; _attackRangeEmitter.renderer.enabled = (_active && _attacking); } }

	private bool _guarding = false;

	[SerializeField]
	private GameObject _moveRangeEmitter;
	[SerializeField]
	private GameObject _attackRangeEmitter;

	// Use this for initialization
	void Start () 
	{
		parent = this.gameObject;
		_currentHealth = _maxHealth;
		_leftHealthPacks = _maxHealthPacks;
		_alive = true;
		
		if (Team == Teams.Team1)
		{
			TurnManager.OnTeam1 += NewTurn;
		}
		if (Team == Teams.Team2)
		{
			TurnManager.OnTeam2 += NewTurn;
		}

		lastPos = this.transform.position;

		NewTurn();
		
		_moveRangeEmitter.transform.localScale = new Vector3(1, 0.1f, 1);
		_attackRangeEmitter.transform.localScale = new Vector3(1, 0.1f, 1);
	}

	Vector3 lastPos;
	// Update is called once per frame
	void Update () 
	{
		if (_currentHealth <= 0)
		{
			_currentHealth = 0;
			_alive = false;
			
			rigidbody.useGravity = true;
			rigidbody.constraints = RigidbodyConstraints.None;
			this.transform.Rotate(Vector3.forward, 1.5f);
			
			Steering.SetSteeringActive(this.transform, false);
			
			if (Team == Teams.Team1)
			{
				TurnManager.OnTeam1 -= NewTurn;
			}
			if (Team == Teams.Team2)
			{
				TurnManager.OnTeam2 -= NewTurn;
			}
		}
		if (Active)
		{
			_moveRangeEmitter.transform.position = this.transform.position;
			_attackRangeEmitter.transform.position = this.transform.position;

			if (_moving)
			{
				Steering.SetSteeringActive(this.transform, true);
				_leftMoveLength -= Vector3.Distance(this.transform.position, lastPos);
				lastPos = this.transform.position;

				_moveRangeEmitter.transform.localScale = new Vector3(_leftMoveLength * 2, 0.1f, _leftMoveLength * 2);

				if (_leftMoveLength <= 0 || Input.GetKeyDown(KeyCode.E))
				{
					Steering.MoveTo(this.transform, this.transform.position);
					_leftMoveLength = 0;

					_moving = false;
					Steering.SetSteeringActive(this.transform, false);
				}

				Steering.SteeringUpdate(this.transform);
			}
		}
	}

	public void NewTurn()
	{
		_leftMoves = _maxMoves;
		_moving = false;
		_attacking = false;
		_guarding = false;
	}

	public void UseTurn(UnitActions action, object value)
	{
		if (_leftMoves > 0 || _moving || _attacking)
		{
			//TODO Do an action
			switch (action)
			{
			case UnitActions.Move:
				if (!_moving)
				{
					Attacking = false;
					_leftMoveLength = _maxMoveLength;
					Moving = true;
				}
				_leftMoves--;
				break;
			case UnitActions.Attack:
				if (!_attacking)
				{
					Moving = false;
					Attacking = true;

					_attackRangeEmitter.transform.localScale = new Vector3(_attackRange * 2, 0.1f, _attackRange * 2);
				}
				_leftMoves--;
				break;
			case UnitActions.Heal:
				if (_leftHealthPacks > 0)
				{
					Heal();
					_leftHealthPacks--;
					_leftMoves--;
				}
				break;
			case UnitActions.Guard:
				_guarding = true;
				break;
			}
		}
		else
		{
			Debug.Log("Don't have any moves left");
			//Can't do anymore this turn.
		}
	}

	public void MoveTo(Vector3 toPos)
	{
		Steering.SetSteeringActive(this.transform, true);
		toPos.y = this.transform.position.y;
		if (Vector3.Distance(this.transform.position, toPos) < _leftMoveLength)
		{
			Steering.MoveTo(this.transform, toPos);
			Debug.Log("Moving");
		}
		else 
		{
			Vector3 newTo = (toPos - this.transform.position).normalized;
			newTo *= _leftMoveLength;
			
			Steering.MoveTo(this.transform, toPos);

			Debug.Log("Too long");
		}
	}

	public void Heal()
	{
		_currentHealth += _healthPackHealAmount;
		_currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
	}

	public bool Attack(Unit attackee)
	{
		//Melee or Attack
		if (Vector3.Distance(this.transform.position, attackee.transform.position) <= _attackRange)
		{
			this.collider.enabled = false;
			Ray collisionRay = new Ray(this.transform.position, (attackee.transform.position - this.transform.position).normalized);
			RaycastHit Rayhit = new RaycastHit();

			if (Physics.Raycast(collisionRay, out Rayhit, _attackRange) && Rayhit.collider == attackee.collider)
			{
				Attacking = false;
				if (UnityEngine.Random.Range(0, 100) < _baseHitChance)
				{
					return attackee.Hit(_attackDamage);
				}
				return false;
			}
			this.collider.enabled = true;
		}
		Debug.Log("Out of range");
		return false;
	}

	bool Hit(float damage)
	{
		if (UnityEngine.Random.Range(0, 100) > (_guarding ? _baseDodgeChance * 2 : _baseDodgeChance))
		{
			_currentHealth -= damage;
			if (_currentHealth <= 0)
			{
				_currentHealth = 0;
				_alive = false;
				StateTracker.WriteHistory (gameObject);

				rigidbody.useGravity = true;
				rigidbody.constraints = RigidbodyConstraints.None;
				this.transform.Rotate(Vector3.forward, 1.5f);
			
				Steering.SetSteeringActive(this.transform, false);

				if (Team == Teams.Team1)
				{
					TurnManager.OnTeam1 -= NewTurn;
				}
				if (Team == Teams.Team2)
				{
					TurnManager.OnTeam2 -= NewTurn;
				}
			}
			return true;
		}
		return false;
	}
}
