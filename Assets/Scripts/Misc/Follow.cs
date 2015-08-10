using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour 
{
	public GameObject target;
	
	
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 offsets = new Vector3(1f, 3f, 2f);
		/*Vector3 newPosition = target.transform.TransformPoint(offsets);
		
		Vector3 clampedPos = new Vector3(newPosition.x, target.transform.position.y + offsets.y, newPosition.z);
		*/
		//transform.position = Vector3.Lerp (transform.position, clampedPos, Time.deltaTime);
		
		transform.position = target.transform.position + offsets;
		
		transform.LookAt(target.transform.position);
		
	}
}
