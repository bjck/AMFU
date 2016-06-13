using System.Collections;
using System;

namespace DecisionTree
{
	public class Action 
	{
		private int id;
		public int ID{get{return id;}}

		public Action(int ID)
		{
			this.id = ID;
		}
	}
}