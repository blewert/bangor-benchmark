using UnityEngine;
using System.Collections;

public class NPCKeyboardController : MonoBehaviour 
{
	//public GameObject characterObject;
	private INPC characterScript;
	private IMortar attachedMortar;

	// Use this for initialization
	void Start () 
	{
		characterScript = GetComponent<INPC>();
		attachedMortar = GetComponentInChildren<IMortar>();
		
		Debug.Log(characterScript);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.W))
			characterScript.moveForward();
			
		else if(Input.GetKey(KeyCode.S))
			characterScript.moveBackward();
			
		if(Input.GetKey(KeyCode.A))
			characterScript.turnLeft();
			
		else if(Input.GetKey(KeyCode.D))
			characterScript.turnRight();

		if(characterScript is IFlyableNPC)
		{
			IFlyableNPC flyableScript = characterScript as IFlyableNPC;
			
			if(Input.GetKey (KeyCode.LeftShift))
				flyableScript.ascend();
				
			else if(Input.GetKey (KeyCode.LeftControl))
				flyableScript.descend ();
				
			if(Input.GetKey (KeyCode.Q))
				flyableScript.strafeLeft();
				
			else if(Input.GetKey (KeyCode.E))
				flyableScript.strafeRight ();
		}
		
		if(Input.GetKeyUp (KeyCode.Space))
		{
			if(attachedMortar != null)
				attachedMortar.Fire ();
		}
	}
}
