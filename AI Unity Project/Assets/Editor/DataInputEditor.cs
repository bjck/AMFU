using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(ObjectInScene))]
public class DataInputEditor : Editor
{
	private List<int> expressionCount = new List<int>();
	private Dictionary<GameObject, string[]> featureTypes = new Dictionary<GameObject, string[]>();
	private string inputData;
	private string[] Operators = new string[]{"<", ">", "==", ">=", "<="};
	private List<string> Actors = new List<string>();
	private static List<State> ActionsAsStates = new List<State>();
	private static string[] Actions = new string[4];
	private static bool hasBeenInitilized = false;
	
	private List<List<int>> actorHighlights = new List<List<int>>();
	private List<List<int>> featureTypeHighlights = new List<List<int>>();
	private List<List<int>> operatorsHightlights = new List<List<int>>();
	private List<List<string>> inputHighlight = new List<List<string>>();
	private List<int> actionHightlights = new List<int>();
	private ObjectInScene myTarget;
	private Vector2 scrollPositionAgent = Vector2.zero;

	private List<PropertyInfo[]> myTargetProperties = new List<PropertyInfo[]>();
	private List<SerializedObject> serializedObj = new List<SerializedObject>();
	private List<Vector2> examplesScrollview = new List<Vector2>();
	private List<List<Feature>> FeatureList = new List<List<Feature>>();
	private List<bool> Foldouts = new List<bool>();
	private Vector2 overallscroll = Vector2.zero;

	private static void init()
	{
		if(!hasBeenInitilized)
		{
			hasBeenInitilized = true;
			for(int i = 0; i < 4; i++)
			{
				ActionsAsStates.Add(new State(new Transition()));
			}
			ActionsAsStates[0].StateName = "Default";
			ActionsAsStates[1].StateName = "Attack";
			ActionsAsStates[2].StateName = "Defend";
			ActionsAsStates[3].StateName = "Flee";
			for(int i = 0; i < ActionsAsStates.Count; ++i)
			{
				Actions[i] = ActionsAsStates[i].StateName;
			}
		}
	}


	private void AddFeatureToRule(int k)
	{
		if (myTarget == null) 
		{
			Debug.Log("myTarget er null");
		}

		else 
		{
			expressionCount[k]++;
			inputHighlight.Add (new List<string>());
			inputHighlight[k].Add("#");
			actorHighlights[k].Add (0);
			featureTypeHighlights[k].Add (0);
			operatorsHightlights[k].Add (0);

			for(int i = 0; i < myTarget.agent.Count; i++)
			{
				if(!featureTypes.ContainsKey(myTarget.agent[i]))
				{
					SerializedProperty property2 = serializedObj[i].GetIterator();
					List<string> result = new List<string>();
					// puts stuff properly in the list that can be chosen.
					while (property2.NextVisible(true)) 
					{
						if (property2.type == "int")
						{
							result.Add(property2.name);
							//Debug.Log("expressionCount[k]-1: " + expressionCount[k]);
							inputHighlight[expressionCount[k]-1].Add (property2.intValue.ToString ());
						}
						if (property2.type == "float")
						{
							result.Add(property2.name);
							//Debug.Log("expressionCount[k]-1: " + expressionCount[k]);
							inputHighlight[expressionCount[k]-1].Add (property2.floatValue.ToString ());
						}
						
					}
					featureTypes.Add (myTarget.agent[i], result.ToArray());
				}
			}
		}
	}
	private void RemoveRule(int k)
	{
		if(expressionCount[k] > 1)
		{
			expressionCount[k]--;
			inputHighlight.RemoveAt (inputHighlight.Count-1);
			inputHighlight[k].RemoveAt (inputHighlight[k].Count-1);
			actorHighlights[k].RemoveAt (actorHighlights[k].Count -1);
			featureTypeHighlights[k].RemoveAt (featureTypeHighlights[k].Count -1);
			operatorsHightlights[k].RemoveAt (operatorsHightlights[k].Count -1);
		}
	}
	
	private void AddExample()
	{
		Foldouts.Add (true);
		actionHightlights.Add (0);
		actorHighlights.Add (new List<int>());
		featureTypeHighlights.Add (new List<int>());
		operatorsHightlights.Add (new List<int>());
		inputHighlight.Add (new List<string>());
		expressionCount.Add (0);
		AddFeatureToRule (expressionCount.Count()-1);
		examplesScrollview.Add (new Vector2());
	}

