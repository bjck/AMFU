using UnityEngine;
using System.Collections;

public class MinCondition : ICondition {

	private int MinValue;
	private int TestValue;
	
	public void SetTestValue(int value, int min) {
		TestValue = value;	
		MinValue = min;
	}
	
	public bool Test() {
		return MinValue <= TestValue; 
	}
}
