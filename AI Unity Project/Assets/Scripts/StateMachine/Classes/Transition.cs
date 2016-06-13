using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Transition : ITransition {
	private IState TargetState;
	private List<IAction> ActionList = new List<IAction>();
	private List<ICondition> ConditionList = new List<ICondition>();

	public Transition(){}

	public Transition(IState target)
	{
		TargetState = target;
	}

	public void SetTargetState(IState target) 
	{
		TargetState = target;
	}

	public IState GetTargetState() 
	{
		return TargetState;
	}

	public void AddCondition(ICondition condition) 
	{
		ConditionList.Add(condition);
	}

	public List<IAction> GetActionList() 
	{
		return ActionList;
	}
	
	public bool IsTriggered() 
	{
		foreach(ICondition c in ConditionList) 
		{
			if(!c.Test()) return false;
		}
		if (ConditionList.Count == 0) 
		{
			return false;
		}

		return true;
	}
}
