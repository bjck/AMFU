using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DecisionTree
{
    class TreeTester : MonoBehaviour
	{
        public void DecisionTreePrinter(DecisionNode startNode)
        {
           // Console.WriteLine("Feature for Root is " + startNode.TestValue.ID.ToString());
            if (startNode.DaughterNodes != null)
            {
                for (int i = 0; i < startNode.DaughterNodes.Length; i++)
                {
                    if (startNode.DaughterNodes[i] != null)
                    {
//                        UnityEngine.Debug.Log("Decision Node " + startNode.TestValue.ID + " Incoming Egde " + startNode.DaughterNodes[i].incomingEgde);
                        DecisionTreePrinter(startNode.DaughterNodes[i]);
                    }
                }
            }
			else
			{
				if(startNode.Probability != null)
				{
					Debug.Log("Node: " + startNode.ToString());
					foreach(var pair in startNode.Probability)
					{
						Debug.Log("Probability of State: " + pair.Key + " : " +pair.Value);
					}
				}
			}
        }

        private void Start()
        {/*
            int numberOfTrainingsSets = 8;
            
            //Domain for Feature 1
            string[] HealthDomain = new string[2] {"100", "20"};
            string[] Ammo = new string[3] {"50", "100", "200"};
            string[] Shield = new string[3] {"100", "50", "0" };
			string[] Distance = new string[3] {"1000", "100", "50"};
            //Actions
			State attack = new State("attack");
			State defend = new State("defend");
			State takeCover = new State("cover");

            #region GeneratingExamples
            TrainingExample[] trainingExamples = new TrainingExample[4];
			Feature[] features = new Feature[4];
			features[0] = new Feature(HealthDomain.ToList(), "100", FeatureType.Health, "Player", ">=", "Health");
			features[1] = new Feature(Ammo.ToList(), "100", FeatureType.Ammo, "Player", ">=", "Ammo");
			features[2] = new Feature(Shield.ToList(), "100", FeatureType.Shield, "Player", ">=", "Shield");
			features[3] = new Feature(Distance.ToList(), "1000", FeatureType.Proximity, "Player", "==", "Distance");

			Feature[] features2 = new Feature[4];
			features2[0] = new Feature(HealthDomain.ToList(), "20", FeatureType.Health, "Player", "<=", "Health");
			features2[0].ID = features[0].ID;
			features2[1] = new Feature(Ammo.ToList(), "200", FeatureType.Ammo, "Player", ">=", "Health");
			features2[1].ID = features[1].ID;
			features2[2] = new Feature(Shield.ToList(), "0", FeatureType.Shield, "Player", "<=", "Health");
			features2[2].ID = features[2].ID;
			features2[3] = new Feature(Distance.ToList(), "1000", FeatureType.Proximity, "Player", "==", "Health");
			features2[3].ID = features[3].ID;

			Feature[] features3 = new Feature[4];
			features3[0] = new Feature(HealthDomain.ToList(), "20", FeatureType.Health, "Player", ">", "Health");
			features3[0].ID = features[0].ID;
			features3[1] = new Feature(Ammo.ToList(), "100", FeatureType.Ammo, "Player", ">=", "Health");
			features3[1].ID = features[1].ID;
			features3[2] = new Feature(Shield.ToList(), "0", FeatureType.Shield, "Player", "<=", "Health");
			features3[2].ID = features[2].ID;
			features3[3] = new Feature(Distance.ToList(), "50", FeatureType.Proximity, "Player", "<=", "Health");
			features3[3].ID = features[3].ID;

			Feature[] features4 = new Feature[4];
			features4[0] = new Feature(HealthDomain.ToList(), "100", FeatureType.Health, "Player", ">=", "Health");
			features4[0].ID = features[0].ID;
			features4[1] = new Feature(Ammo.ToList(), "100", FeatureType.Ammo, "Player", ">=", "Health");
			features4[1].ID = features[1].ID;
			features4[2] = new Feature(Shield.ToList(), "100", FeatureType.Shield, "Player", ">=", "Health");
			features4[2].ID = features[2].ID;
			features4[3] = new Feature(Distance.ToList(), "1000", FeatureType.Proximity, "Player", "==", "Health");
			features4[3].ID = features[3].ID;

			trainingExamples[0] = new TrainingExample(features.ToList (), attack);
			trainingExamples[1] = new TrainingExample(features2.ToList (), takeCover);
			trainingExamples[2] = new TrainingExample(features3.ToList (), defend);
			trainingExamples[3] = new TrainingExample(features4.ToList (), defend);
            #endregion

			DecisionTree tree = new DecisionTree(trainingExamples, features);
			DecisionTreePrinter (tree.RootNode);

			/*
            DecisionTree.Feature HealthFeature = new Feature(ref HealthDomain, "Healthy", 0);
            DecisionTree.Feature EnemyPositionFeature = new Feature(ref EnemyPositionDomain, "Far", 1);
            DecisionTree.Feature AmmonitionFeature = new Feature(ref AmmonitionDomain, "Full", 2);
            DecisionTree.Feature[] FeatureList = new DecisionTree.Feature[3] { HealthFeature, EnemyPositionFeature, AmmonitionFeature };

            //creating tree structure
            DecisionTree.DecisionNode RootNode = new DecisionTree.DecisionNode(null);
            DecisionTree.TreeLearner decisionTree = new DecisionTree.TreeLearner(trainingExamples, FeatureList, RootNode);
			UnityEngine.Debug.Log("Build the tree: " + decisionTree.ToString ());
            DecisionTreePrinter(RootNode);
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();*/
        }
    }  
}