using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITransition {

	IState GetTargetState();

	List<IAction> GetActionList();

	bool IsTriggered();
}
