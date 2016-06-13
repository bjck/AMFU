using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleeAction : IAction {
	private FSMTeam myTeam;
	private FSMTeam enemyTeam;

	private List<GameObject> enemies = new List<GameObject>();
	private List<GameObject> sortedEnemiesByDist = new List<GameObject>();
	private GameObject NicolaisCodeVirkerIkke; 

	public void act(GameObject actor) 
	{
		NicolaisCodeVirkerIkke = GameObject.Find("RUNAWAY");
		//Calculate direction of enemies. Not used, as can result in non-positions. Should find nearest edge
		Steering.MoveTo(actor.transform, NicolaisCodeVirkerIkke.transform.position);

	}

	private void GetActorsTeam() {
		//TODO get the actors team 
	}
	
	private void GetActor() {
		//TODO get actor
	}
	
	private void GetEnemies() {
		enemies = GameData.GetEnemies(myTeam);
	}

	private void FindPointAwayFromEnemies() {
	}
}
