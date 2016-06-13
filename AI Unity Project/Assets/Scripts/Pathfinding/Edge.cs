using System.Collections;

public class Edge
{
	private int source;
	private int goal;
	private double distance;

	public int Source{get{return source;}}
	public int Goal{get{return goal;}}
	public double Distance{get{return distance;}}

	public Edge(int source, int goal, double distance)
	{
		this.source = source;
		this.goal = goal;
		this.distance = distance;
	}
}
