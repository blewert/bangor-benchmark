using UnityEngine;
using System.Collections;

/// <summary>
/// Class which represents a gun to be fired. The gun can either be automatic or single shot (bolt action), and can have a number of parameters
/// such as the inaccuracy of shots, the distance of shots, the delay between automatic firing, whether or not visual effects such as
/// muzzle flash is shown and a callback which can be hooked when a bullet hits an object.
/// </summary>
public class Gun : MonoBehaviour 
{
	//Properties of the gun and it's firing mechanism
	[Header("Gun properties")]
	public float shotDistance = 15f;
	public GunType gunType;
	public float delayBetweenShots;

	public enum CastEnum
	{
		RayCast, SphereCast
	}

	public CastEnum CastType = CastEnum.RayCast;
	
	//Visual effects such as muzzle flare 
	[Header("Effects")]
	public bool showMuzzleFlare = false;
	public GameObject muzzleParticleObject;
	
	//Inaccuracy offsets
	[Header("Inaccuracy Settings")]
	public bool applyInaccuracies = false;
	public float errorDelta;	

	// Firing effect

	public GameObject tracer;
	
	//Delegate for bullet hit handling listener, and the broadcaster itself
	public delegate void OnBulletHitHandler(RaycastHit[] hitData);
	public event OnBulletHitHandler OnBulletHit;
	
	//The particle system which is attached to the gun for visual effects
	private ParticleSystem pSystem;
	private ParticleSystem tSystem;
	private bool isFiring = false;


	//Gun types - single_shot/continous_fire (automatic)
	public enum GunType
	{
		CONTINOUS_FIRE, SINGLE_SHOT
	};
		
	/// <summary>
	/// Called on script start.
	/// </summary>
	void Start () 
	{
		pSystem = muzzleParticleObject.GetComponent<ParticleSystem>();
		tSystem = tracer.GetComponent<ParticleSystem>();
		tracer.transform.rotation = transform.rotation;
		Vector3 pos = transform.position;
		pos.y = pos.y + 10;
		tracer.transform.position = transform.position;
		tracer.transform.forward = transform.forward;
		tSystem.Stop ();
	}
	
	/// <summary>
	/// Fires the gun only once, and returns all hits.
	/// </summary>
	public RaycastHit[] Fire()
	{
		if(gunType != GunType.SINGLE_SHOT)
			return null;
			
		callMuzzleFlash();
			
		return Physics.RaycastAll(transform.position, transform.forward, shotDistance);
	}
	
	/// <summary>
	/// Starts the gun firing automatically.
	/// </summary>
	public void StartFiring()
	{						
		if (!isFiring) {
			if (gunType != GunType.CONTINOUS_FIRE)
				return;
			tSystem.Play ();
			StartCoroutine (fireUpdate ());
			isFiring = true;
		}
	}
	
	/// <summary>
	/// Stops the gun firing automatically.
	/// </summary>
	public void StopFiring()
	{
		StopAllCoroutines();
		tSystem.Stop ();
		isFiring = false;
	}
	
	/// <summary>
	/// The function which is called for every bullet fired automatically; handles the delays between bullets being shot.
	/// </summary>
	private IEnumerator fireUpdate()
	{

		while(true)
		{			
			//Show muzzle flash for each shot (if needed)
			callMuzzleFlash();

			//Calculate ray direction with no inaccuracies
			Vector3 bulletDirection = transform.forward * shotDistance;

			//Debug.DrawRay(transform.position, bulletDirection, Color.green, 3);

			//Debug.Log(transform.position);
			
			if(applyInaccuracies)
			{
				//Take |d| because it might be -ve
				float mag = Mathf.Abs(errorDelta);
				
				//Apply random inaccuracies on local x + y (z would make no difference)
				bulletDirection.x += Random.Range(-mag, mag);
				bulletDirection.y += Random.Range(-mag, mag);
			}
			
			//Cast data, call invokation for callback
			RaycastHit[] hitData;
			if(CastType == CastEnum.RayCast)
				hitData = Physics.RaycastAll (transform.position, bulletDirection, shotDistance);
			else{
				Debug.Log("firing");
				Debug.DrawRay(transform.position, bulletDirection, Color.red);
				hitData = Physics.SphereCastAll(transform.position, 3.0f, bulletDirection, shotDistance);
			}
			OnBulletHit.Invoke(hitData);
			
			//Wait for the next shot
			yield return new WaitForSeconds(delayBetweenShots);
		}
	}	
	
