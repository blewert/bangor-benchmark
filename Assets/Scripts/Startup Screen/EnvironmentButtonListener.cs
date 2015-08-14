using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnvironmentButtonListener : MonoBehaviour
{
	public void OnButtonClick(Button b, Mode mode)
	{
		string name = b.GetComponentInChildren<Text>().text;
		
		var path = mode.supportedEnvironments.Find (x => name == x.name).path;
		
		StartButtonListener.sceneToLoad = path;
	}
}
