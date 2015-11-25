using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;


public class CharacterPrimitivesParser
{
	private static CharacterPrimitive[] lastParsedPrimitives;
	
	public static CharacterPrimitive[] parse(string xmlFile)
	{
		if(!File.Exists(xmlFile))
		{
			Debug.LogError("Couldn't parse '" + xmlFile + "': it doesn't exist.");
			return null;
		}	
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<CharacterPrimitive>();
		
		foreach(var character in root.Elements ("character"))
		{
			var name             = character.Element ("name").Value;
			var prefab           = character.Element ("prefab").Value;
			var locomotionScript = character.Element ("locomotionScript").Value;
			
			var settingsDict = new Dictionary<string, string>();
			
			foreach(var setting in character.Element("possibleSettings").Elements("possibleSetting"))
				settingsDict.Add (setting.Element("name").Value, setting.Element ("type").Value);			
			
			returnValue.Add (new CharacterPrimitive(locomotionScript, prefab, name, settingsDict));
		}
	
		lastParsedPrimitives = returnValue.ToArray();
			
		return returnValue.ToArray();
	}	
	
	public static CharacterPrimitive[] getLastParsedPrimitives()
	{
		return lastParsedPrimitives;
	}
}

public class CharacterPrimitive
{
	public string locomotionScriptPath;
	public string prefabPath;
	public string name;
	public Dictionary<string, string> possibleSettings;
	
	public CharacterPrimitive(string locomotionPath, string prefabPath, string name, Dictionary<string, string> possibleSettings)
	{
		this.locomotionScriptPath = locomotionPath;
		this.prefabPath = prefabPath;
		this.name = name;
		this.possibleSettings = possibleSettings;
	}
}
