using UnityEngine;
using System.Collections;

public class Action : IAction {
	GameObject go = GameObject.Find("Warrior");
	Vector3 Direction = new Vector3();

	public void SetDirection(Vector3 dir) {
		Direction = dir;
	}
	public void act(GameObject actor)
	{
		go.transform.Translate(Direction * 3.0f * Time.deltaTime);
	}
}
