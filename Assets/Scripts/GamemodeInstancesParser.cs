using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

public static class GamemodeInstancesParser
{
	public static GamemodeInstance[] parse(string xmlFile)
	{
		if(!File.Exists(xmlFile))
		{
			Debug.LogError("Couldn't parse '" + xmlFile + "': it doesn't exist.");
			return null;
		}	
		
		GamemodePrimitive[] primitives = PrimitivesParser.getGamemodePrimitives();
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<GamemodeInstance>();
		
		//Parsey parsey
		foreach(var gamemode in root.Elements ("instance"))
		{
			//Name, primitive and settings for current instance
			var name = gamemode.Element ("name").Value;
			var primitive = gamemode.Element("primitive").Value;
			var settings = new Dictionary<string, string>();
			
			//Find linked primitive with this instance
			var linkedPrimitives = primitives.Where (x => x.name.ToLower() == primitive.ToLower ());
			
			if(linkedPrimitives.Count () <= 0)
			{
				//Something went terribly wrong!
				Debug.LogError ("Couldn't parse '" + xmlFile + "', instance '" + name + "' is not linked to any primitive.");
				return null;
			}
			
			foreach(var setting in gamemode.Elements("settings").Elements("setting"))
			{
				var sname = setting.Element("name").Value;
				var value = setting.Element("value").Value;
				
				if(!linkedPrimitives.First ().possibleSettings.ContainsKey (sname))
				{
					//No such possible setting
					DebugLogger.Log ("Couldn't parse '" + xmlFile + "', no possible setting name '" + sname + "' found in primitive '" + primitive + "' for instance '" + name + "'.");
					return null;
				}
				
				settings[sname] = value;
			}
			
			returnValue.Add (new GamemodeInstance(name, primitive, settings));					
		}
		
		return returnValue.ToArray();
	}
}

public class GamemodeInstance : Instance
{
	public GamemodeInstance(string name, string primitiveName, Dictionary<string, string> settings) : base(name, primitiveName, settings)
	{
	}
}