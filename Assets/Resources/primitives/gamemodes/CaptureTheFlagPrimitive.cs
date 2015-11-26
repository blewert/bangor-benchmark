using UnityEngine;
using System.Collections;
using System;

public class CaptureTheFlagPrimitive : GamemodeScript
{
	void Start () 
	{
		var singleRound = (bool)SettingParser.getSetting(instance, "singleRound");
		var playerCount = (int)SettingParser.getSetting (instance, "playerCount");
		
		Debug.Log ("Single round? " + singleRound);
		Debug.Log ("Player count? " + playerCount);
		
		//Need to attach script, and also add environment instance into GamemodePrimitive so we can access the origin point.
		var character = (GameObject)Instantiate (Resources.Load (characterInstance.primitive.prefabPath), Vector3.zero, Quaternion.identity);
		
		character.AddComponent(Type.GetType(characterInstance.primitive.locomotionScriptPath));
		//character.AddComponent(Types.GetType (characterInstance.controllerScript));
		
		//Actually could we just use:
		var originPoint = getOriginPoint();
		Debug.Log (originPoint);
	}
}
