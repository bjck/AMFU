using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefendAction : IAction {
	private FSMTeam myTeam;
	private FSMTeam enemyTeam;

	private List<GameObject> enemies = new List<GameObject>();
	private List<GameObject> sortedEnemiesByDist = new List<GameObject>();

	public void act(GameObject actor) {
		//Calculate direction of enemies. Not used, as can result in non-positions. Should find nearest edge

		Vector3 enemyDirection = Vector3.zero;
		
		foreach(GameObject enemy in enemies)
		{
			enemyDirection += enemy.transform.position - actor.transform.position;
		}
		
		enemyDirection.Normalize();
		
		Steering.MoveTo(actor.transform, actor.transform.position + (-enemyDirection * GameRules.MaxMovableDistance));

	}
	
	private void GetEnemies() {
		enemies = GameData.GetEnemies(myTeam);
	}

	private void FindCoverPoint() {
	}
}
