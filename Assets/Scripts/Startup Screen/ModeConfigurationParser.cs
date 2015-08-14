using UnityEngine;
using System.IO;
using System.Collections;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;



public class ModeConfigurationParser : MonoBehaviour 
{
	public string pathToXMLFile; 
	private List<Mode> modes = new List<Mode>();
	
	// Use this for initialization
	void Start ()
	{
		if(!File.Exists(pathToXMLFile))
		{
			Debug.LogError("XML file not found: " + pathToXMLFile);
			return;
		}
		
		string contents = File.ReadAllText(pathToXMLFile);
		parseXML(contents);
	}
	
	
	private void parseXML(string contents)
	{
		XElement root = XElement.Load (pathToXMLFile);
		
		foreach(var mode in root.Elements ("mode"))
		{
			var gamemode = new Mode();
			
			foreach(var elem in mode.Elements ())
			{
				if(elem.Name == "name")
					gamemode.setName(elem.Value);
				
				else if(elem.Name == "supportedEnvironments")
				{
					List<Mode.Entry> environments = new List<Mode.Entry>();
					
					foreach(var environment in elem.Elements("environment"))
					{
						string name = environment.Element("name").Value;
						string path = environment.Element("path").Value;
						
						environments.Add (new Mode.Entry(name, path));
					}
					
					gamemode.setSupportedEnvironments(environments);	
				}
				
				else if(elem.Name == "supportedCharacters")
				{
					List<Mode.Entry> characters = new List<Mode.Entry>();
					
					foreach(var character in elem.Elements("character"))
					{
						string name = character.Element("name").Value;
						string path = character.Element("path").Value;
						
						characters.Add (new Mode.Entry(name, path));
					}
					
					gamemode.setSupportedCharacters(characters);	
				}
			}
			
			modes.Add (gamemode);
		}
		
		foreach(var env in modes[0].supportedEnvironments)
		{
			print (env.name + " -> "+ env.path);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
