using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using AssRules;
using UnityEngine;
using UnityEditor;

namespace DecisionTree
{
	[System.Serializable]
	public class DecisionTree
	{
		private DecisionNode rootNode;
		public Feature[] Features { get; private set;}

		public DecisionNode RootNode{get{return rootNode;}}

		public DecisionTree(TrainingExample[] examples, Feature[] features) : this(examples, features, new DecisionNode()){}

		public DecisionTree(TrainingExample[] examples, Feature[] features, DecisionNode decisionNode)
		{
			rootNode = decisionNode;
			Features = features;

			TreeLearner(examples, features, decisionNode);
		}

		public Dictionary<string, float> Query()
		{
			//Refreshes the data in the features array
			GetUpdatedData();
			//Printing Tree
			Debug.Log ("Printing Tree to file: " + Application.persistentDataPath);
			XMLManager.PrintTree(this);
			DecisionNode leaf = Search(Features.ToList(), rootNode);
			if(leaf == null)
			{
				return null;
			}
			return leaf.Probability;
		}

		private void GetUpdatedData()
		{
			/* This is a very expensive way to do this but it's late :3*/
			Dictionary<string, GameObject> agentList = new Dictionary<string, GameObject>();
			for(int i = 0; i < Features.Length; i++)
			{
				//Adding new found actor to list.
				if(!agentList.ContainsKey(Features[i].Actor))
				{
					agentList.Add(Features[i].Actor, GameObject.Find (Features[i].Actor));
				}
				SerializedObject serializedObj = new SerializedObject (agentList[Features[i].Actor].GetComponents<MonoBehaviour>());
				SerializedProperty property = serializedObj.GetIterator();
				while (property.NextVisible(true))
				{
					if (property.type == "float")
					{
						foreach(Feature f in Features)
						{
							if(property.name == f.TypeOfFeature)
							{
								f.FeatureValue = property.floatValue.ToString ();
							}
						}
					}
				}
			}

		}

		/*
		 * FIXME We do not check each tree for each feature but find it in a first fit method
		 * This results in inaccurate behaviour but avoids exponential running time...
		*/

		private DecisionNode Search(List<Feature> features, DecisionNode node)
		{
			if(features.Count >= 1 && node.DaughterNodes != null && node.DaughterNodes.Count() >= 1)
			{
				foreach(Feature f in features)
				{
					//if(node.TestValue.ID == f.ID)
					//{
					for(int i = 0; i < node.DaughterNodes.Count(); ++i)
						{
						if(f.TypeOfFeature == node.DaughterNodes[i].FeatureType && f.Actor == node.DaughterNodes[i].Actor)
							{
							//f.OperatorSign
							for(int j = 0; j < node.DaughterNodes.Count(); ++j)
							{
								if(Math.Abs(float.Parse(node.DaughterNodes[j].incomingEgde) - float.Parse(f.FeatureValue)) < Math.Abs (float.Parse(node.DaughterNodes[i].incomingEgde) - float.Parse(f.FeatureValue)) && i != j)
								{
									i = j;
								}
							}
								switch(node.DaughterNodes[i].Operator)
								{
									case ">":
									if(float.Parse(f.FeatureValue) > float.Parse(node.DaughterNodes[i].incomingEgde))
									{
										features.Remove(f);
									return Search(features, node.DaughterNodes[i]);
									}
									break;
									case "<":
								if(float.Parse(f.FeatureValue) < float.Parse(node.DaughterNodes[i].incomingEgde))
									{
										features.Remove(f);
									return Search(features, node.DaughterNodes[i]);
									}
									break;
									case ">=":
								if(float.Parse(f.FeatureValue) >= float.Parse(node.DaughterNodes[i].incomingEgde))
									{
										features.Remove(f);
									return Search(features, node.DaughterNodes[i]);
									}
									break;
									case "<=":
								if(float.Parse(f.FeatureValue) <= float.Parse(node.DaughterNodes[i].incomingEgde))
									{
										features.Remove(f);
									return Search(features, node.DaughterNodes[i]);
									}
									break;
									case "==":
								if(float.Parse(f.FeatureValue) ==  float.Parse(node.DaughterNodes[i].incomingEgde))
									{
										features.Remove(f);
									return Search(features, node.DaughterNodes[i]);
									}
									break;



								}
							}
						}
					//}
				}
			}
			else
			{
				return node;
			}
			Debug.Log ("WARNING - Undefined Behaviour, defaulting to current State");
			return node;
		}
		private void TreeLearner(TrainingExample[] examples, Feature[] features, DecisionNode decisionNode)
		{
			if(examples.Length == 0)
				return;
			if(features.Length == 0)
				return;
			double entropy = Entropy (examples);
			List<TrainingExample>[] bestSets = null;
			if(entropy <= 0)
			{
				return;
			}
			else
			{
				int exampleCount = examples.Length;
				double bestInformationGain = 0;
				Feature bestSplitFeature = null;
				foreach(Feature f in features)
				{
					List<TrainingExample>[] sets = SplitByAttribute(examples, f);
					double overallEntropy = EntropyOfSets(sets, exampleCount);
					double informationGain = entropy - overallEntropy;
					if(informationGain > bestInformationGain)
					{
						bestInformationGain = informationGain;
						bestSplitFeature = f;
						bestSets = sets;
					}
				}
				decisionNode.TestValue = bestSplitFeature;
	            
				List<Feature> lst = features.OfType<Feature>().ToList();
				lst.Remove (bestSplitFeature);
                Feature[] newFeatures = new Feature[features.Length-1];
                newFeatures = lst.ToArray();

                //Instatiate Daugther array of current decisioNode to be equal size of feature's domain
				decisionNode.CreateDaughterNodes(bestSplitFeature.FeatureDomain.Count);
				
				for (int i = 0; i < bestSets.Count(); i++)
                {
                    if (bestSets[i].Count == 0)
                    {
                        decisionNode.DaughterNodes[i] = new DecisionNode(decisionNode);
						decisionNode.DaughterNodes[i].incomingEgde = bestSplitFeature.FeatureDomain[i];
						decisionNode.DaughterNodes[i].Action = decisionNode.Action;
						decisionNode.DaughterNodes[i].Operator = bestSplitFeature.OperatorSign;
						decisionNode.DaughterNodes[i].FeatureType = bestSplitFeature.TypeOfFeature;
						decisionNode.DaughterNodes[i].Actor = bestSplitFeature.Actor;

                    }
					else
					{
		                //If there are no trainings examples for the domain value we create a new node with the action.
		                
		                //Otherwise we create a subset of nodes from each set of trainings examples.
		                decisionNode.DaughterNodes[i] = new DecisionNode(decisionNode);
						decisionNode.DaughterNodes[i].incomingEgde = bestSplitFeature.FeatureDomain[i];
							
						List<State> actions = new List<State>(); 

		                foreach (TrainingExample t in bestSets[i])
		                {
		                    actions.Add(t.Action);
		                }

		                State mostCommonAction = (	from j in actions
				                                        group j by j into grp
				                                        orderby grp.Count() descending
				                                        select grp.Key).First();
		                decisionNode.DaughterNodes[i].Action = mostCommonAction;
						decisionNode.DaughterNodes[i].FeatureType = bestSplitFeature.TypeOfFeature;
						decisionNode.DaughterNodes[i].Probability = GetProbability(actions);
						decisionNode.DaughterNodes[i].Operator = bestSplitFeature.OperatorSign;
						decisionNode.DaughterNodes[i].Actor = bestSplitFeature.Actor;
		                TreeLearner(bestSets[i].ToArray(), newFeatures, decisionNode.DaughterNodes[i]);
					}
                    
				}

			}
		}

