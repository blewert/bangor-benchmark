using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ClientMenuHandler : MonoBehaviour 
{
	[Header("Component options")]
	public Button startButton;
	public InputField ipInput; 
	public InputField portInput;
	public InputField passwordInput;
	public Text statusText;
	
	// Use this for initialization
	void Start () 
	{
		startButton.onClick.AddListener(delegate() 
		{
			startClient();
		});
	}
	
	public void startClient()
	{
		var obj = GameObject.Find ("NetworkManager");
		
		var ipText = ipInput.GetComponentsInChildren<Text>().Where (x => x.transform.name == "Text").FirstOrDefault().text;
		var portText = portInput.GetComponentsInChildren<Text>().Where (x => x.transform.name == "Text").FirstOrDefault().text;
		var passwordText = passwordInput.GetComponentsInChildren<Text>().Where (x => x.transform.name == "Text").FirstOrDefault().text;
		
		int port = int.Parse(portText);
		
		var network = GameObject.Find ("NetworkManager").GetComponent<NetworkServer>();
		
		try
		{
			network.isMultiplayer = true;
			network.startTypeClient = true;
			network.connect (ipText, port, passwordText);
			
			statusText.text = "Connecting to " + ipText + "...";
		}
		catch(System.Exception e)
		{
			statusText.text = "Couldn't connect to " + ipText + ".";
		}
	}
	
	public void OnConnectedToServer()
	{
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
