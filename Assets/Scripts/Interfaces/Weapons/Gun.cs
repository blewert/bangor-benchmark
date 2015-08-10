using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour 
{
	public float shotDistance = 15f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.DrawLine (transform.position, transform.position + transform.up * -shotDistance);	
		Debug.DrawRay(transform.position, -transform.up * shotDistance);
	}

	public RaycastHit[] Fire()
	{
		//Debug.DrawRay (transform.position, -transform.up * shotDistance, Color.yellow);
		return Physics.RaycastAll(transform.position, -transform.up, shotDistance);
	}
}
