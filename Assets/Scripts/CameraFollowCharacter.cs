using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollowCharacter : MonoBehaviour
{
	public GameObject target;
	private Camera camera = Camera.main;
	
	public float height = 3f;
	public float dist = 9.92f;
	public float lerpSpeed = 0.1f;
	
	public void Update()
	{		
		var targetPos = getOffsetPosition();
		
		camera.transform.position = Vector3.Lerp (camera.transform.position, targetPos, lerpSpeed);
		camera.transform.LookAt(target.transform.position);
	}
	
	public Vector3 getOffsetPosition()
	{
		Vector3 targetPos = target.transform.position;
	
		Debug.Log (targetPos);
		
		var zExtent = target.GetComponent<MeshFilter>().mesh.bounds.extents.z;
		
		targetPos = target.transform.position - (target.transform.forward * zExtent * Mathf.Abs (dist));
		targetPos.y += height;
		
		
		
		return targetPos;
	}
}
