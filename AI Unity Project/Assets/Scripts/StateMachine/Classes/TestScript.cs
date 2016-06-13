using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestScript : MonoBehaviour {
	State Attack;
	State Defend;
	FiniteStateMachine FSM;
	MaxCondition HPAttack;
	MinCondition HPDefend;
	Transition SwitchStanceAttack;
	Transition SwitchStanceDefend;
	int PlayerHp = 100;
	int MaxHp = 100;
	Action MoveForward;
	Action MoveBackward;
	List<IAction> ActionList = new List<IAction>();

	// Use this for initialization
	void Start () 
	{
		HPAttack = new MaxCondition();
		HPDefend = new MinCondition();

		MoveForward = new Action();
		MoveBackward = new Action();

		Attack.AddAction(MoveForward);
		Defend.AddAction(MoveBackward);

		MoveForward.SetDirection(-Vector3.right);
		MoveBackward.SetDirection(Vector3.right);

		SwitchStanceDefend = new Transition();
		SwitchStanceAttack = new Transition();

		HPAttack.SetTestValue(PlayerHp, MaxHp/2);
		HPDefend.SetTestValue(PlayerHp, MaxHp/2);

		SwitchStanceDefend.AddCondition(HPDefend);
		SwitchStanceDefend.SetTargetState(Attack);

		SwitchStanceAttack.AddCondition(HPAttack);
		SwitchStanceAttack.SetTargetState(Defend);
		Attack = new State(SwitchStanceAttack);
		Defend = new State(SwitchStanceDefend);

		FSM = new FiniteStateMachine(Attack);
	
	}
	
	// Update is called once per frame
	void Update () {
		if (FSM.CurrentState == Attack) {
			Debug.Log("Attack stance active");
		} else if (FSM.CurrentState == Defend) {
			Debug.Log ("Defend stance active");
		}

		if (Input.GetMouseButton(0)) {
			PlayerHp -= 80;
			HPAttack.SetTestValue(PlayerHp, MaxHp/2);
			HPDefend.SetTestValue(PlayerHp, MaxHp/2);
		} else if(Input.GetMouseButton(1)) {
			PlayerHp += 80;
			HPAttack.SetTestValue(PlayerHp, MaxHp/2);
			HPDefend.SetTestValue(PlayerHp, MaxHp/2);
		}

		ActionList = FSM.UpdateState();

		foreach(IAction a in ActionList)
			a.act(this.gameObject);
	}
}
