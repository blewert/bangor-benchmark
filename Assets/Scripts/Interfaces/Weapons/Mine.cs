using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
	private bool exploded = false;
	
	public float explosionForce;
	public float explosionRadius;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnCollisionEnter(Collision collisionData)
	{
		foreach(ContactPoint point in collisionData.contacts)
		{
			Vector3 topSide = transform.position + transform.up * (transform.localScale.y / 2);
			Vector3 bottomSide = transform.position + -transform.up * (transform.localScale.y / 2);
			
			if(Vector3.Distance (topSide, point.point) < Vector3.Distance (bottomSide, point.point))
				OnExplode(collisionData);
		}
	}
	
	public virtual void OnExplode(Collision collisionData)
	{
		if(exploded)
			return;
			
		exploded = true;
		
		Debug.Log ("boom (mine)");
		
		Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);
		
		foreach(Collider col in objects)
		{
			Rigidbody r = col.attachedRigidbody;
			
			if(r == null)
				continue;
				
			r.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
		}
		
		Destroy (this.gameObject);
	}
}
