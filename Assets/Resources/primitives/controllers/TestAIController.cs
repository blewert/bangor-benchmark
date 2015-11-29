using UnityEngine;
using System.Collections;
using System.Linq;

public class TestAIController : MonoBehaviour
{
	private ILocomotionScript locomotionScript;
	private Castdar castdar;
	
	// Use this for initialization
	void Start () 
	{
		locomotionScript = GetComponent<ILocomotionScript>();
		castdar = GetComponentInChildren<Castdar>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		locomotionScript.moveForward();
		
		if(castdar.GetSeen() > 0)
		{
			//Get hit objects
			var hitObjects = castdar.GetSeenObject();
			
			if(hitObjects.Any (x => x.seenOBJ.tag.Equals("Prop")))
			{
				//Has hit a tree
				locomotionScript.turnRight ();
			}
			else if(hitObjects.Any (x => x.seenOBJ.getTeam () != this.gameObject.getTeam () && x.seenOBJ.getTeam () != "None"))
			{
				//Has hit an enemy
				locomotionScript.moveBackward();
			}
		}
	}
}
