using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Feature
{
	private static int featureCount = 0;

	private List<string> featureDomain;
	private string featureValue;
	private string typeOfFeature;
	private string actor;
	private string operatorSign;

	public int ID{get; set;}
	public string TypeOfFeature{get{return typeOfFeature;}}
	public List<string> FeatureDomain{get{return featureDomain;} set{featureDomain = value;}}
	public string FeatureValue{get{return featureValue;}set{featureValue = value;}}
	public string Actor{get{return actor;}}
	public string OperatorSign{get{return operatorSign;}}
	
	public Feature(List<string> featureDomain,  string featureValue, string type, string actor, string op)
	{
		this.featureDomain = featureDomain;
		this.featureValue = featureValue;
		this.actor = actor;
		this.typeOfFeature = type;
		this.operatorSign = op;
		ID = featureCount;
		featureCount++;

	}

	public override string ToString ()
	{
		return string.Format ("[Feature: ID={0}, TypeOfFeature={1}, FeatureDomain={2}, FeatureValue={3}]", ID, TypeOfFeature, FeatureDomain, FeatureValue);
	}

	public void AddDomainValue(string domainValue)
	{
		featureDomain.Add (domainValue);
	}


	~Feature()
	{
		featureCount--;
	}
}