using UnityEngine;
using System.Collections;

public abstract class ILocomotionScript : MonoBehaviour
{
	public CharacterInstance instance;
	
	public delegate void onUpdateHandler(int id, Vector3 newPosition, Quaternion newRotation);
	public event onUpdateHandler onUpdate;
	
	public abstract void turnLeft();
	public abstract void turnRight();
	public abstract void moveForward();
	public abstract void moveBackward();
		
	public void updatePosition()
	{
		if(onUpdate != null)
			onUpdate.Invoke ((int)gameObject.getID(), transform.position, transform.rotation);
	}
	
	public float health = 100.0f;
	public bool dead = false;
	
	public float getHealth()
	{
		return health;
	}
	
	public void takeHealth(float value)
	{
		health -= value;
		
		if(getHealth() <= 0f)
			dead = true;
	}
	
	public void giveHealth(float value)
	{
		health = Mathf.Clamp (health + value, 0f, 100.0f);
	}
	
}

