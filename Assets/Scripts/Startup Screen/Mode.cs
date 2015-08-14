using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mode
{
	public string name;
	public List<Mode.Entry> supportedEnvironments;
	public List<Mode.Entry> supportedCharacters;
	
	public void setName(string name)
	{
		this.name = name;
	}
	
	public void setSupportedEnvironments(List<Mode.Entry> supportedEnvironments)
	{
		this.supportedEnvironments = supportedEnvironments;
	}
	
	public void setSupportedCharacters(List<Mode.Entry> supportedCharacters)
	{
		this.supportedCharacters = supportedCharacters;
	}
	
	public class Entry
	{
		public string name;
		public string path;
		
		public Entry(string name, string path)
		{
			this.name = name;
			this.path = path;
		}
	}
}
