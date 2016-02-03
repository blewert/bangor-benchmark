using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;


public class CharacterInstancesParser
{
	public static CharacterInstance[] parse(string xmlFile)
	{
		if(!File.Exists(xmlFile))
		{
			Debug.LogError("Couldn't parse '" + xmlFile + "': it doesn't exist.");
			return null;
		}	
		
		var primitives = PrimitivesParser.getCharacterPrimitives();
		
		XElement root = XElement.Load (xmlFile);
		
		var returnValue = new List<CharacterInstance>();
		
		foreach(var instance in root.Elements ("instance"))
		{
			var name = instance.Element("name").Value;
			var primitiveName = instance.Element("primitive").Value;
			var controllerScript = instance.Element ("controllerScript").Value;
			
			var settingsDict = new Dictionary<string, string>();
			
			foreach(var setting in instance.Element ("settings").Elements ("setting"))
				settingsDict.Add (setting.Element("name").Value, setting.Element ("value").Value);
				
			var inst = new CharacterInstance(name, primitiveName, controllerScript, settingsDict);
			inst.primitive = primitives.Where (x => x.name == primitiveName).FirstOrDefault();
			
			returnValue.Add (inst);
		}
		
		return returnValue.ToArray();
	}
}

[System.Serializable]
public class CharacterInstance : Instance
{
	public string controllerScript;
	public CharacterPrimitive primitive;
	
	public CharacterInstance(string name, string primitiveName, string controllerScript, Dictionary<string, string> settings) : base(name, primitiveName, settings)
	{
		this.controllerScript = controllerScript;
	}
}
