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
		
		string contents = File.ReadAllText (xmlFile);
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<EnvironmentInstance>();
		
		//Parsey parsey
		foreach(var environment in root.Elements ("instance"))
		{
			var settings = new Dictionary<string, string>();
			var name = "";
			var primitive = "";
			
			foreach(var settinga in environment.Elements("settings"))
			{
				foreach(var setting in settinga.Elements())
				{
					//setting
					var sname = setting.Element("name").Value;
					var value = setting.Element("value").Value;
					
					settings[sname] = value;
				}
			}
			
			name = environment.Element ("name").Value;
			primitive = environment.Element("primitive").Value;
			
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