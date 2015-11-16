using UnityEngine;
using System.Collections;

public class CaptureTheFlagPrimitive : PrimitiveScript
{
	void Start () 
	{
		var singleRound = (bool)SettingParser.getSetting(instance, "singleRound");
		var playerCount = (int)SettingParser.getSetting (instance, "playerCount");
		
		Debug.Log ("Single round? " + singleRound);
		Debug.Log ("Player count? " + playerCount);
	}
}
