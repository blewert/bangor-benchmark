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
			var linkedInstances = instances.Where (x => primitive.name == x.primitiveName);
			
			foreach(var instance in linkedInstances)
			{
				Debug.Log("Instance linked to " + primitive.name + " -> " + instance.name);
			}
		}
		
		var environment = gameObject.AddComponent<ForestPrimitive>();
		environment.instance = instances[0]; //change to selected value
		
	}

}
