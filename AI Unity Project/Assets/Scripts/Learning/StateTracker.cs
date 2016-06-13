using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class StateTracker : MonoBehaviour 
{
	private static Dictionary<GameObject, List<State[]>> TransitionHistory = new Dictionary<GameObject, List<State[]>>();

	public static void AddTransition(State from, State to, GameObject actor)
	{
		Debug.Log ("Added Transition");
		List<State[]> Transition = new List<State[]>();
		Transition.Add (new State[]{from, to});
		if(TransitionHistory.ContainsKey(actor))
		{
			Transition = TransitionHistory[actor];
			Transition.Add (new State[]{from, to});
			TransitionHistory[actor] = Transition;
		}
		else
		{
			TransitionHistory.Add(actor, Transition);
		}
	}

	public static void WriteHistory()
	{
		using(StreamWriter sr = new StreamWriter("contextZart.txt", true))
		{
			foreach(KeyValuePair<GameObject,List<State[]>> k in TransitionHistory)
			{
				foreach(State[] s in k.Value)
				{
					foreach(State t in s)
					{
						sr.Write(FiniteStateMachine.States.IndexOf(t).ToString() + " ");
					}
				}

			}
		}
	}

	public static void WriteHistory(GameObject go)
	{
		using(StreamWriter sr = new StreamWriter("contextZart.txt", true))
		{
			foreach(KeyValuePair<GameObject,List<State[]>> k in TransitionHistory)
			{
				foreach(State[] s in k.Value)
				{
					XMLManager.GetID(k.Key.name);
					foreach(State t in s)
					{
						//StateName to and ID -_- sigh...
						sr.Write(FiniteStateMachine.States.IndexOf(t).ToString() + " ");
					}
					sr.Write (Environment.NewLine);
				}
				
			}
		}
		Debug.Log("Can't you write to file any faster Claus?");

	}
	/// <summary>
	/// Discards the history of go.
	/// </summary>
	/// <param name="go">GameObject.</param>
	public static void DiscardHistory(GameObject go)
	{
		TransitionHistory.Remove(go);
	}
}
