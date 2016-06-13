using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttackAction : IAction {

	private FSMTeam myTeam;
	private FSMTeam enemyTeam;

	private List<GameObject> enemies = new List<GameObject>();
	private List<GameObject> sortedEnemiesByDist = new List<GameObject>();

	virtual public void act(GameObject actor) {
		GetEnemies();
		//Calculate direction of enemies. Not used, as can result in non-positions
		/*
		Vector3 enemyDirection = Vector3.zero;
		
		foreach(GameObject enemy in enemies)
		{
			Debug.Log(enemy.transform.position);
			enemyDirection += enemy.transform.position - actor.transform.position;
		}
		
		enemyDirection.Normalize();
		Steering.MoveTo(actor.transform, actor.transform.position + (enemyDirection * GameRules.MaxMovableDistance));
		 */

		Steering.MoveTo(actor.transform, enemies[0].transform.position);

		/*
		foreach (var enemy in sortedEnemiesByDist) {
			float distToTarget = Vector3.Distance(enemy.transform.position, actor.transform.position);

			if (distToTarget > GameRules.MaxAttackRange) {
				//TODO use A* algorithm
				float distToMove = distToTarget - GameRules.MaxAttackRange;


			} else if(IsValidTarget(actor, enemy)) {
				//TODO attack
			}
		}
		*/
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

	private void SortEnemiesByDistance(GameObject actor) {
		sortedEnemiesByDist = enemies.OrderBy(e => Vector3.Distance(actor.transform.position, e.transform.position)).ToList();
	}

	private bool IsValidTarget(GameObject actor, GameObject enemy) {
		return !(Physics.Linecast(actor.transform.position, enemy.transform.position));
	}
}
