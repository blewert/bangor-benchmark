using UnityEngine;
using System.Collections;

public class track : MonoBehaviour 
{
	public float trackSpeed = 0.2f; 

		// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach(Transform child in transform) 
		{
			var rend = child.GetComponent<Renderer>();
			rend.material.SetTextureOffset("_MainTex", new Vector2(Time.time * trackSpeed, 0));
		}
	}
}
