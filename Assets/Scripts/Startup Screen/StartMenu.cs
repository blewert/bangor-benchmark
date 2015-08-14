using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour 
{
	public RectTransform leftColumnLabel;
	public RectTransform midColumnLabel;
	public RectTransform rightColumnLabel;
	public Button buttonPrefab;
	
	public int offset;
	
	public void Start()
	{
		for(int i = 0; i < 8; i++)
		{
			var button = (Button)Instantiate(buttonPrefab, midColumnLabel.transform.position, Quaternion.identity);
			
			button.transform.SetParent (transform);
			button.GetComponentInChildren<Text>().text = i.ToString();
		
			var position = button.transform.position;
			
			position.y -= offset + i * offset;
			
			button.transform.position = position; 
		}
	}
}
