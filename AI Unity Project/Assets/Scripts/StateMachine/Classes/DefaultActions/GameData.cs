using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FSMTeam
{
	Team1,
	Team2
}

public static class GameData {
	private static List<GameObject> Team1 = new List<GameObject>();
	private static List<GameObject> Team2 = new List<GameObject>();

	public static void FindTeam1Actors() {
		Team1.AddRange(GameObject.FindGameObjectsWithTag("Team1"));
	}

	public static void FindTeam2Actors() {
		Team1.AddRange(GameObject.FindGameObjectsWithTag("Team2"));
	}

	public static void FindAllActors() {
		Team1.AddRange(GameObject.FindGameObjectsWithTag("Team1"));
		Team1.AddRange(GameObject.FindGameObjectsWithTag("Team2"));
	}

	//returns the visible enemies of the "team" parameter
	public static List<GameObject> GetVisibleEnemies(FSMTeam team) {
		List<GameObject> visibleEnemies = new List<GameObject>();

		for (int i = 0; i < Team1.Count; i++) {
			for (int j = 0; j < Team2.Count; i++) {
				Vector3 player1Pos = Team1[i].transform.position;
				Vector3 player2Pos = Team2[j].transform.position;
				if(!(Physics.Linecast(player1Pos, player2Pos))) {
					//Makes sure no duplicates are added as well as the enemies are within the fog of war
					if(Vector3.Distance(player1Pos, player2Pos) <= GameRules.FogRadius) {
						if (team == FSMTeam.Team1 && !visibleEnemies.Contains(Team2[j])) {
							visibleEnemies.Add(Team2[j]);
						} else if (team == FSMTeam.Team2 && !visibleEnemies.Contains(Team1[i])) {
							visibleEnemies.Add(Team1[i]);
						}
					}
				}
			}
		}

		return visibleEnemies;
	}

	public static List<GameObject> GetEnemies(FSMTeam team) {
		return team == FSMTeam.Team1 ? Team1 : Team2;
	}

	public static List<GameObject> GetTeam1Actors() {
		return Team1;
	}
	
	public static List<GameObject> GetTeam2Actors() {
		return Team2;
	}
}
