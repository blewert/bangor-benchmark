using UnityEngine;
using System.Collections;

public class pulseControler : MonoBehaviour {
	public float moveSpeed = 1f;
	public float hightOffset = 1.5f;
	
	private float terrainX;
	private float terrainZ;
	private float terrainZsize;
	private float terrainXsize;


	void Start () {


		terrainZ = Terrain.activeTerrain.transform.position.z;
		terrainX = Terrain.activeTerrain.transform.position.x;
		terrainZsize = Terrain.activeTerrain.terrainData.size.z;
		terrainXsize = Terrain.activeTerrain.terrainData.size.x;
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		float posY = (Terrain.activeTerrain.SampleHeight(transform.position)) + hightOffset;
		transform.position = new Vector3 (transform.position.x, posY, transform.position.z);

		if (transform.position.x >= terrainX + terrainXsize) {
			Destroy(gameObject);
		}
		if (transform.position.z >= terrainZ + terrainZsize) {
			Destroy(gameObject);
		}


	}


}
