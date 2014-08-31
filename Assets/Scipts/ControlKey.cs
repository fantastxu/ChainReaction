using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlKey : MonoBehaviour {

	public string Key;
	public bool isGirl;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnButtonClicked()
	{
		Debug.Log("Button cliecked!");
		if (isGirl) {
						ArgumentRoom.girl.SelectFragment (Key);
						ArgumentRoom.isGirlSelected = true;
				} else {
						ArgumentRoom.boy.SelectFragment (Key);
						ArgumentRoom.isBoySelected = true;
				}
	}






}
