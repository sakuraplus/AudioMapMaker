using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(PlayFabLogin ))]
public class pfbtesteditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		PlayFabLogin   pfblogin = (PlayFabLogin )target;
	
		if(GUILayout.Button("login test"))
		{
			// if button pressed, then call function in script
			Debug.Log ("oooo");
			pfblogin.logintest ();
		}
		if(GUILayout.Button("getnews test"))
		{
			// if button pressed, then call function in script
			Debug.Log ("nnn");
			pfblogin.getnewstest  ();
		}
	}
}
