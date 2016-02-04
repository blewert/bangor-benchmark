using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Instance
{
	public string name;
	public string primitiveName;
	public Dictionary<string, string> settings;
	
	public Instance()
	{
	}
	
	public Instance(string name, string primitiveName, Dictionary<string, string> settings)
	{
		this.name = name;
		this.primitiveName = primitiveName;
		this.settings = settings;
	}
}
