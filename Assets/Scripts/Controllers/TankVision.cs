using UnityEngine;
using System.Collections;

public class TankVision : MonoBehaviour {

	public GameObject target;
	public int visionAngle = 30;
	public int visionDistance = 20;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 forward = transform.forward;
		Vector3 tPosition = target.transform.position;
		Vector3 position = transform.position;
		// set the ys to 0 as these mess with the calculations
		forward.y = tPosition.y = position.y = 0;
		//float angleDot = Vector3.Dot (forward.normalized, (tPosition - position).normalized); 
		float angle = Vector3.Angle (forward, tPosition - position);
		float distance = Vector3.Distance (forward, tPosition - position);

		if (angle < visionAngle && distance < visionDistance) {

			target.GetComponent<Renderer> ().material.color = Color.red;
		} else {
			target.GetComponent<Renderer> ().material.color = Color.green;
		}
	}
}
