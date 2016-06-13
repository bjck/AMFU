using System.Collections;
using System.Collections.Generic;
namespace DecisionTree
{
	[System.Serializable]
	public class DecisionNode
	{
		private DecisionNode[] daughterNodes;
		private Feature testValue;
		private State action;

		public DecisionNode[] DaughterNodes{get{return daughterNodes;} set{daughterNodes = value;}}
		public Feature TestValue{get{return testValue;} set{testValue = value;}}
        public DecisionNode Parent;
        public string incomingEgde;
		public State Action{get{return action;} set{action = value;}}
		public State[] Actions;
		public string Operator;
		public string FeatureType;
		public Dictionary<string, float> Probability;
		public string Actor;

		public DecisionNode(): this(null){}

		public DecisionNode(DecisionNode parent)		
		{
            this.Parent = parent;
			//HACK domain size is only 2
		}

        public void CreateDaughterNodes(int size)
        {
            daughterNodes = new DecisionNode[size];
        }

		public override string ToString ()
		{
			return string.Format ("[DecisionNode: DaughterNodes={0}, TestValue={1}, Action={2}]", DaughterNodes, TestValue, Action);
		}

	}

}