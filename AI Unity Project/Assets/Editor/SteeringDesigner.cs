using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TestingSteering))]
public class SteeringDesigner : Editor {
	public override void OnInspectorGUI ()
	{
		//serializedObject.Update();
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("_currentBehaviour"));
		//serializedObject.ApplyModifiedProperties();

		//Only show variables needed for current selected behaviour

		base.OnInspectorGUI();
	}
}
