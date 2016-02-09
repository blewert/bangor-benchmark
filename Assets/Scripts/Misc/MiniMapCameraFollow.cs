using UnityEngine;
using System.Collections;

public class MiniMapCameraFollow : MonoBehaviour {
	public Transform Target;
	
	void LateUpdate () {
		transform.position = new Vector3 (Target.position.x, transform.position.y, Target.position.z);
		transform.rotation = Quaternion.Euler(90, 0, Target.rotation.eulerAngles.y);
	}
}
