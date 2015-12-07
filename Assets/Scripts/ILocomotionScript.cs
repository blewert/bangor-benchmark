using UnityEngine;
using System.Collections;

public abstract class ILocomotionScript : MonoBehaviour
{
	public CharacterInstance instance;
	
	public abstract void turnLeft();
	public abstract void turnRight();
	public abstract void moveForward();
	public abstract void moveBackward();
	
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

