using UnityEngine;
using System.Collections;

public class Mortar : MonoBehaviour, IMortar
{
	public event OnProjectileHitHandler OnProjectileHit;
	
	[Header("Firing options")]
	public GameObject projectilePrefab;
	public float fireForce;
	public float delayBetweenShots;
	
	[Header("Muzzle flash options")]
	public bool showMuzzleFlash;
	public GameObject muzzleFlashObject;
	private ParticleSystem pSystem;
	
	[Header("Random force applied to projectile")]
	public bool applyRandomForces;
	public Vector2 maxForcesApplied;
	
	// Use this for initialization
	void Start () 
	{
		//Add continous mortar interface IContinousMortar
		pSystem = muzzleFlashObject.GetComponent<ParticleSystem>();
	}
	
	public void InvokeProjectileHit(GameObject a, Collision b)
	{
		OnProjectileHit.Invoke (a, b);
	}
	
	public virtual void StartFiring()
	{
		StartCoroutine (fireUpdate());
	}
	
	public virtual void StopFiring()
	{
		StopAllCoroutines();
	}
	
	private IEnumerator fireUpdate()
	{
		while(true)
		{
			Fire ();
			
			yield return new WaitForSeconds(delayBetweenShots);
		}
	}
	
	public virtual void Fire()
	{	
		if(showMuzzleFlash && pSystem != null)
			pSystem.Emit(1);
		
		Vector3 instantiatePos = transform.TransformPoint(Vector3.up * 1.2f);
		
		Debug.DrawLine (instantiatePos, transform.position + transform.up * 8, Color.yellow, 2f);
		
		GameObject projectile = (GameObject)Instantiate(projectilePrefab, instantiatePos, transform.rotation);
		IProjectile projectileInterface = projectile.GetComponent<IProjectile>();
		
		//projectile.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.z);
		
		projectileInterface.SetParent(this);
		
		Rigidbody pRigid = projectile.GetComponent<Rigidbody>();
		
		Vector3 newForce = -transform.up * fireForce;
		
		if(applyRandomForces)
		{
			newForce.x += Random.Range (-maxForcesApplied.x, maxForcesApplied.x);
			newForce.y += Random.Range (-maxForcesApplied.y, maxForcesApplied.y);
		}
		
		pRigid.AddForce(newForce, ForceMode.Impulse);
	}
	
	void OnDrawGizmosSelected()
	{
	}
	
	#region old handler
	/*public virtual void OnProjectileHit(GameObject projectile, Collision collisionData)
	{
		Destroy (projectile);
		
		Collider[] colliders = Physics.OverlapSphere(collisionData.contacts[0].point, explosionRadius);
		
		foreach(Collider col in colliders)
		{
			Rigidbody attachedRigidbody = col.attachedRigidbody;
			
			if(attachedRigidbody == null)
				continue;
			
			attachedRigidbody.AddExplosionForce(explosionForce, collisionData.contacts[0].point, explosionRadius, 0f, ForceMode.Impulse);
		}
	}*/
	#endregion
}
