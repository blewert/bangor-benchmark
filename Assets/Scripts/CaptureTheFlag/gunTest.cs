using UnityEngine;
using System.Collections;

public class gunTest : MonoBehaviour {

	private Gun gun;


	// Use this for initialization
	void Start () {
		gun = GetComponentInChildren<Gun> ();
		gun.OnBulletHit += callback;

		gun.StartFiring();
	}


	void callback(RaycastHit[] hits){
		foreach (var hit in hits) {
			Debug.Log(hit.transform.gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
