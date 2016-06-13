using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
[System.Serializable]
public class State : IState 
{
	public static List<State> States = new List<State>();

	public static State GetState(string stateName)
	{
		foreach(State state in States)
		{
			if (state.StateName == stateName)
				return state;
		}
		return null;
	}

	private List<IAction> ActionList = new List<IAction>();
	private List<ITransition> TransitionList = new List<ITransition>();
	private List<IAction> EntryActionList = new List<IAction>();
	private List<IAction> ExitActionList = new List<IAction>();

	public string StateName{get; set;}

	public State(string n): this(n, null)
	{
		//TODO DO STUFF HERE... Not sure what ... :D
	}

	public State(Transition transition)
	{
		TransitionList.Add(transition);
	}

	public State(string n, List<ITransition> transitions): this(n, transitions, null){}
	
	public State(string n, List<ITransition> transitions, List<IAction> actions)
	{
		StateName = n;
		TransitionList = transitions;
		ActionList = actions;
		States.Add (this);
		FiniteStateMachine.States.Add(this);
	}

	public void AddEntryAction(IAction action) 
	{
		EntryActionList.Add(action);
	}
	
	public void AddExitAction(IAction action) 
	{
		ExitActionList.Add(action);
	}
	
	public void AddAction(IAction action) 
	{
		if(ActionList == null)
		{
			ActionList = new List<IAction>(); 
		}
		ActionList.Add(action);
	}
	
	public void AddTransition(ITransition transition) 
	{
		TransitionList.Add(transition);
	}
	
	public List<ITransition> GetTransition() 
	{
		return TransitionList;
	}
	
	public List<IAction> GetActionList() 
	{
		return ActionList;
	}
	
	public List<IAction> GetEntryActionList() 
	{
		return EntryActionList;
	}
	
	public List<IAction> GetExitActionList() 
	{
		return ExitActionList;
	}

	~State()
	{
		States.Remove (this);
	}
	 
}
