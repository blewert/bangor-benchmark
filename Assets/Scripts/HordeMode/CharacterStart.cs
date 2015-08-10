using UnityEngine;
using System.Collections;

public class CharacterStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float posY = (Terrain.activeTerrain.SampleHeight (transform.position)) + 7f ;
		transform.position = new Vector3 (transform.position.x, posY, transform.position.z);

	}
	
	// Update is called once per frame
	void Update () {

		//print (posY);
	}
}
