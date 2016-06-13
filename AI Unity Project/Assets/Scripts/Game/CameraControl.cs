using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	[SerializeField]
	private float _scrollSpeed = 3;

	// Update is called once per frame
	void Update () {
		{
			//Camera Zoom
			this.transform.position += new Vector3(0, -Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed, 0);
		}
		if (Input.GetMouseButton(2))
		{
			this.transform.position += new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y"));
		}
	}
}
