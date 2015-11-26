using UnityEngine;
using System.Collections;

public class CapsuleLocomotion : INPC
{
	private CharacterController characterController;
	
	public void Start()
	{
		characterController = GetComponent<CharacterController>();
		Debug.Log ("bing bong!");
	}
	
	public override void turnLeft ()
	{
		//Turn left
	}

	public override void turnRight ()
	{
		//Turn right
	}

	public override void moveForward ()
	{
		//Move forward
	}

	public override void moveBackward ()
	{
		//Move backward
	}

}
