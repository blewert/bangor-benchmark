using UnityEngine;
using System.Collections;

public class pulser : MonoBehaviour {

	public GameObject energyPulse;
	public Terrain hordeTerrain;

	private float terrainX;
	private float terrainZ;
	private float terrainZsize;
	private float terrainXsize;

	// Use this for initialization
	void Start () {
		terrainZ = hordeTerrain.transform.position.z;
		terrainX = hordeTerrain.transform.position.x;
		terrainZsize = hordeTerrain.terrainData.size.z;
		terrainXsize = hordeTerrain.terrainData.size.x;

		for (int i = 0; i < 400; i++) {
			if (Random.Range (0, 4) < 2) {
				Instantiate (energyPulse, new Vector3 (Random.Range (terrainX, terrainX + terrainXsize), 1f, Random.Range (terrainZ, terrainZ + terrainZsize)), Quaternion.Euler(Quaternion.identity.eulerAngles.x, Quaternion.identity.eulerAngles.y + 90f, Quaternion.identity.eulerAngles.z));
			} else {			
				Instantiate (energyPulse, new Vector3 (Random.Range (terrainX, terrainX + terrainXsize), 1f, Random.Range (terrainZ, terrainZ + terrainZsize)), Quaternion.identity);
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		//Create an energy pulse along one of the edges

		if (Random.Range (0, 4) < 2) {
			Instantiate (energyPulse, new Vector3 (terrainX, 1f, Random.Range (terrainZ, terrainZ + terrainZsize)), Quaternion.Euler(Quaternion.identity.eulerAngles.x, Quaternion.identity.eulerAngles.y + 90f, Quaternion.identity.eulerAngles.z));
		} else {			
			Instantiate (energyPulse, new Vector3 (Random.Range (terrainX, terrainX + terrainXsize), 1f, terrainZ), Quaternion.identity);
		}

	}
}
