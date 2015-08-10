using UnityEngine;
using System.Collections;

public class Mortar : MonoBehaviour, IMortar
{
	public event OnProjectileHitHandler OnProjectileHit;
	
	public GameObject projectilePrefab;
	public float explosionForce;
	public float explosionRadius;
	
	// Use this for initialization
	void Start () 
	{
		//OnProjectileHit += new OnProjectileHitHandler(aaa);
	}
	
	public void InvokeProjectileHit(GameObject a, Collision b)
	{
		OnProjectileHit.Invoke (a, b);
	}
	
	public virtual void Fire(float force)
	{	
		Vector3 instantiatePos = transform.TransformPoint(Vector3.up * 1.2f);
		
		Debug.DrawLine (instantiatePos, transform.position + transform.up * 8, Color.yellow, 2f);
		
		GameObject projectile = (GameObject)Instantiate(projectilePrefab, instantiatePos, transform.rotation);
		IProjectile projectileInterface = projectile.GetComponent<IProjectile>();
		
		projectile.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.z);
		
		projectileInterface.SetParent(this);
		
		Rigidbody pRigid = projectile.GetComponent<Rigidbody>();
		
		pRigid.AddForce(transform.up * force, ForceMode.Impulse);
	}
	
	
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
}
