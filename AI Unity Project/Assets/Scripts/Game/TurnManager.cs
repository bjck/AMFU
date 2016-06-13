using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Teams
{
	Team1,
	Team2
}

public class TurnManager : MonoBehaviour 
{

	static TurnManager _turnManager;

	public static Teams CurrentTeam { get; private set; }
	
	public delegate void OnTeam1Start();
	public static event OnTeam1Start OnTeam1;

	public delegate void OnTeam2Start();
	public static event OnTeam2Start OnTeam2;

	static bool currentPlayerDone = false;
	float rotationPerSecond = 90;

	[SerializeField]
	public bool _useAI = true;

	public static bool UseAI { get { return _turnManager._useAI; } }

	public static TurnManager CurrentTurnManInstance {get{return _turnManager;}}
	private List<Unit> _teamAI;
	private Dictionary<Unit, State> _unitCurrentState;
	public FSMTurnBased UnitFSM;

	private bool GameOver;
#if _useAI == true
	[SerializeField]
#endif
	public Teams _teamUsingAI;

	public static Teams TeamUsingAI { get { return _turnManager._teamUsingAI; } }

	// Use this for initialization
	void Start () 
	{
		_turnManager = this.gameObject.GetComponent<TurnManager>();
		GameOver = false;
		GameData.FindAllActors();
		UnitFSM = new FSMTurnBased();
		CurrentTeam = Teams.Team1;
		_teamAI = new List<Unit>();
		_unitCurrentState = new Dictionary<Unit, State>();

		if (_useAI)
		{
			foreach (Object unit in GameObject.FindObjectsOfType(typeof(Unit)))
			{
				if ((unit as Unit).Team == TeamUsingAI)
				{
					_teamAI.Add((Unit)unit);

					//TODO Get the correct Default state
					//Tror det er rigtigt. Bliver oprettet i FiniteStateMachine
					_unitCurrentState.Add((Unit)unit, State.GetState("Default"));
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentPlayerDone)
		{
			if (_useAI && _teamUsingAI == CurrentTeam)
			{
				bool AIDone = true;
				foreach (Unit unit in _teamAI)
				{
					Steering.SteeringUpdate(unit.transform);
					if (!Steering.GetSteeringActive(unit.transform))
					{
						if (unit.LeftMoves > 0)
						{
							//Debug.Log("Previous: " + _unitCurrentState[unit].StateName);

							AIDone = false;
							/*
							if (_unitCurrentState[unit].GetExitActionList() != null)
							{
								//Der er ikke noget i den her liste smarte...

								foreach(IAction action in _unitCurrentState[unit].GetExitActionList())
								{
									action.act(unit.gameObject);
								}
							}*/

							State s = AIWrapper.GetDecision(unit.gameObject, ((State)UnitFSM.FSM.CurrentState));
							if(s != UnitFSM.FSM.CurrentState && s.StateName != "default")
							{
								StateTracker.AddTransition((State)UnitFSM.FSM.CurrentState, s, unit.gameObject);
								UnitFSM.FSM.CurrentState = s;
							}
							List<IAction> acti =((State)UnitFSM.FSM.CurrentState).GetActionList();
							foreach(IAction ac in acti)
							{
								ac.act(unit.gameObject);
							}

							//TODO Get action
							//DefendAction att = new DefendAction();
							unit.UseTurn(UnitActions.Move, null);
							//att.act(unit.gameObject);
						}
					}
					else
					{
						AIDone = false;
					}
					//TODO do AI stuff
				}
				if (AIDone)
					SwitchPlayer();
			}
			else if (CurrentTeam == Teams.Team1)
			{
				if (Camera.main.transform.rotation.eulerAngles.y < 10 || Camera.main.transform.rotation.eulerAngles.y > 350)
				{
					Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
					currentPlayerDone = false;
				}
				else
					Camera.main.transform.Rotate(new Vector3(0,0,-1) * rotationPerSecond * Time.deltaTime);
			}
			else if (CurrentTeam == Teams.Team2)
			{
				if (Camera.main.transform.rotation.eulerAngles.y < 190 && Camera.main.transform.rotation.eulerAngles.y > 170)
				{
					Camera.main.transform.eulerAngles = new Vector3(90, 180, 0);
					currentPlayerDone = false;
				}
				else
					Camera.main.transform.Rotate(new Vector3(0,0,1) * rotationPerSecond * Time.deltaTime);
			}
		}
		if (OnTeam1 == null && !GameOver)
		{
			GameOver = true;
			if(_teamUsingAI == Teams.Team1)
			{
				foreach(Unit u in _teamAI)
				{
					StateTracker.WriteHistory (u.parent);
				}
			}
			Debug.Log("Team 2 won");
		}
		else if (OnTeam2 == null && !GameOver)
		{
			GameOver = true;
			if(_teamUsingAI == Teams.Team2)
			{
				foreach(Unit u in _teamAI)
				{
					StateTracker.WriteHistory (u.parent);
				}
			}
			Debug.Log("Team 1 WON");
		}
			//rotate camera
	}

	public static void SwitchPlayer()
	{
		if (CurrentTeam == Teams.Team1)
		{
			CurrentTeam = Teams.Team2;
			OnTeam2();
		}
		else if (CurrentTeam == Teams.Team2)
		{
			CurrentTeam = Teams.Team1;
			OnTeam1();
		}
		currentPlayerDone = true;
	}
}
