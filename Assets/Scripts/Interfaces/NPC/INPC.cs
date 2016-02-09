using UnityEngine;
using System.Collections;

public abstract class INPC : MonoBehaviour
{
	public const float MAX_HEALTH = 1f;
	public bool alive = true;
	public float health = MAX_HEALTH;

	public float getHealth()
	{
		return health;
	}
	
	public void takeHealth(float amount)
	{
		health -= Mathf.Clamp (amount, 0, MAX_HEALTH);

		alive = health > 0f;
		
		if(!alive)
			onDeath();
	}
	
	public void onDeath()
	{
	}
		
	abstract public void turnLeft();
	abstract public void turnRight();
	abstract public void moveForward();
	abstract public void moveBackward();
}
