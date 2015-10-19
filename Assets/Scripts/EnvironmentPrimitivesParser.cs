using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

public static class EnvironmentPrimitivesParser
{
	public static EnvironmentPrimitive[] parse(string xmlFile)
	{
		if(!File.Exists(xmlFile))
		{
			Debug.LogError("Couldn't parse '" + xmlFile + "': it doesn't exist.");
			return null;
		}	
		
		string contents = File.ReadAllText (xmlFile);
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<EnvironmentPrimitive>();
		
		foreach(var environment in root.Elements("environment"))
		{
			string name    = environment.Element ("name").Value;
			string script  = environment.Element ("scriptPath").Value;
			Dictionary<string, string> settings = new Dictionary<string, string>();

			var possibleSettings = environment.Element("possibleSettings").Elements ("possibleSetting");
			
			foreach(var possibleSetting in possibleSettings)
			{				
				string settingName  = possibleSetting.Element("name").Value;
				string settingType  = possibleSetting.Element("type").Value;
				
				settings.Add (settingName, settingType);
			}
			
			if(!File.Exists (script))
			{
				Debug.LogError("Couldn't parse primitive '" +  name + "' in '" + xmlFile + "' - script path doesn't exist.");
				return null;
			}
			
			returnValue.Add (new EnvironmentPrimitive(name, script, settings));
		}
		
		return returnValue.ToArray();
	}
}

public class EnvironmentPrimitive
{
	public string name;
	public string scriptPath;
	public Dictionary<string, string> possibleSettings;
	
	public EnvironmentPrimitive(string name, string scriptPath, Dictionary<string, string> possibleSettings)
	{
		this.name = name;
		this.scriptPath = scriptPath;
		this.possibleSettings = possibleSettings;
	}
}