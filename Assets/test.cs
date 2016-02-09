using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (target.transform.position);
		Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
	}
}
