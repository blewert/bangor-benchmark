using UnityEngine;
using System.Collections;
using System.Linq;

public class Bootup : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		var primitives = PrimitivesParser.getEnvironmentPrimitives();
		var instances = PrimitivesParser.getEnvironmentInstances();
		
		foreach(var primitive in primitives)
		{
			EnvironmentInstance[] linkedInstances = instances.Select(x => x.name == primitive.name);
			
			foreach(var instance in linkedInstances)
			{
				Debug.Log("Instance linked to " + primitive.name + " -> " + instance.name);
			}
		}
	}

}
