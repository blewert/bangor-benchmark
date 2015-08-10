using UnityEngine;
using System.Collections;

[AddComponentMenu("Bangor Benchmark/Sensors/Castdar")]
public class CastdarTrig : MonoBehaviour 
{
	[Range(0.01f, 15.0f)]
	public float raySparsity = 1.5f;
	public float rayRange    = 20f;
	
	[Range(0.0f, 180.0f)]
	public float innerAngle  = 40f;
	
	[Range(-180.0f, 180.0f)]
	public float visionOffset = 0f;
	public Vector3 relativePosition = Vector3.zero;
	public Vector3 relativeOrigin = Vector3.zero;
	
	// Use this for initialization
	void Start () 
	{
		//InvokeRepeating("Cast", 20, 500);
	}
	
	private void Update()
	{
		innerAngle = Mathf.Clamp (innerAngle, 0f, 180f);
		raySparsity = Mathf.Max (0.01f, raySparsity);
		
		Vector3 origin = transform.TransformPoint(relativePosition + relativeOrigin);
		
		for(float t = -innerAngle; t < innerAngle; t += raySparsity)
		{
			Quaternion angle = Quaternion.AngleAxis (t + visionOffset, Vector3.up);
			
			Ray toCast = new Ray(origin, angle * transform.forward);
			
			Debug.DrawLine(toCast.origin, transform.TransformPoint(relativePosition + toCast.direction * rayRange));
		}
	}
}
