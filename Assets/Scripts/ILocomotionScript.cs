using UnityEngine;
using System.Collections;

public abstract class ILocomotionScript : MonoBehaviour
{
	public Instance instance;
	
	public abstract void turnLeft();
	public abstract void turnRight();
	public abstract void moveForward();
	public abstract void moveBackward();
}

