using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
public class Sensor : MonoBehaviour
{
	//FIXME Instead of 1 get numbers of actors from... somewhere
	private const int numberOfActors = 1;
	[SerializeField]
	private GameObject[] Actors = new GameObject[numberOfActors];
	/// <summary>
	/// Dictionary key is Actor(GameObject), each index in the list is a script 
	/// and each entry in the containing List is a property
	/// </summary>
	public Dictionary<GameObject, List<List<PropertyInfo>>> Properties;
	public List<string> test;

	private void Start()
	{
		GetFields (Actors[0]);
	}

	/// <summary>
	/// returns world data of actor.
	/// </summary>
	/// <param name="f">F integer value of Featuretype.</param>
	public static List<List<PropertyInfo>> GetFields(GameObject actor)
	{
		List<List<PropertyInfo>> PropertyList = new List<List<PropertyInfo>>();
		MonoBehaviour[] scripts = actor.GetComponents<MonoBehaviour>();
		for(int i = 0; i < scripts.Length; i++)
		{
			PropertyList.Add (new List<PropertyInfo>());
			foreach(var fi in scripts[i].GetType().GetProperties())
			{
				PropertyList[i].Add (fi);
			}
		}
		return PropertyList;
	}

}