	private void RemoveExample()
	{
		Foldouts.RemoveAt(Foldouts.Count-1);
		actionHightlights.RemoveAt(actionHightlights.Count-1);
		actorHighlights.RemoveAt(actorHighlights.Count-1);
		featureTypeHighlights.RemoveAt (featureTypeHighlights.Count - 1);
		operatorsHightlights.RemoveAt (operatorsHightlights.Count -1);
		inputHighlight.RemoveAt (inputHighlight.Count -1);
		expressionCount.RemoveAt (expressionCount.Count -1);
		examplesScrollview.RemoveAt (examplesScrollview.Count - 1);

	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		
		myTarget = (ObjectInScene)target;
		EditorGUILayout.LabelField("Drag n drop your actor");
		scrollPositionAgent = EditorGUILayout.BeginScrollView(scrollPositionAgent);
		for(int i = 0; i < myTarget.agent.Count; ++i)
		{
			myTarget.agent[i] = (GameObject)EditorGUILayout.ObjectField(myTarget.agent[i], typeof(GameObject), true);
			myTargetProperties.Add ( myTarget.agent[i].GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance));
			serializedObj.Add (null);
			if(serializedObj[i] == null)
			{
				Debug.Log ("Updating serializedObj");
				serializedObj[i] = new SerializedObject (myTarget.agent[i].GetComponents<MonoBehaviour>());
			}
		}
		//Formatting the Actors List to update when the list is changed.
		if(myTarget.agent.Count != Actors.Count)
		{
			Actors.Clear ();
			for(int i = 0; i < myTarget.agent.Count; i++)
			{
				Actors.Add(myTarget.agent[i].name);
				Actors = Actors.Distinct ().ToList ();
			}
			
		}
		EditorGUILayout.EndScrollView();


		#region DataInput
		overallscroll = EditorGUILayout.BeginScrollView(overallscroll);
		if(expressionCount.Count > 0)
		{
			for(int k = 0; k < expressionCount.Count; k++)
			{
				Foldouts[k] = EditorGUILayout.Foldout(Foldouts[k], "Rule "+k); 
				if(Foldouts[k])
				{
					//GUI.BeginGroup (new Rect(0,0,Screen.width, Screen.height));
					//examplesScrollview[k] = EditorGUILayout.BeginScrollView(examplesScrollview[k]);
					if(featureTypes != null  && Actors.Count > 0 && featureTypes.Count > 0)
					{
						if(expressionCount[k] > 0)
						{
							for(int i = 0; i < expressionCount[k]; i++)
							{
								EditorGUILayout.LabelField("Condition " + i.ToString ());
								actorHighlights[k][i] = EditorGUILayout.Popup(actorHighlights[k][i],Actors.ToArray (), EditorStyles.popup);
								//Getting the proper list of properties.
								var tmp = 	(from sceneObjects in myTarget.agent
											where sceneObjects.name == Actors[actorHighlights[k][i]]
											select sceneObjects).First();
								featureTypeHighlights[k][i] = EditorGUILayout.Popup(featureTypeHighlights[k][i],featureTypes[tmp].ToArray (), EditorStyles.popup);
								operatorsHightlights[k][i] = EditorGUILayout.Popup(operatorsHightlights[k][i],Operators, EditorStyles.popup);
								inputHighlight[k][i] = EditorGUILayout.TextField (inputHighlight[k][i]);
							}
						}
					}

					if(GUILayout.Button("Add Rule"))
					{
						AddFeatureToRule(k);
					}
					if(GUILayout.Button ("Remove Rule"))
					{
						RemoveRule(k);
					}

					EditorGUILayout.LabelField("Action - Choose which kind of action your agent should perform");
					actionHightlights[k] = EditorGUILayout.Popup(actionHightlights[k], Actions, EditorStyles.popup);
					EditorGUILayout.Space ();
					//EditorGUILayout.EndScrollView();
					//GUI.EndGroup();
				}
			}
			
		}