        private List<State> GetDistinctValues(List<TrainingExample> list)
        {
            return list.Select(e => e.Action).Distinct().ToList() ;
        }
		/// <summary>
		/// Calculate the entropy or disorder of the example set.
		/// </summary>
		/// <param name="examples">Traning Examples.</param>
		private double Entropy(TrainingExample[] examples)
		{
			int exampleCount = examples.Length;
			double entropy;

			if(exampleCount <= 1)
			{
				return 0;
			}
			else
			{
				//int[] ActionTallies = new ActionTallies[examples.Length];
				//ArrayList ActionTallies = new ArrayList();
				Dictionary<State, int> ActionTallies  = new Dictionary<State, int>();
				foreach(TrainingExample e in examples)
				{
					if(ActionTallies.ContainsKey(e.Action))
					{
						ActionTallies[e.Action] = ActionTallies[e.Action]+1;
					}
					else
					{
						ActionTallies.Add(e.Action, 1);
					}
				}

				int actionCount = ActionTallies.Count;

				if(actionCount == 0)
				{
					return 0;
				}
				else
				{
					entropy = 0;
					foreach(KeyValuePair<State, int> i in ActionTallies)
					{
						double proportion = (double)i.Value / (double)exampleCount;
						entropy -= proportion * Math.Log (proportion);
					}
				}
			}
			return entropy;
		}

		private List<TrainingExample>[] SplitByAttribute(TrainingExample[] examples, Feature feature)
		{
			//Array of TrainingExample Lists.
			List<TrainingExample>[] sets = new List<TrainingExample>[feature.FeatureDomain.Count];
			//Initilizing Lists
			for(int i = 0; i <  feature.FeatureDomain.Count; i++)
			{
				sets[i] = new List<TrainingExample>();
			}
			//run through every example and look at the featurevalue of the argument feature, and add example of the corresponding attribute to a trainingslist.
			//TODO Domain mismatch of different Actors ? ->Features need to take actor into account!
			foreach(TrainingExample e in examples)
			{
				int indexOfFeature = -1;
				//finding the index of the target feature in the example e and using its feature value as index to add the example to.
				for(int i = 0; i < e.Features.Count(); i++)
				{
					//Added && clause for multiple actors
					if(e.Features[i].TypeOfFeature == feature.TypeOfFeature && e.Features[i].Actor == feature.Actor)
					{
						indexOfFeature = i;
					}
				}
				Debug.Log ("The value of " + e.Features[indexOfFeature].TypeOfFeature + " = " + e.Features[indexOfFeature].FeatureValue + "was added to sets");
				int indexOfFeatureValue = Array.IndexOf(feature.FeatureDomain.ToArray (), e.Features[indexOfFeature].FeatureValue);
                sets[indexOfFeatureValue].Add(e);
			}
			Debug.Log("Halt");
			return sets;
		}

		private double EntropyOfSets(List<TrainingExample>[] sets, int exampleCount)
		{
			double entropy = 0;
            double proportion = 0;
			foreach(List<TrainingExample> l in sets)
			{
				proportion = (double)l.Count / exampleCount;
				entropy -= proportion * Entropy(l.ToArray());
			}
			return entropy;
		}

		private Dictionary<string, float> GetProbability(List<State> actions)
		{
			Dictionary<string, float> probabilities = new Dictionary<string, float>();
			var query = actions.Select(x => x.StateName).GroupBy(s=>s).Select (g => new {Name = g.Key, Count = g.Count()});
			foreach(var result in query)
			{
				probabilities.Add(result.Name.ToString (),(float)result.Count / (float)query.Count ());
			}
			return probabilities;
		}
	}
}