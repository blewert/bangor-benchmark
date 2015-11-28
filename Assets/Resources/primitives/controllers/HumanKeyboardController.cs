using UnityEngine;
using System.Collections;

public class HumanKeyboardController : MonoBehaviour 
{
	private ILocomotionScript locomotionScript;
	
	public void Start()
	{
		locomotionScript = GetComponent<ILocomotionScript>();	
	}
	
	public void Update()
	{
		if(Input.GetKey(KeyCode.W))
			locomotionScript.moveForward();
		
		else if(Input.GetKey (KeyCode.S))
			locomotionScript.moveBackward();
			
		if(Input.GetKey (KeyCode.A))
			locomotionScript.turnLeft();
			
		else if(Input.GetKey (KeyCode.D))
			locomotionScript.turnRight();
			
	}
}
