using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowProjectVersion : MonoBehaviour 
{
	void Start () 
	{
		GetComponent<Text>().text = Application.productName + " " + Application.version + "-" + Application.unityVersion + "-" + Application.platform;
	}
}
