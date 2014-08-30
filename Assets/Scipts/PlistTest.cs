using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlistCS;

public class PlistTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

		TextAsset plistasset = Resources.Load<TextAsset>("ChainReaction");

		Dictionary<string, object> dic2 = (Dictionary<string, object>)Plist.readPlist(plistasset.bytes);
		Dictionary<string, object> girldic = (Dictionary<string, object>)dic2["Girl"];
		Dictionary<string, object> santance1 = (Dictionary<string, object>)girldic["1"];
		foreach (string key in santance1.Keys) {
			Debug.Log(key+" is "+santance1[key].GetType());		
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
