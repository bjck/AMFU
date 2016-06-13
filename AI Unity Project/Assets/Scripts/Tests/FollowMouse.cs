using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		this.transform.position = ray.origin + (ray.direction * 17.0f);
		this.transform.position -= new Vector3(0, this.transform.position.y, 0);
	}
}
