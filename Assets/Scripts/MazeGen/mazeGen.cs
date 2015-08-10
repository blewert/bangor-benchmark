using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mazeGen : MonoBehaviour {

	public GameObject wall;
	public GameObject spawnPoint;
	public GameObject Player;
	public GameObject Drone;

	private float segmentWidth;
	private float segmentLength;
	public int mazeLength = 10;
	public int mazeWidth = 10;
	private GameObject[] allSegments;
	private GameObject[,] gridSegments;
	private GameObject[] allSpawns;
	public int numRooms;
	public int numCorridors;
	public int maxTunnelDepth = 6;


	// Use this for initialization
	void Start () {

		gridSegments = new GameObject[mazeLength, mazeWidth];
		segmentWidth = Mathf.Abs(wall.GetComponent<Renderer>().bounds.min.x - wall.GetComponent<Renderer>().bounds.max.x);
		segmentLength = Mathf.Abs(wall.GetComponent<Renderer>().bounds.min.z - wall.GetComponent<Renderer>().bounds.max.z);

		// Generate Initial Block
		for (int x = 0; x < mazeWidth; x++) {
			for (int y = 0; y < mazeWidth; y++) {
				GameObject segment = (GameObject)Instantiate(wall, new Vector3(x * segmentWidth, 3f, y * segmentLength), Quaternion.identity);
				segment.GetComponent<wallSegment>().setID(y,x);
				gridSegments[x,y] = segment;
				if (x == 0 || x == mazeWidth - 1)
					segment.GetComponent<wallSegment>().border = true;
				if (y == 0 || y == mazeWidth - 1)
					segment.GetComponent<wallSegment>().border = true;
			}
		}

		allSegments = GameObject.FindGameObjectsWithTag ("Segment");

		int randomRooms = numRooms; //Random.Range (1, 4);

		for (int i = 0; i < randomRooms; i++) {

			int xPos 	= Random.Range(1, mazeLength);
			int yPos 	= Random.Range(1, mazeWidth);
			int xLength = Random.Range(7,20);
			int yLength = Random.Range(7,20);

			createRoom (xPos, yPos, xLength, yLength, i);


		}

		createPassage (); 

		for (int n = 0; n < allSegments.Length; n++) {
			GameObject brick = allSegments[n];


			if(allSegments[n].GetComponent<wallSegment>().accessable && !allSegments[n].GetComponent<wallSegment>().border) {

				Destroy( allSegments[n] );

			}



		}

		for (int n = 0; n < allSegments.Length; n++) {
			GameObject brick = allSegments[n];

			
			if (allSegments[n].GetComponent<wallSegment>().corridor) {
				allSegments[n].GetComponent<Renderer>().enabled = false;
				
				allSegments[n].GetComponent<Renderer>().GetComponent<Collider>().enabled = false;
				//allSegments[n].renderer.material.color = Color.green;
				allSegments[n].tag = "Door"; 
			}
				
				
		}

		allSpawns = GameObject.FindGameObjectsWithTag ("Respawn");
		int index = Random.Range (0, allSpawns.Length);
		GameObject playerSpawn = allSpawns [index];

		Instantiate(Player, new Vector3(playerSpawn.transform.position.x, 3f, playerSpawn.transform.position.z), Quaternion.identity);

	}
	

	void Update () {

	}


	void createRoom(int x, int y, int xlen, int ylen, int roomID) {
		for (int i = 0; i < allSegments.Length; i++) {
			int xpos = allSegments[i].GetComponent<wallSegment>().xpos;
			int ypos = allSegments[i].GetComponent<wallSegment>().ypos;
			GameObject brick = allSegments[i];

			int randomXDoor = x + Random.Range(0, xlen); 
			int randomYDoor = y + Random.Range(0, ylen); 


			// Create Wall
			if ((xpos >= x -1 && xpos < (x + xlen + 1) ) && (ypos >= y -1 && ypos < (y + ylen + 1))) {
				brick.GetComponent<wallSegment>().remove = false;
				brick.GetComponent<wallSegment>().roomID = -1;
			}

	

			// Deleate Floor Section
			if ((xpos >= x && xpos < (x + xlen) && xpos < mazeWidth - 1) && (ypos >= y && ypos < (y + ylen) && ypos < mazeLength - 1)) {
				brick.GetComponent<wallSegment>().remove = true; 
				brick.GetComponent<wallSegment>().roomID = roomID; 
			}


		}

		
	}

	void createPassage() {

		//for (int i = 0; i < 30; i++) {
		bool foundRoom = false;
		int xpos = -1;
		int ypos = -1;
		int id = 1;
		int roomID = -1;
		while (!foundRoom) {
			xpos = Random.Range (1, mazeLength-1);
			ypos = Random.Range (1, mazeLength-1);
			GameObject segment = gridSegments [xpos, ypos]; 
			if (segment.GetComponent<wallSegment> ().remove == true) {
				foundRoom = true;
				roomID = segment.GetComponent<wallSegment> ().roomID;
			}
		}
		spawn (id, xpos, ypos, roomID);

		for (int i = 0; i < 6; i++) {
			List<GameObject> accessableSpace = new List<GameObject> ();
			for (int n = 0; n < allSegments.Length; n++) {
				if (allSegments [n].GetComponent<wallSegment> ().accessable) {
					accessableSpace.Add (allSegments [n]);
				}
			}
			int randomIndex = Random.Range(0, accessableSpace.Count);
			if (accessableSpace.Count > 1) {
				spawn (id, accessableSpace [randomIndex].GetComponent<wallSegment> ().xpos, accessableSpace [randomIndex].GetComponent<wallSegment> ().ypos, accessableSpace [randomIndex].GetComponent<wallSegment> ().roomID);
			}

			//print (accessableSpace [randomIndex].GetComponent<wallSegment> ().xpos);
			//print (accessableSpace [randomIndex].GetComponent<wallSegment> ().ypos);

			//
		}

			

		//}

	}

	void spawn(int id, int xpos, int ypos, int roomID) {
		Instantiate(spawnPoint, new Vector3(xpos* segmentWidth, 1f, ypos* segmentLength), Quaternion.identity);
		bool go = true;
		// UP
		GameObject up = gridSegments[xpos,ypos];
		bool 	upBcont = true;
		bool 	upBdrill = false;
		int 	upDrillLength = 0;
		// Right
		GameObject right = gridSegments[xpos,ypos];
		bool 	rightBcont = true;
		bool 	rightBdrill = false;
		int 	rightDrillLength = 0;
		// Down
		GameObject down = gridSegments[xpos,ypos];
		bool 	downBcont = true;
		bool 	downBdrill = false;
		int 	downDrillLength = 0;
		// Left
		GameObject left = gridSegments[xpos,ypos];
		bool 	leftBcont = true;
		bool 	leftBdrill = false;
		int 	leftDrillLength = 0;

		if (id > maxTunnelDepth) {
			go = false;
		}

		//int thisRoomID = gridSegments [xpos, ypos].GetComponent<wallSegment> ().roomID;

		if(go) {
			//print (id);
			for (int i = 0; i < 20; i++) {
				//////////////////////
				// Check upwards

				if(xpos + i >= mazeLength) {upBcont = false;}
				if(upBcont) {
					bool foundWall = false;
					up = gridSegments[xpos + i,ypos];

					//if (thisRoomID != up.GetComponent<wallSegment>().roomID)
					if(up.GetComponent<wallSegment>().roomID == -1)
						foundWall = true;
					if (foundWall)
						if(up.GetComponent<wallSegment>().accessable)
							upBcont = false;
					if (!up.GetComponent<wallSegment>().accessable && up.GetComponent<wallSegment>().roomID != -1  ) {
					//if (up.GetComponent<wallSegment>().roomID != roomID && up.GetComponent<wallSegment>().roomID != -1  ) { 
						upBdrill = true; 
						upBcont = false;
						upDrillLength = i;
						up = gridSegments[xpos + (i - 1),ypos];
					}

				}

				//////////////////////
				// Check right 
				if (ypos + i >= mazeLength) {rightBcont = false;}
				if(rightBcont) {
					bool foundWall = false;
					right = gridSegments[xpos,ypos + i];
					//if (thisRoomID != right.GetComponent<wallSegment>().roomID)
					if(right.GetComponent<wallSegment>().roomID == -1)
						foundWall = true;
					if (foundWall)
						if( right.GetComponent<wallSegment>().accessable)
							rightBcont = false;
					if (!right.GetComponent<wallSegment>().accessable  && right.GetComponent<wallSegment>().roomID != -1){
					//if (right.GetComponent<wallSegment>().roomID != roomID && right.GetComponent<wallSegment>().roomID != -1 ) { 
						rightBdrill = true; 
						rightBcont = false; 
						rightDrillLength = i; 
						right = gridSegments[xpos,ypos + (i - 1)];
					}
				}

				//////////////////////
				// Check down
				if(xpos - i <= 1) {downBcont = false;}
				if(downBcont) {
					bool foundWall = false;
					down = gridSegments[xpos - i,ypos];
					if(down.GetComponent<wallSegment>().roomID == -1)
						foundWall = true;
					if (foundWall)
						if(down.GetComponent<wallSegment>().accessable)
							downBcont = false;
					if (!down.GetComponent<wallSegment>().accessable  && down.GetComponent<wallSegment>().roomID != -1){
					//if (down.GetComponent<wallSegment>().roomID != roomID && down.GetComponent<wallSegment>().roomID != -1 ) { 
						downBdrill = true; 
						downBcont = false; 
						downDrillLength = i;
						down = gridSegments[xpos - (i - 1),ypos];
					}
				}

				//////////////////////
				// Check left
				if(ypos - i <= 1) {leftBcont = false;}
				if(leftBcont) {
					bool foundWall = false;
					left = gridSegments[xpos,ypos - i];

					if(left.GetComponent<wallSegment>().roomID == -1)
						foundWall = true;
					if (foundWall)
						if(left.GetComponent<wallSegment>().accessable)
							leftBcont = false;
					if (!left.GetComponent<wallSegment>().accessable  && left.GetComponent<wallSegment>().roomID != -1){

					//if (left.GetComponent<wallSegment>().roomID != roomID && left.GetComponent<wallSegment>().roomID != -1  ) { 
						leftBdrill = true; 
						leftBcont = false; 
						leftDrillLength = i;
						left = gridSegments[xpos, ypos - (i - 1)];
					}
				} 
			}

			//////////////////////
			// Drill through
			//////////////////////
			int newRoomID = 0;
			if (upBdrill){

				for (int j = 0; j <= upDrillLength; j ++) {
					gridSegments[xpos + j,ypos].GetComponent<wallSegment>().corridor = true;
					gridSegments[xpos + j,ypos].GetComponent<wallSegment>().corridorID = id;
					newRoomID = gridSegments[xpos + j,ypos].GetComponent<wallSegment>().roomID;
				}
				for (int n = 0; n < allSegments.Length; n++) {
					if (allSegments[n].GetComponent<wallSegment>().roomID == newRoomID) {
						allSegments[n].GetComponent<wallSegment>().accessable = true;
					}
				}
				spawn(id + 1, xpos + upDrillLength, ypos, newRoomID);

			} 

			if (rightBdrill){
				for (int j = 0; j <= rightDrillLength; j ++) {
					gridSegments[xpos,ypos + j].GetComponent<wallSegment>().corridor = true;
					gridSegments[xpos,ypos + j].GetComponent<wallSegment>().corridorID = id;
					newRoomID = gridSegments[xpos,ypos + j].GetComponent<wallSegment>().roomID;
				}
				for (int n = 0; n < allSegments.Length; n++) {
					if (allSegments[n].GetComponent<wallSegment>().roomID == newRoomID) {
						allSegments[n].GetComponent<wallSegment>().accessable = true;
					}
				}
				spawn(id + 1, xpos, ypos + rightDrillLength, newRoomID);

			} 

			if (downBdrill){
				for (int j = 0; j <= downDrillLength; j ++) {
					gridSegments[xpos - j,ypos].GetComponent<wallSegment>().corridor = true;
					gridSegments[xpos - j,ypos].GetComponent<wallSegment>().corridorID = id;
					newRoomID = gridSegments[xpos - j,ypos].GetComponent<wallSegment>().roomID;
				}
				for (int n = 0; n < allSegments.Length; n++) {
					if (allSegments[n].GetComponent<wallSegment>().roomID == newRoomID) {
						allSegments[n].GetComponent<wallSegment>().accessable = true;
					}
				}
				spawn(id + 1, xpos - downDrillLength, ypos, newRoomID);

			} 

			if (leftBdrill){
				for (int j = 0; j <= leftDrillLength; j ++) {
					gridSegments[xpos,ypos - j].GetComponent<wallSegment>().corridor = true;
					gridSegments[xpos,ypos - j].GetComponent<wallSegment>().corridorID = id;
					newRoomID = gridSegments[xpos,ypos - j].GetComponent<wallSegment>().roomID;
				}
				for (int n = 0; n < allSegments.Length; n++) {
					if (allSegments[n].GetComponent<wallSegment>().roomID == newRoomID) {
						allSegments[n].GetComponent<wallSegment>().accessable = true;
					}
				}
				spawn(id + 1, xpos, ypos - leftDrillLength, newRoomID);

			} 

		}
		
	}


	void createCorridors(int x, int y, int xlen, int ylen) {
		int direction = Random.Range (0, 4);
	
		int startposX; 
		int startposY;


		//print (gridSegments.Length);
		// UP
		if (direction == 0) {
			startposX = x + xlen;
			startposY = Random.Range(y, y + ylen);
			for (int i = 0; i < 20; i++) {
				if((startposX + i) < mazeLength)
					if(startposY < mazeLength)
						gridSegments[startposX + i, startposY].GetComponent<wallSegment>().corridor = true;
			}
		}

		// RIGHT
		if (direction == 1) {
			startposX = Random.Range(x, x + xlen);
			startposY = y + ylen;
			for (int i = 0; i < 20; i++) {
				if(startposY + i < mazeLength)
					if(startposX < mazeLength) 
						gridSegments[startposX, startposY + i].GetComponent<wallSegment>().corridor = true;
			}
		}

		// DOWN
         if (direction == 2) {
			startposX = x;
			startposY = Random.Range(y, y + ylen);
			for (int i = 0; i < 20; i++) {
				if(startposX - i > 0)
					if(startposY < mazeLength)
						gridSegments[startposX - i, startposY].GetComponent<wallSegment>().corridor = true;
			}
		}
		// LEFT
		if (direction == 3) {
			startposX = Random.Range(x, x + xlen);
        	startposY = y;
			for (int i = 0; i < 20; i++) {
				if(startposY - i > 0)
					if(startposY < mazeLength)
						gridSegments[x, startposY - i].GetComponent<wallSegment>().corridor = true;
			}
		}


	}



}
