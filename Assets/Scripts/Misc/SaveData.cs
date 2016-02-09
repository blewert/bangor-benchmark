using UnityEngine;
using System.Collections;
using System.IO;

public class SaveData : MonoBehaviour {

	public string csv;
	private string data;
	private int loopCount = 0;
	// Use this for initialization
	void Start () {
		StartCoroutine (LogData ());
		data = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	private IEnumerator LogData(){
		while(true){
			yield return new WaitForSeconds(1);
			data += GetComponentInParent<TankSpawner>().GetData();
			if(loopCount ++ == 29){
				if (!File.Exists(csv)){
					string createText = "Player Health, Player Score, Companion Health, " + 
						"Companion Score, Companion Distance, Enemy Count, Under Attack\n";
					File.WriteAllText(csv, createText);
				} 
				File.AppendAllText(csv, data);
				Debug.Log("Data Written!");
				Debug.Log (GameObject.Find("LeftHUD").transform.position);
				loopCount = 0;
			}
		}


	}

	public void WriteCSV(string fileName, string data, string head = ""){
		if (!File.Exists(fileName)){
			File.WriteAllText(fileName, head);
		}
		File.AppendAllText(fileName, data);
	}
}
