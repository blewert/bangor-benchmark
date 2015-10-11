﻿using UnityEngine;
using System.Collections;

public class TestMortarFire : MonoBehaviour 
{
	private IMortar mortar;
	public float offsetY;
	public GameObject particleSystemObject;

	// Use this for initialization
	void Start () 
	{
		mortar = GetComponentInChildren<IMortar>();
		mortar.OnProjectileHit += projectileHit;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mortar is IContinousMortar)
		{
			var castMortar = mortar as IContinousMortar;
			
			if(Input.GetKeyDown(KeyCode.Space))
				castMortar.StartFiring();
	
			else if(Input.GetKeyUp (KeyCode.Space))
				castMortar.StopFiring();
		}
		else if(mortar is IMortar)
		{
			if(Input.GetKeyUp (KeyCode.Space))
				mortar.Fire ();
		}
	}
	
	void projectileHit(GameObject particle, Collision collision)
	{
		Destroy (particle);
		
		//add explosion here
		Vector3 position = collision.contacts[0].point;	
		
		position.y += offsetY;
		
		GameObject explosionObject = (GameObject)Instantiate (particleSystemObject, position, Quaternion.identity);
		explosionObject.GetComponent<ParticleSystem>().Play();
		
		Destroy (explosionObject, 1f);
	}
	
}
