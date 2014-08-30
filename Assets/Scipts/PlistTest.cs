using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlistCS;

public class PlistTest : MonoBehaviour {

	Girl girl;
	Boy boy;
	System.Random personRandom;
	bool girlTurn = true;
	int times = 0;

	// Use this for initialization
	void Start () {

		TextAsset plistasset = Resources.Load<TextAsset>("ChainReaction");

		Dictionary<string, object> dic = (Dictionary<string, object>)Plist.readPlist(plistasset.bytes);

		girl = new Girl (dic);
		boy = new Boy (dic);

		personRandom = new System.Random ((int)System.DateTime.UtcNow.Ticks);


	}
	
	// Update is called once per frame
	void Update () {

		if (times > 4)
			return;

		if (girlTurn) {
			PickFragment (girl, -1, null);
			if(girl.IsCompleteSentence())
			{

				Debug.Log("Girl damage:"+girl.Damage);
				girl.SpeakTo(boy);
				Debug.Log("Boy motion:"+boy.PositiveMotionPercent);

			}

		} else {
			PickFragment(boy, -1, null);
			if(boy.IsCompleteSentence())
			{
				Debug.Log("Boy damage:"+boy.Damage);
				boy.SpeakTo(girl);
				Debug.Log("Girl motion:"+girl.NegativeMotionPercent);
			}
		}
		
		girlTurn = !girlTurn;
		times++;
	}

	void PickFragment(Person persion, int index, List<string> fragments)
	{
		if (index >= 0) {
			persion.SelectFragment (fragments [index]);
			//..Debug.Log ("Damage" + persion.Damage);
		}

		List<string> nextfragments = persion.GetNextFragment ();
		foreach (string key in nextfragments) {
			//..Debug.Log(key);		
		}


		if (nextfragments.Count > 0) 
		{
			//select next
			int newindex = personRandom.Next(0, nextfragments.Count);
			PickFragment (persion, newindex, nextfragments);
				
		} else {
			//..Debug.Log ("Sentence finished");
			Debug.Log(persion.Sentence);
		}
	}
}
