using UnityEngine;
using System.Collections;

public abstract class IFlyableNPC : INPC
{
	abstract public void strafeLeft();
	abstract public void strafeRight();
	abstract public void ascend();
	abstract public void descend();
}
