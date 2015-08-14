using UnityEngine;
using System.Collections;

public class StartButtonListener : MonoBehaviour 
{
	public static string sceneToLoad = null;
	public static string charToLoad = null;
	public static int numberOfPlayers = 10;
	
	public void OnStartButtonClicked()
	{
		if(sceneToLoad == null || charToLoad == null)
			return;
			
		PlayerPrefs.SetInt("numberOfPlayers", numberOfPlayers);
		PlayerPrefs.SetString ("characterPrefab", charToLoad);
				
		Debug.Log ("Loading scene " + sceneToLoad + " with character " + charToLoad);
	
		Application.LoadLevel (sceneToLoad);
	}
}
