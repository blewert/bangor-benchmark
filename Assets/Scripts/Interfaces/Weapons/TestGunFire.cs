using UnityEngine;
using System.Collections;

public class TestGunFire : MonoBehaviour 
{
	private Gun gun;
	
	// Use this for initialization
	void Start () 
	{
		gun = GetComponentInChildren<Gun>();
	
		gun.OnBulletHit += OnBulletHit;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gun.gunType == Gun.GunType.CONTINOUS_FIRE)
		{
			if(Input.GetKeyDown (KeyCode.Space))
				gun.StartFiring ();
				
			else if(Input.GetKeyUp (KeyCode.Space))
				gun.StopFiring();
		}
		else
		{
			gun.Fire ();
		}
	}
	
	public void OnBulletHit(RaycastHit[] hitData)
	{
		if(hitData.Length == 0)
			return;
			
		RaycastHit hit = hitData[0];
		
		Debug.Log ("Name of hit object: " + hit.transform.name);
	}
}