	/// <summary>
	/// Shows the muzzle flare/flash
	/// </summary>
	public void callMuzzleFlash()
	{
		//Do we want to show it?
		if(!showMuzzleFlare)
			return;
			
		//Can we show it?
		if(pSystem == null)
			return;
		
		//If so, emit a single particle
		pSystem.Emit (1);


	}
	
	/// <summary>
	/// Used for updating editor view for controlling user variables such as shot distance.
	/// </summary>
	void OnDrawGizmosSelected() 
	{		
		//Only for edit mode!
		if(!Application.isEditor)
			return;
			
		//Distance indicator is white
		Gizmos.color = Color.red;
		
		//Draw a line from the gun to the point in front of it by shot distance, render a sphere there
//		Gizmos.DrawLine (transform.position, transform.position + transform.forward * shotDistance);
//		Gizmos.DrawWireSphere(transform.position + transform.forward * shotDistance, 0.2f);

		//Gizmos.DrawRay (transform.position, transform.forward * 5);

		//Gizmos.DrawSphere (transform.position, 3.0f);
		
//		if (applyInaccuracies) {
//			//Inaccuracy indicators are blue
//			Gizmos.color = Color.blue;
//			
//			//Find the point in front of the gun by shot distance again
//			Vector3 centrePosition = transform.position + transform.forward * shotDistance;
//			
//			//Number of points to render in cone
//			int numPoints = 16;
//		
//			for (float t = 0; t < 360f; t += (360f/numPoints)) {
//				//Copy position for modification locally
//				Vector3 offsetPosition = centrePosition;
//				
//				//Find x and y of this angle on the plane parallel to the vector from the gun
//				offsetPosition.x += Mathf.Sin (Mathf.Deg2Rad * t) * errorDelta;
//				offsetPosition.y += Mathf.Cos (Mathf.Deg2Rad * t) * errorDelta;	
//				
//				//And draw a line to it
//				Gizmos.DrawLine (transform.position, offsetPosition);
//				
//				
//				//Bit of a hackish way to find old position and draw a circle, but it works:
//				Vector3 oldPosition = centrePosition;
//				
//				oldPosition.x += Mathf.Sin (Mathf.Deg2Rad * (t - (360f / numPoints))) * errorDelta;
//				oldPosition.y += Mathf.Cos (Mathf.Deg2Rad * (t - (360f / numPoints))) * errorDelta;	
//				
//				Gizmos.DrawLine (oldPosition, offsetPosition);
//			}
//		} else {
//			//Inaccuracy indicators are blue
//			Gizmos.color = Color.blue;
//			
//			//Find the point in front of the gun by shot distance again
//			Vector3 centrePosition = transform.position + transform.forward * 500;
//			
//			//Number of points to render in cone
//			int numPoints = 16;
//
//				//Copy position for modification locally
//			Vector3 offsetPosition = centrePosition;
//			
//			//Find x and y of this angle on the plane parallel to the vector from the gun
//			offsetPosition.x += Mathf.Sin (Mathf.Deg2Rad * 0) * errorDelta;
//			offsetPosition.y += Mathf.Cos (Mathf.Deg2Rad * 0) * errorDelta;	
//			
//			//And draw a line to it
//			Gizmos.DrawLine (transform.position, centrePosition);
//			
//			
//			//Bit of a hackish way to find old position and draw a circle, but it works:
//			Vector3 oldPosition = centrePosition;
//			
//			oldPosition.x += Mathf.Sin (Mathf.Deg2Rad * (0 - (360f / numPoints))) * errorDelta;
//			oldPosition.y += Mathf.Cos (Mathf.Deg2Rad * (0 - (360f / numPoints))) * errorDelta;	
//			
//			//Gizmos.DrawLine (oldPosition, centrePosition);
//
//		}
	}
	
}
