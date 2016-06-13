using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityDistribution
{
	public State[] Action{private set; get;}
	public float[] Probability{private set; get;}
	ProbabilityDistribution(ref State[] a, ref float[] p)
	{
		Action = a;
		Probability = p;
	}
}