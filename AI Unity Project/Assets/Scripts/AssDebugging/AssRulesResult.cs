using System.Collections;
using System.Collections.Generic;

namespace AssRules
{
	[System.Serializable]
	public class AssRulesResult 
	{
		public List<int> input = new List<int>();
		public List<int> output = new List<int>();
		public int support = 0;
		public double confidence = 0;

		public AssRulesResult(){}
	}
}
