#define DEBUG

using UnityEngine;
using System.Collections;

public static class DebugLogger
{
	public static void Log(object message)
	{
		#if DEBUG
		Debug.Log("<color=red><b>[DEBUG]</b></color> " + message);
		#endif
	}
	
	public static void LogError(object message)
	{
		#if DEBUG
		Debug.LogError("<color=red><b>[ERROR]</b></color> " + message);
		#endif
	}
}
