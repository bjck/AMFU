using UnityEngine;
using System.Collections;
using System;
[Serializable]
public class testSletMig : MonoBehaviour {
	[SerializeField]
	public int a = 10;
	public int b = 10;
	public int c = 10;
	public int d = 10;
	public int e = 10;
	public int f = 10;
	public int g = 10;
	public int h = 10;

	[SerializeField]
	public int aprop {get{return a;}set{a = value;}}
	public int bprop {get{return a;}set{a = value;}}
	public int cprop {get{return a;}set{a = value;}}
	public int dprop {get{return a;}set{a = value;}}
}
