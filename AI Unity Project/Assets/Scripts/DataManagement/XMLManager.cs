using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class XMLManager
{
	//#########################################################
	//USE THIS !!!11 :P
	//#########################################################

	static Dictionary<string, int> UniqueGameObjectIDs; 	
	public static void SaveAI(AIContainer data)
	{
		using (Stream stream = File.Open(Application.dataPath +"/AI.bin", FileMode.OpenOrCreate))
		{
			BinaryFormatter bin = new BinaryFormatter();
			bin.Serialize(stream, data);
			Debug.Log("Wrote a file to: " +Application.dataPath +"/AI.bin");
		}
	}

	public static AIContainer LoadAI()
	{
		AIContainer r = new AIContainer();
		using (Stream stream = File.Open(Application.dataPath +"/AI.bin", FileMode.OpenOrCreate))
		{
			BinaryFormatter bin = new BinaryFormatter();
			r  = (AIContainer)bin.Deserialize(stream);
		}
		return r;
	}

	public static List<AssRules.AssRulesResult> ReadCAR()
	{
		List<AssRules.AssRulesResult> l = new List<AssRules.AssRulesResult>();
		using(Stream stream = File.Open ("AIarules.bin", FileMode.Open))
		{
			BinaryFormatter bin = new BinaryFormatter();
			l = (List<AssRules.AssRulesResult>) bin.Deserialize(stream);
		}
		return l;
	}

	public static int GetID(string n)
	{
		if(UniqueGameObjectIDs == null)
		{
			LoadID();
		}
		if(UniqueGameObjectIDs != null && UniqueGameObjectIDs.ContainsKey (n))
		{
			return UniqueGameObjectIDs[n];
		}
		else
		{
			UniqueGameObjectIDs = new Dictionary<string, int>();
			UniqueGameObjectIDs.Add(n, UniqueGameObjectIDs.Count()-1);
			SaveID(UniqueGameObjectIDs);
			return UniqueGameObjectIDs.Count()-1;
		}
	}

	private static void LoadID()
	{
		try
		{
			using(Stream stream = File.Open(Application.persistentDataPath + "/GameObjectIDs.bin", FileMode.Open))
			{
				Debug.Log (Application.persistentDataPath);
				BinaryFormatter bin = new BinaryFormatter();
				UniqueGameObjectIDs = (Dictionary<string, int>)bin.Deserialize(stream);
			}
		}
		catch(FileNotFoundException)
		{
			Debug.Log ("No file found - Proceeding...");
		}
	}

	private static void SaveID(Dictionary<string, int> data)
	{
		using(Stream stream = File.Open(Application.persistentDataPath + "/GameObjectIDs.bin", FileMode.OpenOrCreate))
		{
			BinaryFormatter bin = new BinaryFormatter();
			bin.Serialize(stream, data);
			Debug.Log ("New Entry in ID has been Saved.");
		}
	}

	//#########################################################
	//Left Over data serilization - use with caution.
	//########################################################

	public static void PrintTree(DecisionTree.DecisionTree n)
	{
		using(StreamWriter s = new StreamWriter(Application.persistentDataPath + "/DecisionTree.txt", false))
		{
			s.Write(RecursiveTreePrintBuilder(n.RootNode, null).ToString());
		}
	}

	private static StringBuilder RecursiveTreePrintBuilder(DecisionTree.DecisionNode root, StringBuilder output, string indentation = "", int level =0)
	{
		if(output == null)
		{
			output = new StringBuilder();
		}
		else
		{
			indentation += "\t";
		}
		if(root.Actor == null)
		{
			output.AppendLine("ROOT");
			indentation += "\t";
		}
		if(root.DaughterNodes == null)
		{
			indentation += "\t";
			output.AppendLine (indentation + "Leaf " + root.FeatureType + " " + root.Operator + " " + root.incomingEgde + " " + root.Action.StateName + " " + root.Actor);
		}
		else
		{
			foreach(DecisionTree.DecisionNode d in root.DaughterNodes)
			{
				output.AppendLine (indentation + Array.IndexOf (root.DaughterNodes, d).ToString () + " " + d.FeatureType + " " + d.Operator + " " + d.incomingEgde + " " + d.Action.StateName + " " + d.Actor);
				RecursiveTreePrintBuilder(d, output, indentation, ++level);
			}
		}
		return output;
	}


	public static void WriteUniqueFeatures(string filePath, List<Feature> features, State action)
	{
		try
		{

			using (Stream stream = File.Open("features.bin", FileMode.OpenOrCreate))
			{
				BinaryFormatter bin = new BinaryFormatter();
				bin.Serialize(stream, features);
			}
		}
		catch(Exception e)
		{
			Debug.Log ("Error While Writing" +e.ToString());
			return;
		}
	}

	public static List<Feature> ReadUniqueFeatures(string filePath = "features.xml")
	{
		try
		{
			using(Stream stream = File.Open ("features.bin", FileMode.Open))
			{
				BinaryFormatter bin = new BinaryFormatter();

				var features = (List<Feature>)bin.Deserialize(stream);
				return features;
			}
		}
		catch(IOException){}
		return null;
	}

	public static void WriteExamples(List<DecisionTree.TrainingExample> t)
	{
		try
		{
			using(Stream stream = File.Open ("examples.bin", FileMode.OpenOrCreate))
			{
				BinaryFormatter b = new BinaryFormatter();
				b.Serialize(stream, t);
			}
		}
		catch(Exception)
		{
			return;
		}
		return;
	}

	public static List<DecisionTree.TrainingExample> ReadExamples()
	{
		try
		{
			using(Stream stream = File.Open ("examples.bin", FileMode.Open))
			{
				BinaryFormatter bin = new BinaryFormatter();

				var examples = (List<DecisionTree.TrainingExample>)bin.Deserialize(stream);
				return examples;
			}
		}
		catch(Exception e){Debug.Log ("I suck at reading: " +e.ToString ());	 }
		return null;
	}

	public static void WriteToFile(string filePath, string[] data, string[] domain, string node)
	{
		XDocument doc;
		try
		{
			doc = XDocument.Load (filePath);
		}
		catch
		{
			doc = new XDocument(new XElement("root"));

			for(int i = 0; i < data.Length; i++)
			{
				doc.Root.Add (new XElement(data[i],domain[i]));
			}
		}
		doc.Save (filePath);
	}

	public static string[][] ReadFromFile(string filePath)
	{
		XDocument doc = null;
		try
		{
			doc = XDocument.Load(filePath);
		}
		catch(Exception e)
		{
			throw new Exception("Something went wrong :C " + e.ToString ());
		}

		var rows = doc.Root.Elements();
		string[] rowValues = new string[rows.Count()];
		string[] rowNames = new string[rows.Count()];
		string[][] returnValue = new string[2][];
		int tmp = 0;
		//Debug.Log (rows.ToString ());
		foreach(var r in rows)
		{
			rowValues[tmp] = r.Value;
			rowNames[tmp] = r.Name.LocalName;
			tmp++;
		}
		returnValue[0] = rowNames;
		returnValue[1] = rowValues;

		return returnValue;
	}
}