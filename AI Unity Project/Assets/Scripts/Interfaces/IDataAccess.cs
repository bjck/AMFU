using UnityEngine;
using System.Collections;

public enum DataType
{
	All,
	Actions,
	Features,
	TrainingData,
}

public interface IDataAccess
{
	/// <summary>
	/// Used to fetch data.
	/// </summary>
	/// <returns>An ArrayList of the requested data</returns>
	/// <param name="type">Which DataType that was requested</param>
	ArrayList GetData(DataType type);
}
