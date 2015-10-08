using UnityEngine;
using System.Collections;

/// <summary>
/// Capsule NPC, a simple capsule-based player with primitive movements.
/// </summary>
public class TankController : ITankNPC
{
	//The controller to use for moving
	private CharacterController controller;
	
	//Rotation and movement speed
	public float angleStep = 1f;
	public float movementSpeed = 3f;
	public GameObject turret;
	
	//The target speed to lerp to, and the current velocity
	private float movementTarget = 0f;
	private float movementVelocity = 0f;
	
	/// <summary>
	/// Called when this script is first ran.
	/// </summary>
	void Start () 
	{
		//Grab the controller from the capsule
		this.controller = GetComponent<CharacterController>();
	}
	
	/// <summary>
	/// Implemented turnLeft() from interface, turns the capsule NPC left.
	/// </summary>
	public override void turnLeft()
	{
		applyTurningSpeed(-angleStep);
	}
	
	/// <summary>
	/// Implemented turnRight() from interface, turns the capsule NPC right.
	/// </summary>
	public override void turnRight()
	{
		applyTurningSpeed(angleStep);
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
	
	/// <summary>
	/// Called continously throughout the lifetime of this script.
	/// </summary>
	public void Update()
	{
		//Calculate smooth movement from current velocity to movement target
		float movement = Mathf.SmoothDamp (movementVelocity, movementTarget, ref movementVelocity, 1.5f);
		
		//Apply the movement and reset the target (for smooth stopping)
		controller.SimpleMove (transform.forward * movement);
		movementTarget = 0f;
	}
	
	/// <summary>
	/// Moves the NPC forwards.
	/// </summary>
	public override void moveForward()
	{
		movementTarget = movementSpeed;		
	}
	
	/// <summary>
	/// Moves the NPC backwards.
	/// </summary>
	public override void moveBackward()
	{
		movementTarget = -movementSpeed;
	}

	public override void TurnTurretLeft()
	{
		turret.transform.Rotate(new Vector3(0,5,0));
	}

	public override void TurnTurretRight()
	{
		turret.transform.Rotate(new Vector3(0,-5,0));
	}
}
