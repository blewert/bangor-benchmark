using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCHoverAI : MonoBehaviour {

	private NPCcontroler movement;
	public GameObject castdarSensor;
	public GameObject visionSensor;
	public Castdar castdar;
	public Vision vision;

	public List<float> doorsList;  

	// Collision Indicators
	private float[] collisions;
	private float visionLength;
	
	public float wallAvoidLength;
	private bool leftCheck;
	private bool rightCheck;
	private bool forwardCheck;
	private float leftDist;
	private float rightDist;
	private float forwardDist;
	private int turning;
	private int angle;
	private float ahead;

	// Use this for initialization
	void Start () {
		movement = gameObject.GetComponent<NPCcontroler> ();
		castdar = castdarSensor.GetComponent<Castdar> ();
		vision = visionSensor.GetComponent<Vision> ();


	}
	
	// Update is called once per frame
	void Update () {

		Quaternion theRotation = transform.localRotation;


		ahead = castdar.GetWalls ().Length / 2;
		//print ("AHEAD   " + ahead);
		ScannerSweep ();

		if (doorsList.Count > 0) {
			for (int i = 0; i < doorsList.Count; i++) {

				Quaternion rotation = theRotation * Quaternion.Euler (0, doorsList[i], 0);
				transform.localRotation = rotation;
				Debug.DrawLine (transform.position, transform.position + (transform.forward * 8f), Color.yellow);
				transform.localRotation = theRotation;

			}

			float dir = 0;
			float dirDifference = 0;

			if (turning == 0) {
				turning = Random.Range(1,3);
			}


			// Go towards the door closest
			if (turning == 2) {
				
				dir = castdar.GetWalls ().Length;
				dirDifference = castdar.GetWalls ().Length;

				for (int i = 0; i < doorsList.Count; i++) {
					if( FindDifference(doorsList[i], 0) < dirDifference) {
						dirDifference = FindDifference(doorsList[i], 0);
						dir = doorsList[i];
						print ("closest");
					}	
				}
			} 

			// Go towards the door the furthest away
			if (turning == 1) {
				
				dir = castdar.GetWalls ().Length;
				dirDifference = 0;
				
				for (int i = 0; i < doorsList.Count; i++) {
					if( FindDifference(doorsList[i], 0) > dirDifference) {
						dirDifference = FindDifference(doorsList[i], 0);
						dir = doorsList[i];
						print ("furthest");
					}	
				}
			}


			Quaternion rotation2 = transform.localRotation * Quaternion.Euler (0, dir, 0); 
			transform.localRotation = rotation2;
			Debug.DrawLine (transform.position, transform.position + (transform.forward * 8f), Color.red);
			transform.localRotation = theRotation;

			if (dirDifference > 3) {

				if(dir < 0) {
					movement.turnLeft ();
				} else {
					movement.turnRight ();
				}
			}

			if ( !forwardCheck ) {

				AvoidCollisions();
				movement.turnRight();
				turning = 0;
			} 

		} else {  //No Doors Seen, randomly explore
			RandomExplore();
		}

	}

	void AvoidCollisions() {
		if (leftCheck) {
			movement.turnRight ();
		}
		if (rightCheck) 
			movement.turnLeft ();
	}

	void RandomExplore() {

		if (leftCheck && rightCheck && forwardCheck) {
			if (turning == 0) {
				turning = Random.Range(1,3);
			}
			if (turning == 1)
				movement.turnRight ();
			if (turning == 2)
				movement.turnLeft ();
		}

		if (!leftCheck && !rightCheck && forwardCheck) {
			if (turning == 0) {
				turning = Random.Range(1,3);
			}
			if (turning == 1)
				movement.turnRight ();
			if (turning == 2)
				movement.turnLeft ();
		}

		// left and forward blocked
		if (leftCheck && !rightCheck && forwardCheck) {
			movement.turnRight ();
		}

		// right and forward blocked
		if (!leftCheck && rightCheck && forwardCheck) {
			movement.turnLeft ();
		}
		// Forward unblocked
		if (!forwardCheck) {
			movement.moveForward();
			turning = 0;
		}

	}

	//////////////////////////
	// SWEEP AND SCAN
	void ScannerSweep () {
		CheckForWalls ();
		FindDoors ();
	}
	// END SWEEP AND SCAN
	//////////////////////////


	//////////////////////////
	// AVOID WALLS
	// Check Left Right and Forwards
	// Store distances for all three
	// 
	void CheckForWalls () {
		collisions = castdar.GetWalls (); // Get the Wall array
		
		float sweepSegment = (collisions.Length / 8); // A segment of the sweep 
		visionLength = castdar.radarRangeWalls; // How far forward can we see
		float distCheck = visionLength;
		
		forwardCheck = false;
		rightCheck = false;
		leftCheck = false;
		leftDist = visionLength;
		rightDist = visionLength;
		forwardDist = visionLength;
		
		float lastCheck = forwardDist;
		
		for (int i = 0; i < collisions.Length; i++) {
			float check = collisions[i];
			
			if( i < (0 + sweepSegment) ) {
				if (check < wallAvoidLength) {
					leftCheck = true;
					if (check < leftDist)
						leftDist = check;
				}
			}
			
			if( i > (ahead - sweepSegment) && i < (ahead + sweepSegment) ) {
				if (check < wallAvoidLength) {
					forwardCheck = true;
					if (check < forwardDist)
						forwardDist = check;
				}
			}
			
			if( i > (collisions.Length - sweepSegment) ) {
				if (check < wallAvoidLength) {
					rightCheck = true;
					if (check < rightDist)
						rightDist = check;
				}
			}



		}
	}
	// End AVOID WALLS
	//////////////////////////
	
	//////////////////////////
	// FIND DOORS
	void FindDoors () {

		doorsList.Clear ();
		bool doorSeen = false;
		//float lastCheck = forwardDist;

		float doorcheck = 0;
		float doorcounter = 0;
		int lastCheck = 0;

		for (int i = 0; i < collisions.Length; i++) {

			if (i > 3) {
				lastCheck = i - 3;
			} 

			float check = collisions[i];
			
			if ( (collisions[lastCheck] * 1.1f) < check ) {
				//print (i);
				doorSeen = true;
				//print ("HELLO");
			}
			
			if ( doorSeen ) {
				doorcheck += i;
				doorcounter++;
			}

			if( doorSeen ) {
				if ( (check * 1.1f) < collisions[lastCheck] || i == (collisions.Length -1) ) {
					doorSeen = false;
					//print ("GOODBYE");
					float doorHere = (doorcheck / doorcounter) - ahead;
					//print (doorHere);
					if (doorcounter > 20) 
						doorsList.Add(doorHere);

					doorcheck = 0;
					doorcounter = 0;
				}
			}
		}
	}
	// End of Wall Check
	//////////////////////////
	
	
	/////////////////////////////////////////////
	/// --Helper Functions-- (PRIVATE)
	/////////////////////////////////////////////
	float FindDifference(float v1, float v2)
	{
		return Mathf.Abs(v1 - v2);
	}
}
