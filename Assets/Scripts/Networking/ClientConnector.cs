using UnityEngine;
using System.Collections;

public class ClientConnector : MonoBehaviour
{
	public string ip;
	public int port;
	public string password;

	// Use this for initialization
	void Start ()
	{
		NetworkServer.isMultiplayer = true;
		NetworkServer.startTypeClient = true;
		NetworkServer.connect (ip, port, password);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

