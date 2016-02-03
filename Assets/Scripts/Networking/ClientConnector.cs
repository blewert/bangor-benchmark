using UnityEngine;
using System.Collections;

public class ClientConnector : MonoBehaviour
{
	public string ip;
	public int port;
	public string password;
	private NetworkServer network;
	
	// Use this for initialization
	void Start ()
	{
		network = GameObject.Find ("NetworkManager").GetComponent<NetworkServer>();
		
		network.isMultiplayer = true;
		network.startTypeClient = true;
		network.connect (ip, port, password);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

