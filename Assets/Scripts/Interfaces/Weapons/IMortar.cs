using UnityEngine;
using System.Collections;

public delegate void OnProjectileHitHandler(GameObject projectile, Collision collisionData);

public interface IMortar 
{
	event OnProjectileHitHandler OnProjectileHit; 
	void InvokeProjectileHit(GameObject a, Collision b);
	void Fire();
}
