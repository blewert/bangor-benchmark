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
	public GameObject lobbyTitle;
	public GameObject lobbyError;
	
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
		//lobbyText.GetComponentInChildren<Text>().text = "Server status (waiting for connections...)\n\n";
		
		if(connections.Count > 0)
		{
			lobbyTitle.GetComponent<Text>().text = "Lobby (" + connections.Count + "/" + NetworkServer.NUMBER_OF_CONNECTIONS + " players)";
			
			var tempConnections = connections.Select (x => "Player " + (connections.IndexOf(x)+1) + ": " + x.ipAddress + ":" + x.port).ToArray ();
			
			lobbyText.GetComponentInChildren<Text>().text += tempConnections.Aggregate((prev, next) => prev + "\n" + next);
		}
		else
		{
			lobbyTitle.GetComponent<Text>().text = "Lobby (Waiting for players...)";
			lobbyText.GetComponentInChildren<Text>().text = "";
		}
	}
	
	public void setErrorText(string text)
	{
		lobbyError.GetComponent<Text>().text = text;
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
