using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

public static class EnvironmentInstancesParser
{
	public static EnvironmentInstance[] parse(string xmlFile)
	{
		if(!File.Exists(xmlFile))
		{
			Debug.LogError("Couldn't parse '" + xmlFile + "': it doesn't exist.");
			return null;
		}	
		
		EnvironmentPrimitive[] primitives = PrimitivesParser.getEnvironmentPrimitives();
		
		string contents = File.ReadAllText (xmlFile);
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<EnvironmentInstance>();
		
		//Parsey parsey
		foreach(var environment in root.Elements ("instance"))
		{
			//Name, primitive and settings for current instance
			var name = environment.Element ("name").Value;
			var primitive = environment.Element("primitive").Value;
			var settings = new Dictionary<string, string>();
			
			//Find linked primitive with this instance
			var linkedPrimitives = primitives.Where (x => x.name.ToLower() == primitive.ToLower ());
						
			if(linkedPrimitives.Count () <= 0)
			{
				//Something went terribly wrong!
				Debug.LogError ("Couldn't parse '" + xmlFile + "', instance '" + name + "' is not linked to any primitive.");
				return null;
			}
			
			foreach(var setting in environment.Elements("settings").Elements("setting"))
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
			
			returnValue.Add (new EnvironmentInstance(name, primitive, settings));					
		}
		
		return returnValue.ToArray();
	}
}

public class EnvironmentInstance
{
	public string name;
	public string primitiveName;
	public Dictionary<string, string> settings;
	
	public EnvironmentInstance(string name, string primitiveName, Dictionary<string, string> settings)
	{
		this.name = name;
		this.primitiveName = primitiveName;
		this.settings = settings;
	}
}