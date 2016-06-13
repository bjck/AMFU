using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSMTurnBased 
{
	public FiniteStateMachine FSM;
	State 	flee, 
			defend, 
			attack, 
			idle;

	List<ITransition> 	idleTransitions, 
						attackTransitions, 
						defendTransition, 
						fleeTransition;

	public FSMTurnBased()
	{
		idleTransitions = new List<ITransition>();
		attackTransitions = new List<ITransition>();
		defendTransition = new List<ITransition>();

		#region Transitions
		//Idle Transitions
		idleTransitions.Add (new Transition(defend));
		idleTransitions.Add (new Transition(attack));
		idleTransitions.Add (new Transition(flee));

		//Attack Transitions
		attackTransitions.Add (new Transition(idle));
		attackTransitions.Add (new Transition(defend));
		attackTransitions.Add (new Transition(flee));

		//Defend Transitions
		defendTransition.Add (new Transition(idle));
		defendTransition.Add (new Transition(attack));
		defendTransition.Add (new Transition(flee));

		//idle
		defendTransition.Add (new Transition(defend));
		defendTransition.Add (new Transition(attack));
		defendTransition.Add (new Transition(flee));
		#endregion

		//Creating States
		idle = new State("default",idleTransitions);
		attack = new State("Attack",attackTransitions);
		defend = new State("Defend",defendTransition);
		flee = new State("Flee",fleeTransition);
		attack.AddAction(new AttackAction());
		flee.AddAction (new FleeAction());
		defend.AddAction(new DefendAction());
		idle.AddAction(new IdleAction());
		FSM = new FiniteStateMachine(idle);
	}
}