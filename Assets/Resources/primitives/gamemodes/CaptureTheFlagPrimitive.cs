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
		
		var script = (ILocomotionScript)character.AddComponent(Type.GetType(characterInstance.primitive.locomotionScriptPath));
		
		script.instance = characterInstance;
		
		//character.AddComponent(Types.GetType (characterInstance.controllerScript));
		
		//Actually could we just use:
		var originPoint = getOriginPoint();
		var randomOffset = randomXZAroundPoint(originPoint, 3f);
		
		randomOffset.y += character.GetComponent<MeshRenderer>().bounds.extents.y * character.transform.localScale.y;
	
		character.transform.position = randomOffset;
	}
}
