﻿using UnityEngine;
using System.Collections;

public class HumansController : ILocomotionScript {
	
	public float force = 1; 
	public float turningSpeed = 5;
	private Rigidbody playerRigidbody;
	private float slowDown = 0.05f;

	// Use this for initialization
	void Start () {
		playerRigidbody = GetComponent<Rigidbody>();
	}

	public override void turnLeft(){
		transform.Rotate (0,-turningSpeed,0);
	}

	public override void turnRight(){
		transform.Rotate (0,turningSpeed,0);
	}

	public override void moveForward(){
		applyMovement (force);
	}

	public override void moveBackward(){
		applyMovement (-force);
	}

	private void applyMovement(float mforce)
	{
		transform.position = transform.position + (mforce * transform.forward*slowDown);
	}
}
