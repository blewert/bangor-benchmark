//#define DEBUG_NON_RELEASE

using UnityEngine;
using System.Linq;
using System.Collections;
using UnityStandardAssets.Utility;

public class TankSpawner : MonoBehaviour 
{
	private string[] npcNames = 
	{
		"C. Headleand",
		"J. Jackson",
		"B. Williams",
		"L. Chapman",
		"J. Hall",
		"H. Price",
		"S. Marriott",
		"K. Bold",
		"L. Kuncheva",
		"I. Soo Lim",
		"C. Gray",
		"J. Roberts",
		"S. Mansoor",
		"W. Teahan",
		"M. Smith",
		"L. ap Cenydd"
	};
	
	private int NUMBER_OF_TEAMS = 2;
	public int NUMBER_OF_AGENTS = 16;
	public float MAX_SPAWN_RADIUS = 10f;
	
	private Transform redTeamSpawn;
	private Transform blueTeamSpawn;
	
	public GameObject agentPrefab;
	
	// Use this for initialization
	void Start () 
	{
		#if !DEBUG_NON_RELEASE
		NUMBER_OF_AGENTS = PlayerPrefs.GetInt ("numberOfPlayers");
		agentPrefab = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString ("characterPrefab"), typeof(GameObject));
		#endif

		npcNames = npcNames.OrderBy(x => Random.value).ToArray(); 
		
		redTeamSpawn = GameObject.Find("Red Team Spawn").transform;
		blueTeamSpawn = GameObject.Find("Blue Team Spawn").transform;
		
		NameTag redFlagNameTag = redTeamSpawn.gameObject.AddComponent<NameTag>();
		redFlagNameTag.obj = redTeamSpawn.gameObject;
		redFlagNameTag.text = "Red Base";
		redFlagNameTag.setFontColor(Color.white);
		redFlagNameTag.fontSize = 14;
		
		NameTag blueFlagNameTag = blueTeamSpawn.gameObject.AddComponent<NameTag>();
		blueFlagNameTag.obj = blueTeamSpawn.gameObject;
		blueFlagNameTag.text = "Blue Base";
		blueFlagNameTag.setFontColor(Color.white);
		blueFlagNameTag.fontSize = 14;
		
		int humanSpawnIndex = Random.Range (0, NUMBER_OF_AGENTS);
		
		for(int i = 0; i < NUMBER_OF_AGENTS; i++)	
		{
			Vector3 spawnPosition = Vector3.zero;
			
			if(i % NUMBER_OF_TEAMS == 0)
				spawnPosition = redTeamSpawn.position;
			else
				spawnPosition = blueTeamSpawn.position;
		
			
			//Spawn some npcs
			GameObject npc = instantiateAround(agentPrefab, ref spawnPosition);
			
			NameTag nameTagScript = npc.AddComponent<NameTag>();
			nameTagScript.obj = npc.gameObject;
			nameTagScript.text = npcNames[i];
			nameTagScript.fontSize = 10;
			nameTagScript.bold = true;
			
			if(i % NUMBER_OF_TEAMS == 0)
			{
				npc.tag = "Team1";
				nameTagScript.setFontColor(Color.red);
			}
			else
			{
				npc.tag = "Team2";
				nameTagScript.setFontColor(Color.blue);
			}
			
			if(i != humanSpawnIndex)
			{			
				//Bolt on NPC logic
				//npc.AddComponent<AIController>();
			}
			else
			{
				npc.AddComponent <TankHumanController>();
				//npc.AddComponent <Crosshair>();
			}
		}
	}
	
	private GameObject instantiateAround(GameObject prefab, ref Vector3 position)
	{
		position.x += Random.Range (-MAX_SPAWN_RADIUS, MAX_SPAWN_RADIUS);
		position.z += Random.Range (-MAX_SPAWN_RADIUS, MAX_SPAWN_RADIUS);
		position.y = Terrain.activeTerrain.SampleHeight(position) + 5;
		
		Vector3 euler = new Vector3(0, Random.Range (0, 360), 0); 
		
		return (GameObject)Instantiate ((Object)prefab, position, Quaternion.Euler (euler));
	}
}
