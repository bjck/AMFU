using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FiniteStateMachine 
{
	//TODO Need to know who is using this state machine somehow -_-
	private GameObject actor;
	public static List<State> States = new List<State>();


	public IState CurrentState;
	//private IState InitialState;
	private ITransition TriggeredTransition;
	private IState TargetState;
	private List<IAction> ActionList = new List<IAction>();

	public State GetState(string stateName)
	{
		foreach(State state in States)
		{
			if (state.StateName == stateName)
				return state;
		}
		return null;
	}

	public FiniteStateMachine(IState InitialState) 
	{
		CurrentState = InitialState;
	}

	public List<IAction> UpdateState() 
	{
		TriggeredTransition = null;
		foreach (ITransition t in CurrentState.GetTransition())
		{
			if (t.IsTriggered())
			{
				TriggeredTransition = t;
				Debug.Log("triggered");
				break;
			}
		}

		if (TriggeredTransition != null) 
		{
			TargetState = TriggeredTransition.GetTargetState();

			ActionList.AddRange(CurrentState.GetExitActionList());
			ActionList.AddRange(CurrentState.GetActionList());
			ActionList.AddRange(TargetState.GetEntryActionList());

			//Adding transition to the tracker
			StateTracker.AddTransition((State)CurrentState, (State)TargetState, actor);
			CurrentState = TargetState;

			return ActionList;
		}

		return CurrentState.GetActionList();
	}

}
