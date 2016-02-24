using UnityEngine;
using System.Collections;

public class HumansController : ILocomotionScript {
	
	public float force = 1; 
	public float turningSpeed = 2;
	private Rigidbody playerRigidbody;
	private float slowDown = 0.05f;

	// Use this for initialization
	void Start () {
		playerRigidbody = GetComponent<Rigidbody>();
	}

	public override void turnLeft(){
		updatePosition ();
		transform.Rotate (0,-turningSpeed,0);
	}

	public override void turnRight(){
		updatePosition ();
		transform.Rotate (0,turningSpeed,0);
	}

	public override void moveForward(){
		updatePosition ();
		applyMovement (force);
	}

	public override void moveBackward(){
		updatePosition ();
		applyMovement (-force);
	}

	private void applyMovement(float mforce)
	{
		updatePosition ();
		transform.position = transform.position + (mforce * transform.forward*slowDown);
	}
}
