﻿using UnityEngine;
using System.Collections;

/// <summary>
/// A static class for parsing primitives from XML files. 
/// </summary>
public static class PrimitivesParser
{
	private const string BASE_PATH = "Assets/Config/";
	
	private static string environmentPrimitivesPath = BASE_PATH + "environmentPrimitives.xml";
	private static string charactersPrimitivesPath  = BASE_PATH + "charactersPrimitives.xml";
	private static string gamemodePrimitivesPath    = BASE_PATH + "gamemodePrimitives.xml";
	
	private static string environmentInstancesPath  = BASE_PATH + "environmentInstances.xml";
	
	public static EnvironmentPrimitive[] getEnvironmentPrimitives()
	{
		return EnvironmentPrimitivesParser.parse (environmentPrimitivesPath);
	}
	
	public static EnvironmentInstance[] getEnvironmentInstances()
	{
		return EnvironmentInstancesParser.parse (environmentInstancesPath);
	}
}