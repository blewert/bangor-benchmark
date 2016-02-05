using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraFollowCharacter : MonoBehaviour
{
	public GameObject target;
	public List<GameObject> targets;
	
	public int targetIdx = default(int);
	
	private Camera camera = Camera.main;
	
	public float height = 3f;
	public float dist = 9.92f;
	public float lerpSpeed = 0.1f;
	
	public void LateUpdate()
	{		
		if(targets != null)
		{
			targets.RemoveAll(x => x == null);
			
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				targetIdx = (targetIdx - 1) % (targets.Count);
			   
				if(targetIdx < 0)
					targetIdx = targets.Count - 1;
			}
			   
			else if(Input.GetKeyDown(KeyCode.RightArrow))
			   targetIdx = (targetIdx + 1) % (targets.Count);
			
			target = targets[targetIdx];	
		}
		
		var targetPos = getOffsetPosition();
		
		camera.transform.position = Vector3.Lerp (camera.transform.position, targetPos, lerpSpeed);
		camera.transform.LookAt(target.transform.position);
	}
	
	public Vector3 getOffsetPosition()
	{
		Vector3 targetPos = target.transform.position;

		var zExtent = target.GetComponent<MeshFilter>().mesh.bounds.extents.z;

		targetPos = target.transform.TransformPoint(Vector3.forward * -Mathf.Abs (dist) * zExtent);
		targetPos.y += height;
		//targetPos = target.transform.position - (target.transform.forward * zExtent * Mathf.Abs (dist));
		//targetPos.y += height;
		
		return targetPos;
	}
}
