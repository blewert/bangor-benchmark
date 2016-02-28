using UnityEngine;
using System.Collections;

public class CapsuleLocomotion : ILocomotionScript
{
	private CharacterController characterController;
	public float speed;
	public float turnSpeed;
	
	public void Start()
	{
		characterController = GetComponent<CharacterController>();	
		
		speed = (float)SettingParser.getSetting(instance, "speed");
		turnSpeed = (float)SettingParser.getSetting(instance, "turnSpeed");
	}
	
	/// <summary>
	/// Applies a given turning speed to the NPC, in order to rotate it.
	/// </summary>
	/// <param name="speed">The speed/stepping to rotate the NPC by.</param>
	private void applyTurningSpeed(float speed)
	{
		//Calculate rotation around Y axis
		Quaternion newRotation = Quaternion.AngleAxis (transform.eulerAngles.y + speed, Vector3.up);
		
		//Interpolate rotation from current to calculated
		transform.rotation = Quaternion.Lerp (transform.rotation, newRotation, Time.time * Mathf.Abs (speed));
	}
	
	public override void modifyTurnRate (float rate)
	{
		turnSpeed *= rate;
	}
	
	public override void modifySpeed (float rate)
	{
		speed *= rate;
	}
	
	public override void turnLeft ()
	{
		updatePosition ();
		
		//Turn left
		applyTurningSpeed(-turnSpeed);
	}

	public override void turnRight ()
	{
		updatePosition ();
		
		//Turn right
		applyTurningSpeed(turnSpeed);
	}

	public override void moveForward ()
	{
		updatePosition ();
		
		//Move forward
		characterController.SimpleMove(transform.forward * speed);
	}

	public override void moveBackward ()
	{
		updatePosition ();
		
		//Move backward
		characterController.SimpleMove(-transform.forward * speed); 
	}

}
