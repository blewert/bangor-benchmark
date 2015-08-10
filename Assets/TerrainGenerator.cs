using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
	public static MazeGenerator2 mg;
	public bool SEEDON;
	public int SEED;
	public GameObject wall;
	public GameObject spawnPoint;
	public GameObject Player;
	public GameObject Drone;
	private GameObject[] allSpawns;
	public GameObject SpawnPosition;

	public int mazeLength;
	public int mazeWidth;
	public float wallStretch;

	// Use this for initialization
	void Start () {
		if(SEEDON){
			UnityEngine.Random.seed = SEED;}
		mg = new MazeGenerator2();
		mg.GenerateMaze(mazeLength,mazeWidth);
		for(int i =0; i<mazeLength; i++){
			for(int j =0; j<mazeWidth; j++){
				if(mg.maze[i,j] == 1){
					GameObject segment = (GameObject)Instantiate(wall, new Vector3(i*3 * wallStretch, 3f,j*3 * wallStretch), Quaternion.identity);
					segment.transform.localScale = new Vector3(wallStretch*3, 6, wallStretch*3);
					segment.GetComponent<Renderer>().material.mainTextureScale = new Vector2(wallStretch*2, 2);
				}
			}
		}
		SpawnPosition.transform.Translate( new Vector3(wallStretch * 3f,-0.4f,wallStretch * 3f));
		allSpawns = GameObject.FindGameObjectsWithTag ("Respawn");
		int index = Random.Range (0, allSpawns.Length);
		GameObject playerSpawn = allSpawns [index];
		
		Instantiate(Player, new Vector3(playerSpawn.transform.position.x, 3f, playerSpawn.transform.position.z), Quaternion.identity);
	}
}
