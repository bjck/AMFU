using UnityEngine;
using System.Collections;

public class MaxCondition : ICondition {

	private int MaxValue;
	private int TestValue;

	public void SetTestValue(int value, int max) {
		TestValue = value;	
		MaxValue = max;
	}
	
	public bool Test() {
		return MaxValue >= TestValue; 
	}
}
