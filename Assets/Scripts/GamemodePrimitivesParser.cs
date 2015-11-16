using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

/// <summary>
/// Parses the gamemodePrimitives.xml file.
/// </summary>
public static class GamemodePrimitivesParser
{
	private static GamemodePrimitive[] lastParsedPrimitives;
	
	public static GamemodePrimitive[] parse(string xmlFile)
	{
		if(!File.Exists(xmlFile))
		{
			Debug.LogError("Couldn't parse '" + xmlFile + "': it doesn't exist.");
			return null;
		}	
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<GamemodePrimitive>();
		
		foreach(var gamemode in root.Elements("mode"))
		{
			//Get every gamemode, parse each one-by-one
			GamemodePrimitive gamemodeObject;
			gamemodeObject = new GamemodePrimitive(gamemode.Element("name").Value, gamemode.Element("scriptPath").Value);
			
			/*if(!File.Exists (gamemode.Element("scriptPath").Value))
			{
				throw new UnityException("Cannot parse '" + xmlFile + "', script path in xml file does not exist.");
				return null;
			}*/
			
			//Get possible exclusions
			List<string> characterExclusions = new List<string>();
			List<string> environmentExclusions = new List<string>();
			
			//Add each character exclusion
			foreach(var exclusion in gamemode.Element ("characterExclusions").Elements("exclusion"))
				characterExclusions.Add (exclusion.Value);
				
			//Add each environment exclusion
			foreach(var exclusion in gamemode.Element ("environmentExclusions").Elements("exclusion"))
				environmentExclusions.Add (exclusion.Value);
				
			//Get possible settings
			Dictionary<string, string> possibleSettings = new Dictionary<string, string>();
			
			foreach(var possibleSetting in gamemode.Element("possibleSettings").Elements ("possibleSetting"))
				possibleSettings[possibleSetting.Element ("name").Value] = possibleSetting.Element("type").Value;
				
			//Set up gamemode object before adding it in
			gamemodeObject.characterExclusions = characterExclusions.ToArray();
			gamemodeObject.environmentExclusions = environmentExclusions.ToArray();
			gamemodeObject.possibleSettings = possibleSettings;
			
			//Add it in
			returnValue.Add (gamemodeObject);
		}
		
		lastParsedPrimitives = returnValue.ToArray();
		
		//Return an array of gamemode primitive objects
		return returnValue.ToArray();
	}
	
	public static GamemodePrimitive[] getLastPrimitives()
	{
		return lastParsedPrimitives;
	}
}

/// <summary>
/// A class to represent the gamemode primitives, and the possible settings that an instance can pass.
/// </summary>
public class GamemodePrimitive
{
	//The name and script path for the gamemode primitive.
	public string name;
	public string scriptPath;
	
	//Possible additional settings, character exclusions (from the gametype), and environmental
	//exclusions.
	public Dictionary<string, string> possibleSettings;
	public string[] characterExclusions;
	public string[] environmentExclusions;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GamemodePrimitive"/> class, with a given name and script path.
	/// </summary>
	/// <param name="name">The name of the primitive.</param>
	/// <param name="scriptPath">The path to the primitive's script, for instances to pass settings into.</param>
	public GamemodePrimitive(string name, string scriptPath)
	{
		this.name = name;
		this.scriptPath = scriptPath;
	}
}
