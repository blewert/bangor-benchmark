using UnityEngine;
using System.Collections;

public class HoverWobbleTest : MonoBehaviour 
{
	public float wobbleSpeed = 0.5f;
	public float LIMITING_CONSTANT = 0.5f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float perturbX = Mathf.PerlinNoise(transform.position.x, Time.time * this.wobbleSpeed) - LIMITING_CONSTANT;
		float perturbY = Mathf.PerlinNoise(transform.position.y, Time.time * this.wobbleSpeed) - LIMITING_CONSTANT;
		float perturbZ = Mathf.PerlinNoise(transform.position.z, Time.time * this.wobbleSpeed) - LIMITING_CONSTANT;
		
		Vector3 newPos = transform.position;
		
		newPos.x += perturbX;
		newPos.y += perturbY;
		newPos.z += perturbZ;
		
		transform.position = Vector3.Lerp (transform.position, newPos, Time.deltaTime);
	}
}
