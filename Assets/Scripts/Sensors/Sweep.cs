using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////
/// --SWEEP--
/// The sweep sensor swings back and forth between 2 angles  
/// each update based on a user defined vision cone. This 
/// should be started as a coroutine and left to run, however
/// this can be disabled (bool autoSweep) and triggered 
/// manually from another function.
////////////////////////////////////////////////////////////

public class Sweep : MonoBehaviour 
{
	///////////////////////////
	// Sensor Sweep Rotation Vairables
	public float rotationSpeed = 100f;
	public float rotationLimit = 30f;  // RotationLimit + and - this rotation angle
	public float sweepRange = 20f;
	private float currentRotation = 0f;
	private float timer = 0f;

	///////////////////////////
	// Sweep Data Containers
	private bool seen = false;
	private GameObject seenObject;

	///////////////////////////
	// AutoSweep Switch
	private bool autoSweep = true;

	// Starts the Coroutines (if enabled) 
	void Start () {
		if(autoSweep)
			StartCoroutine("AutoSweep");
	}

	/////////////////////////////////////////////
	/// --Sweep Coroutines (PRIVATE)--
	/// Does a sweep each update
	/////////////////////////////////////////////
	 
	IEnumerator AutoSweep() {
		while(autoSweep) {
			RunSweep ();
			yield return null;
		}
	}
	
	/////////////////////////////////////////////
	/// --Sweep Function (PUBLIC)--
	/// Does a sweep each update
	/////////////////////////////////////////////

	public void RunSweep () {

		// Store all the hit data
		RaycastHit hit;

		// Increment a timer
		timer += Time.deltaTime;

		// Swing the sensor back and forth based on the rotationLimit and the rotationSpeed
		currentRotation = Mathf.PingPong (timer * rotationSpeed, rotationLimit * 2f) - rotationLimit;

		// Create a temporary variable to store the new rotation
		Quaternion rotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y + currentRotation, transform.localRotation.z);

		// Set the rotation to the temp var
		transform.localRotation = rotation;

		// Set the raycast direction
		Vector3 fwd = transform.TransformDirection(Vector3.forward);

		// If the raycast makes a hit add the object seen to a public var
		if (Physics.Raycast (transform.position, fwd, out hit, sweepRange)) {
			if (hit.transform.gameObject.tag != "Wall") {
				seenObject = hit.transform.gameObject;
				// Uncomment the next line for debug
				// Debug.DrawLine(transform.position, hit.point, Color.red);
				seen = true;
			} else {
				seen = false;
			}
		} else {
			seen = false;
		}		
	}
	
	/////////////////////////////////////////////
	/// --Get Methods-- 
	/////////////////////////////////////////////

	public bool GetSeen() {
		return seen;
	}
	
	public GameObject GetSeenObject() {
		return seenObject;
	}

}
