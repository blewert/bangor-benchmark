using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LobbyMenuHandler : MonoBehaviour 
{
	[Header("Menu components")]
	public GameObject lobbyText;
	public Button startButton;
	
	public List<NetworkPlayer> connections = new List<NetworkPlayer>(); 
	
	public void hookStartButton(Bootup referrer)
	{
		startButton.onClick.AddListener(delegate() 
		{
			referrer.onMultiplayerStartButtonClick();
		});
	}
	
	public void updateLobbyGUI()
	{
		lobbyText.GetComponentInChildren<Text>().text = "Server status (waiting for connections...)\n\n";
		
		if(connections.Count > 0)
		{
			var tempConnections = connections.Select (x => x.ipAddress + ":" + x.port).ToArray ();
			
			lobbyText.GetComponentInChildren<Text>().text += "Connections (" + tempConnections.Count() + "/" + NetworkServer.NUMBER_OF_CONNECTIONS + "): \n";
			
			lobbyText.GetComponentInChildren<Text>().text += tempConnections.Aggregate((prev, next) => prev + "\n" + next);
		}
		else
		{
			lobbyText.GetComponentInChildren<Text>().text += "Connections (0/" + NetworkServer.NUMBER_OF_CONNECTIONS + "): \n";
			lobbyText.GetComponentInChildren<Text>().text += "None!";
		}
	}
	
	public void addConnection(NetworkPlayer player)
	{
		connections.Add (player);
		updateLobbyGUI();
	}
	
	public void removeConnection(NetworkPlayer player)
	{
		connections.Remove(player);
		updateLobbyGUI();
	}
}
