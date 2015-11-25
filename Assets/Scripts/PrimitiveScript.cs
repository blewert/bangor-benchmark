using UnityEngine;
using System.Collections;

public abstract class PrimitiveScript : MonoBehaviour
{
	public Instance instance;
}

public abstract class GamemodeScript : PrimitiveScript
{
	public CharacterInstance characterInstance;
}