using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour, IProjectile
{
	private bool collided = false;
	private IMortar parentObject = null;
	
	public void SetParent(IMortar parentObject)
	{
		this.parentObject = parentObject;
	}
	
	public void OnCollisionEnter(Collision collision)
	{
		if(collided)
			return;
			
		collided = true;

		if(parentObject != null)
			parentObject.InvokeProjectileHit(this.gameObject, collision);
	}
}
