using UnityEngine;
using System.Collections;

public class CapsuleLocomotion : ILocomotionScript
{
	private CharacterController characterController;
	public float speed;
	public float turnSpeed;
	private bool movingBackwards = false;
	
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
	
	public void Update()
	{
		if(Input.GetKeyUp(KeyCode.S))
			movingBackwards = false;
	}
	
	public void modifyTurnRate (float rate)
	{
		turnSpeed *= rate;
	}
	
	public void modifySpeed (float rate)
	{
		speed *= rate;
	}
	
	public override void turnLeft ()
	{
		updatePosition ();
		
		var speed = -turnSpeed;
		speed = (movingBackwards) ? (-speed) : (speed);
		
		//Turn left
		applyTurningSpeed(speed);
	}

	public override void turnRight ()
	{
		updatePosition ();
		
		var speed = turnSpeed;
		speed = (movingBackwards) ? (-speed) : (speed);
		
		//Turn right
		applyTurningSpeed(speed);
	}

	public override void moveForward ()
	{
		updatePosition ();
		
		movingBackwards = false;
		
		//forwardVel = Mathf.Lerp (forwardVel, this.speed, forwardVel / speed); 	
			
		//Move forward
		characterController.SimpleMove(transform.forward * speed);
	}

	public override void moveBackward ()
	{
		updatePosition ();
		
		movingBackwards = true;
		
		//Move backward
		characterController.SimpleMove(transform.forward * -speed);
	}

}
