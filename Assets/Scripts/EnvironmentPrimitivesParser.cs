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
			string name = "";
			string script = "";
			
			foreach(var node in environment.Elements())
			{
				if(node.Name == "name")
					name = node.Value;
					
				else if(node.Name == "scriptPath")
					script = node.Value;
			}
			
			if(!File.Exists (script))
			{
				Debug.LogError("Couldn't parse primitive '" +  name + "' in '" + xmlFile + "' - script path doesn't exist.");
				return null;
			}
			
			returnValue.Add (new EnvironmentPrimitive(name, script));
		}
		
		return returnValue.ToArray();
	}
}

public class EnvironmentPrimitive
{
	public string name;
	public string scriptPath;
	
	public EnvironmentPrimitive(string name, string scriptPath)
	{
		this.name = name;
		this.scriptPath = scriptPath;
	}
}