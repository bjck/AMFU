using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AssRules;
using System.IO;
using System.Linq;
public class AIWrapper
{
	private static bool isAssLoaded = false;
	public static List<AssRulesResult> CARResults;
	public static Dictionary<string, DecisionTree.DecisionTree> DecisionTrees = new Dictionary<string, DecisionTree.DecisionTree>();

	//TODO Need a similiar structure for the Association rules indexed by actor(type) preferably.
	//TrainingExample[] examples, Feature[] features, DecisionNode decisionNode
	public static void CreateTree(DecisionTree.TrainingExample[] examples)
	{
		try
		{
			DecisionTrees.Add(examples[0].Actor.name, new DecisionTree.DecisionTree(examples, examples[0].Features));
			AIContainer d = new AIContainer();
			d.Key = examples[0].Actor.name;
			d.DTree = DecisionTrees[examples[0].Actor.name];
			XMLManager.SaveAI(d);
		}
		catch(ArgumentException)
		{
			DecisionTree.DecisionTree d = new DecisionTree.DecisionTree(examples, examples[0].Features);
			Debug.Log("There already exists a Decision Tree for this actor!, Overwriting previous tree.");
			DecisionTrees[examples[0].Actor.name] = d;
			AIContainer con = new AIContainer();
			con.Key =  examples[0].Actor.name;
			con.DTree = DecisionTrees[examples[0].Actor.name];
			XMLManager.SaveAI(con);
		}
	}
	/// <summary>
	/// Runs the Association Rules algorithm. Very expensive !
	/// </summary>
	public static void CreateAssRules()
	{
		if(isAssLoaded == false)	
		{
			isAssLoaded = true;
			//Clean up from last run
			if(File.Exists("AIarules.bin"))
			{
				File.Delete("AIarules.bin");
			}
			Program.RunAss();
			//Import AssRules into memory
			CARResults = XMLManager.ReadCAR();
		}
	}

	public static State GetDecision(GameObject actor, State currentState)
	{	
		AssRulesResult CARresult = null;
		bool CARhasResult = false;
		if(CARResults != null && CARResults.Count > 0)
		{
			CARhasResult = true;
			foreach(AssRulesResult r in CARResults)
			{
				if(Convert.ToInt32(actor.name) == r.input[0])
				{
					if(r.input[1] == Convert.ToInt32(currentState.StateName))
					{
						CARresult = r;
					}
				}
			}
		}
		if(DecisionTrees.Count <= 0)
		{
			AIContainer result = XMLManager.LoadAI();
			DecisionTrees.Add (result.Key, result.DTree);
		}
		Dictionary<string, float> dtreeresult = DecisionTrees[actor.name].Query ();
		string highestState = "default";
		float highestProbability = -1;
		if(dtreeresult != null)
		{
			foreach(KeyValuePair<string, float> r in dtreeresult)
			{
				if(r.Value > highestProbability)
				{
					highestProbability = r.Value ;
					highestState = r.Key;
				}
			}
		}
		if(CARhasResult)
		{
			if(CARresult.confidence > highestProbability && CARresult.support >= 1)
			{
				//foreach(State s in State.States)
				for(int i = 0; i < FiniteStateMachine.States.Count; ++i)
				{
					if(i == CARresult.output[0])
					{
						return FiniteStateMachine.States[i];
					}
				}
			}
		}

		else
		{
			return TurnManager.CurrentTurnManInstance.UnitFSM.FSM.GetState(highestState);
		}
		return TurnManager.CurrentTurnManInstance.UnitFSM.FSM.GetState(highestState);

		//TODO Implement the retrievable of ASSRULES
		//TODO Compare confidence of ass rules to probablity of DecisionTree.

	}
}
