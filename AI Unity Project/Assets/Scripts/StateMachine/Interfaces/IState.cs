using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IState {
	void AddAction(IAction action);

	void AddEntryAction(IAction action);

	void AddExitAction(IAction action);

	void AddTransition(ITransition transition);

	List<ITransition> GetTransition();

	List<IAction> GetActionList();

	List<IAction> GetEntryActionList();

	List<IAction> GetExitActionList();
}