		EditorGUILayout.EndScrollView();
		if(GUILayout.Button ("Add Example"))
		{
			init();
			//Debug.Log ("Antal expressions " + expressionCount.Count);
			if(expressionCount.Count < 1)
			{
				List<DecisionTree.TrainingExample> examples = XMLManager.ReadExamples();
				//Debug.Log (examples.ToString ());
				if(examples != null && examples.Count > 0)
				{
					for(int i = 0; i < examples.Count; i++)
					{
						expressionCount.Add (examples[i].Features.Count ());
						//GameObject to control
						actorHighlights.Add (new List<int>());
						//Feature type: Proximity
						featureTypeHighlights.Add (new List<int>());
						//< == > !=
						operatorsHightlights.Add(new List<int>());
						//Value
						inputHighlight.Add (new List<string>());	
						for(int j = 0; j < examples[i].Features.Length; j++)
						{
							//FIXED Test if this works properly everthing should be setup for it now
							actorHighlights[i].Add (Actors.IndexOf (examples[i].Features[j].Actor));
							//Need an additional array to keep track of the actual gameobjects...
							actionHightlights.Add (0);
							featureTypeHighlights[i].Add(Array.IndexOf (featureTypes[myTarget.agent[0]], examples[i].Features[j].TypeOfFeature));
							operatorsHightlights[i].Add (Array.IndexOf (Operators, examples[i].Features[j].OperatorSign));
							inputHighlight[i].Add (examples[i].Features[j].FeatureValue);
						}
					}
				}
				else
				{
					AddExample();
				}
			}
			else
			{
				AddExample();
			}
			
			//Import from binary reader
		}
		
		if(GUILayout.Button ("Remove Example"))
		{
			if(expressionCount.Count > 1)
			{
				RemoveExample();
			}

		}
		if(GUILayout.Button ("Export Rules"))
		{
			InstantiateFeatures();
			CreateTrainingData();
			for (int i = 0; i < FeatureList.Count; i++) 
			{
				XMLManager.WriteUniqueFeatures("Data.bin", FeatureList[i], new State(new Transition()));
			}
			
		}
		//GUILayout.EndArea();
#endregion
	}

	private void CreateTrainingData() 
	{
		DecisionTree.TrainingExample[] examples = new DecisionTree.TrainingExample[FeatureList.Count];
		for(int i = 0; i < FeatureList.Count; i++)
		{
			examples[i] = (new DecisionTree.TrainingExample(FeatureList[i], ActionsAsStates[actionHightlights[i]]));
		}
		AIWrapper.CreateTree (examples);
	}

	private void InstantiateFeatures() 
	{
		FeatureList.Clear();
		for(int i = 0; i < expressionCount.Count; i++) 
		{
			FeatureList.Add(new List<Feature>());
			for(int j = 0; j < expressionCount[i]; j++) 
			{
				string featureValue = inputHighlight[i][j].ToString();
				List<string> featureDomain = new List<string>();
				featureDomain.Add(featureValue);
				var tmp = 	(from sceneObjects in myTarget.agent
				            where sceneObjects.name == Actors[actorHighlights[i][j]]
				            select sceneObjects).First();
				string featureType = featureTypes[tmp][featureTypeHighlights[i][j]];
				string featureOperator = Operators[operatorsHightlights[i][j]];
				string featureActor = Actors[actorHighlights[i][j]];
				
				//Find feature, if feature containing values already exists
				foreach(List<Feature> fl in FeatureList)
				{
					foreach(Feature ffs in  fl)
					{
						if(ffs.TypeOfFeature == featureType && ffs.Actor == featureActor && ffs.FeatureValue != featureValue)
						{
							ffs.FeatureDomain.Add (featureValue);
							ffs.FeatureDomain = ffs.FeatureDomain.Distinct ().ToList ();
						}
					}
				}

				Feature validExistingFeature = null;
				for(int k = 0; k < FeatureList.Count; k++) 
				{
					if(k != i)
					{
						validExistingFeature = FeatureList[k].Find(f => f.TypeOfFeature == featureType && f.Actor == featureActor);
					}
				}
				if(validExistingFeature == null) 
				{
					//Create a list of duplicate features
					List<Feature> duplicateFeatureTypes = new List<Feature>();
					
					for (int k = 0; k < FeatureList.Count; k++)
					{
						//Add features with the same feature type to one another
						duplicateFeatureTypes.AddRange(FeatureList[k].Where (f => f.TypeOfFeature == featureType && f.Actor == featureActor));
					}

					if (duplicateFeatureTypes.Count > 0) 
					{

						foreach(var feature in duplicateFeatureTypes) 
						{
							feature.AddDomainValue(featureValue);
							
							if(!featureDomain.Contains(feature.FeatureValue)) 
							{
								featureDomain.Add(feature.FeatureValue);
							}
						}
						FeatureList[i].Add(new Feature(featureDomain, featureValue, featureType, featureActor, featureOperator));
					} 
					else 
					{
						FeatureList[i].Add(new Feature(featureDomain, featureValue, featureType, featureActor, featureOperator));
					}
				} 
				else 
				{
					FeatureList[i].Add (new Feature(featureDomain, featureValue, featureType, featureActor, featureOperator));
				}
				}
			}
		}
	}