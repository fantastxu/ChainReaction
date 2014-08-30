using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlistCS;

public class PlistTest : MonoBehaviour {

	Girl girl;
	Boy boy;
	List<string> fragments = new List<string>();
	System.Random girlRandom;
	System.Random boyRandom;

	// Use this for initialization
	void Start () {

		TextAsset plistasset = Resources.Load<TextAsset>("ChainReaction");

		Dictionary<string, object> dic = (Dictionary<string, object>)Plist.readPlist(plistasset.bytes);

		girl = new Girl (dic);
		boy = new Boy (dic);

		girlRandom = new System.Random ((int)System.DateTime.UtcNow.Ticks);
		PickFragment (-1, null);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void PickFragment(int index, List<string> fragments)
	{
		if (index >= 0) {
			girl.SelectFragment (fragments [index]);
			Debug.Log ("Damage" + girl.Damage);
		}

		List<string> nextfragments = girl.GetNextFragment ();
		foreach (string key in nextfragments) {
			Debug.Log(key);		
		}


		if (nextfragments.Count > 0) 
		{
			//select next
			int newindex = girlRandom.Next(0, nextfragments.Count);
			PickFragment (newindex, nextfragments);
				
		} else {
			Debug.Log ("Sentence finished");
			Debug.Log(girl.Sentence);
		}
	}
}
