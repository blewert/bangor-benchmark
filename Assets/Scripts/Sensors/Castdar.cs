using UnityEngine;
using System.Collections;
using System.Collections.Generic;


////////////////////////////////////////////////////////////
/// --CASTDAR (a Portmanteau of RayCAST and RaDAR)--
/// The CASTDAR throws out raycasts in two arcs based on a 
/// user defined range and number of degrees. The first set
/// of raycasts detects any objects and places them in a
/// list called "SeenObject". The second set of raycasts
/// detects walls only, and for each invidual ray places the 
/// the range to the wall in an array called "Walls". Each
/// of the raycasts can be turned off using boolien. 
////////////////////////////////////////////////////////////

public class Castdar : MonoBehaviour {

	///////////////////////////
	// Vision Cone Vairables
	/*objects*/
	public int visionAngle = 30;
	public int visionAngleWalls = 30;// We halve this to get the posative and negative angles

	/*Walls*/
	public float radarRange = 15f;
	public float radarRangeWalls = 10f;

	/*Fidelity - how often the castdar pings*/
	public float refreshObjectScan = 0.5f;
	public float refreshWallScan = 0.5f;

	///////////////////////////
	// Vision Data Containers (PRIVATE)
	private List<HitObject> seenObject = new List<HitObject>(); //  objects
	private List<HitObject> wallObjects = new List<HitObject>();
	private float[] walls; // Walls
	private int seen; // Number of objects seen

	///////////////////////////
	// Timer Switches
	public bool scanForObjects = true;
	public bool scanForWalls = true;

	// Starts the Coroutines (if enabled) 
	void Start () {
		if(scanForObjects)
			StartCoroutine("TimedVisionSweep");
		if(scanForWalls)
			StartCoroutine("TimedWallSweep");
	}

	void Update () {
		//print (seen);
	}

	/////////////////////////////////////////////
	/// --Timed Castdar Coroutines (PRIVATE)--
	/// Does a pulse at a regular interval
	/////////////////////////////////////////////

	// Timed Vision Sweep
	IEnumerator TimedVisionSweep () {
		while(scanForObjects) {
			VisionSweep ();
			yield return new WaitForSeconds(refreshObjectScan);
		}
	}

	// Timed Wall Sweep
	IEnumerator TimedWallSweep () {
		while(scanForWalls) {
			WallSweep ();
			yield return new WaitForSeconds(refreshWallScan);
		}
	}
	
	/////////////////////////////////////////////
	/// --Vision Sweep Functions (PUBLIC)--
	/// Gives external access to do a sweep on demand
	/////////////////////////////////////////////

	private int tickCount = 0;
	private long tickTotal = 0;
	
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (transform.position, transform.position + transform.TransformDirection (Vector3.forward) * 3f);
	}
	
	// Vision Sweep
	public void VisionSweep () {
		// Empty the list
		seenObject.Clear();

		// Vision should be + and - so we need to halve it. 
		float visionModifier = (visionAngle / 2);

		// We will need to reset to the center once we have done a sweep
		Quaternion resetRotation = transform.localRotation;

		// Capture All Hits
		RaycastHit[] hits;
		
		
		// Loop round + and - to create a vision cone
		visionAngle = (visionAngle > 360) ? (0) : (visionAngle);
		visionAngle = (visionAngle < 0) ? (360) : (visionAngle);
		
		for (int i = 0; i < visionAngle; i ++)
		{ 
			// Move the rotation based on I. 
			Quaternion rotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y + (i - visionModifier), transform.localRotation.z);
			transform.localRotation = rotation;
			
			// Cast a ray
			Vector3 fwd = transform.TransformDirection (Vector3.forward);
			hits = Physics.RaycastAll (transform.position, fwd, radarRange);
			
			// We have hit something... bettr check what? 
			if (hits.Length > 0) {
				for(int j = 0; j < hits.Length; j++) {
			
					// We hit a wall... Best stop there... 
					if(hits[j].transform.gameObject.tag == "Wall") {
						j = hits.Length + 1;
					} else {
							
						// We hit something and it wasnt a wall.. add it to the array. 
						HitObject hitOBJ = new HitObject(hits[j].transform.gameObject, hits[j].distance, i - visionModifier);
						
						// Check if we have already added this object... we scan with VERY high fidelity. 
						if (!HasDuplicate(hitOBJ))
							seenObject.Add(hitOBJ);
					}
				}
				
			}
			// Uncomment the next line for debug on the sweep. 
		 	Debug.DrawLine(transform.position, (transform.position + (transform.forward * radarRange)) , Color.red);
		}
		
			
		// Reset the rotation to the center. 
		transform.rotation = resetRotation;
		seen = seenObject.Count;
	}


	// Collision Sweep
	public void WallSweep () {
		// Clear the wall array
		walls = new float[visionAngleWalls];
		wallObjects.Clear ();
		
		// Vision should be + and - so we need to halve it. 
		float visionModifier = (visionAngleWalls / 2);

		// We will need to reset to the center once we have done a sweep
		Quaternion resetRotation = transform.localRotation;

		// Capture All Hits
		RaycastHit hit;
		
		// Loop round + and - to create a vision cone
		for (int i = 0; i < visionAngleWalls; i ++){

			// Move the rotation based on I. 
			Quaternion rotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y + (i - visionModifier), transform.localRotation.z);
			transform.localRotation = rotation;
			
			// Cast a ray
			Vector3 fwd = transform.TransformDirection (Vector3.forward);
			
			// We have hit something... bettr check what? 
			if (Physics.Raycast (transform.position, fwd, out hit, radarRangeWalls)) {
				
				//registers the hit
				walls[i] = hit.distance;
				
				// We hit something and it wasnt a wall.. add it to the array. 
				HitObject hitOBJ = new HitObject(hit.transform.gameObject, hit.distance, i - visionModifier);
				
				// Check if we have already added this object... we scan with VERY high fidelity. 
				if (!HasDuplicate(hitOBJ))
					wallObjects.Add(hitOBJ);
				
				
			} else {
				walls[i] = radarRangeWalls;
			}
			// Uncomment the next line for debug on the sweep. 
			Debug.DrawLine(transform.position, (transform.position + (transform.forward * radarRangeWalls)) , Color.blue);
		}

		
		// Reset the rotation to the center. 
		transform.rotation = resetRotation;
	}

	/////////////////////////////////////////////
	/// --Helper Functions-- (PRIVATE)
	/////////////////////////////////////////////

	public bool HasDuplicate(HitObject h) {
		bool check = false;
		foreach( HitObject h2 in seenObject) {
			if (h2.seenOBJ == h.seenOBJ)
				check = true;
		}
		return check;
	}

	/////////////////////////////////////////////
	/// --Get Methods-- 
	/////////////////////////////////////////////

	// Return objects seen
	public List<HitObject> GetSeenObject () {
		return seenObject;
	}
	
	// Return objects seen
	public List<HitObject> GetSeenWalls () {
		return wallObjects;
	}

	// Return number of objects seen
	public int GetSeen () {
		return seen;
	}

	// Return the wall hit array
	public float[] GetWalls () {
		return walls;
	}
	
	/////////////////////////////////////////////
	/// --HitObject Class-- 
	/// Stores data about the objects that have been detected
	/////////////////////////////////////////////

	public class HitObject {

		//Values for the hitObject
		public GameObject seenOBJ; // What has been seen
		public float distance; // Distance too the object
		public float angle; // The angle it was seen at
		
		// HitObject Constructor
		public HitObject( GameObject seen, float dist, float ang)
		{
			seenOBJ = seen;
			distance = dist;
			angle = ang;
		}
	}



}
