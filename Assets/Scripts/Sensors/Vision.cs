using UnityEngine;
using System.Collections;
using System.Collections.Generic;

////////////////////////////////////////////////////////////
/// --VISION--
/// The vision sensor is designed to imitate a basic computer
/// vision system, using a camera. It does this by using a 
/// sphere colider, detecting all game objects which enter it. 
/// As soon as a game object enters the colider, if the angle
/// too the game object is less than the vision angle limit
/// a ray is cast to check for occlusion. 
/// 
/// NOTE: for this sensor to work all game objects must have 
/// a riged body. 
////////////////////////////////////////////////////////////

public class Vision : MonoBehaviour {

	///////////////////////////
	// Vision Data Containers
	private List<GameObject> detected = new List<GameObject>();
	private List<GameObject> seenObject = new List<GameObject>();
	private int seen;

	///////////////////////////
	// Vision Cone Varables
	public float visionAngle = 40f;
	private float actualVisionAngle;
	public float visionRange = 5f;
	public SphereCollider visionCollider;


	/////////////////////////////////////////////
	/// --Update (PRIVATE)--
	/// Looks through the detected list and checks
	/// if any detected game objects can be seen.
	/////////////////////////////////////////////

	void Update () {
		// Reset Seen to 0
		seenObject.Clear ();

		// ActualVisionAngle is half vision angle + and - from center
		actualVisionAngle = visionAngle / 2; 

		// Adjusts the range by changing the radius of the collider
		visionCollider.radius = visionRange;

		// if the detected list is larger than 0 check the objects
		if (detected.Count > 0) {

			// Loop through the list 
			for(int i = 0; i < detected.Count; i++) {

				if(detected[i] == null)
					continue;
					
				Vector3 targetDir = detected[i].transform.position - transform.position;

				// Check the angle between the sensor and the game object
				float angle = Vector3.Angle(targetDir, transform.forward);

				//if its within our vision angle 
				if ( angle < actualVisionAngle) {

					// Dont check if it is either a wall or a floor
					if (detected[i].tag != "Wall" && detected[i].tag != "Floor") {

						// Check we can see it using the HitOcclusion function
						HitOcclusion(detected[i]);
					}
				}
			}
		}

		seen = seenObject.Count;
	}
	
	/////////////////////////////////////////////
	/// --HitOcclusion (PRIVATE)--
	/// Turns towards the object and casts a ray
	/////////////////////////////////////////////

	void HitOcclusion(GameObject target) {

		// Store the rotation so we can reset. 
		Quaternion localRotationReset = transform.localRotation;

		// Store the hit data
		RaycastHit hit;

		// Turn towards the target
		transform.LookAt(target.transform);

		// Cast a ray the length of the visionRange
		if (Physics.Raycast (transform.position, transform.forward, out hit, visionRange)) {
			if (hit.transform.gameObject.tag != "Wall" && hit.transform.gameObject.tag != "Floor" && hit.transform.gameObject.tag != "Projectile") {
				
				// Add the object to the accessable list
				seenObject.Add(hit.transform.gameObject);

				// Uncomment the next line for debug
				//Debug.DrawLine (transform.position, hit.point, Color.green);
			}
		}

		// Reset the rotation
		transform.localRotation = localRotationReset;
	}

	/////////////////////////////////////////////
	/// --Get Methods-- 
	/////////////////////////////////////////////
	
	public int GetSeen() {
		return seenObject.Count;
	}
	
	public List<GameObject> GetSeenObject() {
		return seenObject;
	}

	
	/////////////////////////////////////////////
	/// --Set Methods-- 
	/////////////////////////////////////////////

	public void AddDetected (Collider other) {
		if(!detected.Contains(other.gameObject)) {
			detected.Add(other.gameObject);
		}
	}
	
	public void RemoveDetected (Collider other) {
		if(detected.Contains(other.gameObject)) {
			detected.Remove(other.gameObject);
		}
	}

}
