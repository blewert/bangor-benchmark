using UnityEngine;
using System.Collections;

public interface IContinousMortar : IMortar
{
	void StartFiring();
	void StopFiring();
}
