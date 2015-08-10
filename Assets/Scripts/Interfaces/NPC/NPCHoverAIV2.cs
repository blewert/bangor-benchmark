using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCHoverAIV2 : MonoBehaviour {

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
	public float bumpAvoidLength;
	private bool leftCheck;
	private bool rightCheck;
	private bool forwardCheck;
	private float leftDist;
	private float rightDist;
	private float forwardDist;
	private int turning;
	private int angle;
	private int turncheck = 0;
	private int exploreTurning = 0;
	private float ahead;
	private bool doorSelected;
	public bool debug;
	private float dir = 0;
	private float dirDifference = 0;

	// Use this for initialization
	void Start () {
		movement = gameObject.GetComponent<NPCcontroler> ();
		castdar = castdarSensor.GetComponent<Castdar> ();
		vision = visionSensor.GetComponent<Vision> ();


	}
	
	// Update is called once per frame
	void Update () {

		ScannerSweep (); // CHECK THE SENSORS

		AvoidCollisions (); // DONT HIT THE WALLS 


		if(!forwardCheck) {
			if (DoorSeen ()) { // CAN WE SEE A DOOR
				if (!doorSelected) { // WE HAVE SEEN A DOOR, BUT NOT SELECTED ONE
					SelectDoor (); // SELECT A DOOR
				}
				TurnToDoor (); // TURN TOWARDS THE SELECTED DOOR

			} else {
				RandomlyExplore(); // RANDOMLY EXPLORE THE ENVIRONMENT
			}

				

			if (!forwardCheck) {
				movement.moveForward();
			} else {
				if(!doorSelected)
					movement.turnRight();
			}
		} else {
			RandomlyExplore(); // RANDOMLY EXPLORE THE ENVIRONMENT
		}

	}

	//////////////////////////////////////////
	/// Stops GNPC from hitting walls
	//////////////////////////////////////////
	void AvoidCollisions() {
		if (leftCheck) 
			movement.turnRight();
		if (rightCheck) 
			movement.turnLeft();
	}
	
	//////////////////////////////////////////
	/// Just checks if the doorlist has entries
	/// Draws debug lines if required
	//////////////////////////////////////////
	bool DoorSeen() {

		// A reset rotation
		Quaternion theRotation = transform.localRotation;

		// Make sure we know the forward angle
		ahead = castdar.GetWalls ().Length / 2;

		// We have scanned for doors... check if we have seen them
		if (doorsList.Count > 0) {
			if(debug)
				for (int i = 0; i < doorsList.Count; i++) {

					Quaternion rotation = theRotation * Quaternion.Euler (0, doorsList[i], 0);
					transform.localRotation = rotation;
					Debug.DrawLine (transform.position, transform.position + (transform.forward * 8f), Color.yellow);
					transform.localRotation = theRotation;
				}
			return true;
		} else {  
			return false;
		}

	}

	void SelectDoor () {

		if (turning == 0) {
			turning = Random.Range(1,3);
			turncheck = 0; 
		}

		// Go towards the door closest
		if (turning == 2) {
			
			dir = castdar.GetWalls ().Length;
			dirDifference = castdar.GetWalls ().Length;
			
			for (int i = 0; i < doorsList.Count; i++) {
				if( FindDifference(doorsList[i], 0) < dirDifference) {
					dirDifference = FindDifference(doorsList[i], 0);
					dir = doorsList[i];
					angle = i;
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
					angle = i;
				}	
			}
		}

		doorSelected = true;
	}

	void TurnToDoor() {

		if(doorsList.Count > angle) {

			dirDifference = FindDifference(doorsList[angle], 0); 
			dir = doorsList[angle];

			if (dirDifference > 3) {

				if(dir < 0) {
					movement.turnLeft();
				} else {
					movement.turnRight();
				}
			} else {
				turncheck++;
				if (turncheck > 20) {
					turncheck = 0;
					turning = 0;
					doorSelected = false;
				}
			}
		} else {
			turning = 0;
			doorSelected = false;
		}
	}

	void RandomlyExplore() {

		if (Random.Range(0,30) < 2) 
			exploreTurning = Random.Range(1,3);

		if (exploreTurning == 1) 
			movement.turnLeft();

		if (exploreTurning == 2)
			movement.turnRight();
		
		if (exploreTurning != 0) 
			turncheck++;

		if (turncheck == 8) { 
			turncheck = 0;
			exploreTurning = 0;
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
				if (check < bumpAvoidLength) {
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
					if (doorcounter > 5 && doorcounter < 90) 
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
