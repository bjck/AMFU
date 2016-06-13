using UnityEngine;
using System.Collections;

public class XMLTester : MonoBehaviour {


	// Use this for initialization
	void Start () 
	{
		string NodeName = "Features";
		string[] contents;
		contents = new string[3];
		contents[0] = "CanAttack";
		contents[1] = "IsInRange";
		contents[2] = "Sucks";
		string[] Domain = new string[3];
		Domain[0] = "Yes No";
		Domain[1] = "Barly No";
		Domain[2] = "VeryMuch, FUCKOOF BITCH 111";

		XMLManager.WriteToFile("lol.xml", contents, Domain ,NodeName);
		XMLManager.ReadFromFile ("lol.xml");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
