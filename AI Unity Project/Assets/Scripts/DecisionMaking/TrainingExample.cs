using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionTree
{
	[System.Serializable]
	public class TrainingExample
	{
		private Feature[] features;
		private State action;
		private GameObject actor;

    public State Action { get { return action; } set { action = value; } }
		public Feature[] Features{get{return features;}}
		public GameObject Actor{get{return actor;}}

		public TrainingExample(List<Feature> features, string action)
		{
			this.features = features.ToArray();
			//this.action = action;
		}

		public TrainingExample(List<Feature> features, State action)
		{
			this.features = features.ToArray ();
			this.action = action;
			this.actor = GameObject.Find(features[0].Actor);
		}
	}
}