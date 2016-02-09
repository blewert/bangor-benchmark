using UnityEngine;
using System.Collections;

public class CameraFollow: MonoBehaviour {
	
	// The target we are following
	public  Transform target;
	// The distance in the x-z plane to the target
	public int distance = 10;
	// the height we want the camera to be above the target
	public float height = 10.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 0.6f;
	
	
	void  LateUpdate (){
		// Early out if we don't have a target
		if (true){
			if (!target)
				return;
			
			// Calculate the current rotation angles
			var wantedRotationAngle = target.eulerAngles.y;
			var wantedHeight = target.position.y + height;
			
			var currentRotationAngle = transform.eulerAngles.y;
			var currentHeight = transform.position.y;
			
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
			
			// Convert the angle into a rotation
			var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;
			Vector3 pos = transform.position;

			// Set the height of the camera
			pos.y = currentHeight;

			transform.position = pos;

			// Always look at the target
			transform.LookAt (target);
		}
	}
}