using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlistCS;

public class ArgumentRoom : MonoBehaviour {

	public GameObject canvas;
	public static Boy boy;
	public static Girl girl;
	public GameObject boywords;
	public GameObject girlwords;
	List<GameObject> boyobjs = new List<GameObject>();
	List<GameObject> girlobjs = new List<GameObject>();
	static public bool isGirlSelected = false;
	static public bool isBoySelected = false;
	public Text boyText;
	public Text girlText;
	public Slider boySlider;
	public Slider girlSlider;
	// Use this for initialization
	void Start () {

		TextAsset plistasset = Resources.Load<TextAsset>("ChainReaction");
		
		Dictionary<string, object> dic = (Dictionary<string, object>)Plist.readPlist(plistasset.bytes);
		
		girl = new Girl (dic);
		boy = new Boy (dic);

		//CreateFragmentButtons (girl.GetNextFragment (), true);
		//CreateFragmentButtons (boy.GetNextFragment (), false);

		StartCoroutine (StartFromBeginning ());


	}

	IEnumerator StartFromBeginning()
	{
		yield return new WaitForSeconds (2);

		CreateFragmentButtons (girl.GetNextFragment (), true);
		CreateFragmentButtons (boy.GetNextFragment (), false);
	}
	
	// Update is called once per frame
	void Update () {
		boyText.text = boy.Sentence;
		girlText.text = girl.Sentence;

		boySlider.value = boy.NegativeMotionPercent;
		girlSlider.value = girl.PositiveMotionPercent;

		if (isGirlSelected) {
			isGirlSelected = false;
			List<string> fragment = girl.GetNextFragment();
			CreateFragmentButtons (fragment, true);
			if(fragment.Count<=0)
			{
				girl.SpeakTo(boy);

				foreach(GameObject obj in boyobjs)
					Destroy(obj);

				StartCoroutine (StartFromBeginning ());
			}
				
		}

		if (isBoySelected) {
			isBoySelected = false;	
			List<string> fragment = boy.GetNextFragment();
			CreateFragmentButtons (fragment, false);
			if(fragment.Count<=0)
			{
				boy.SpeakTo(girl);

				foreach(GameObject obj in girlobjs)
					Destroy(obj);

				StartCoroutine (StartFromBeginning ());
			}
		}
	}

	void CreateFragmentButtons(List<string> fragments, bool girl)
	{
		//clear old ones
		List<GameObject> objs;
		if (girl)
			objs = girlobjs;
		else
			objs = boyobjs;

		foreach (GameObject obj in objs)
						Destroy (obj);

		objs.Clear ();

		foreach (string frag in fragments) {
						
			GameObject gobj = (GameObject)Instantiate (Resources.Load ("SentenceButton"));
			if(girl && girlwords != null)
			{
				gobj.transform.parent = girlwords.transform;
				girlobjs.Add(gobj);
			}
			else
			{
				gobj.transform.parent = boywords.transform;
				boyobjs.Add(gobj);
			}

			ControlKey ckey = gobj.GetComponent<ControlKey>();
			Button button = gobj.GetComponent<Button>();
			RectTransform rt = gobj.GetComponent<RectTransform>();
			Text buttontext = gobj.GetComponentInChildren<Text>();
			Button.ButtonClickedEvent bevent = new Button.ButtonClickedEvent();
			UnityEngine.Events.UnityAction action = ckey.OnButtonClicked;
			bevent.AddListener(action);
			button.onClick = bevent;


			ckey.isGirl = girl;
			ckey.Key = frag;

			string[] showwords = frag.Split('#');
			string[] finalwords = showwords[0].Split('$');
			buttontext.text = finalwords[0];

			Vector2 newrect = new Vector2(buttontext.preferredWidth+4, buttontext.preferredHeight+1);
			if(newrect.x<20)
				newrect.x = 20;

			rt.sizeDelta = newrect;

			//random position
			Random.seed = (int)System.DateTime.UtcNow.Ticks;
			//Debug.Log("Screen width:"+Screen.width);
			float width = Screen.width*0.5f;
			float height = Screen.height;
			if(!girl)
			{
				Vector3 pos = new Vector3(Random.Range(-width/2.0f+newrect.x, 0.0f-newrect.x), Random.Range(-height/2.0f+newrect.y, height/2.0f-newrect.y), 0.0f);
				button.gameObject.transform.localPosition = pos;
			}
			else
			{
				Vector3 pos = new Vector3(Random.Range(0.0f+newrect.x, width/2.0f-newrect.x), Random.Range(-height/2.0f+newrect.y, height/2.0f-newrect.y), 0.0f);
				button.gameObject.transform.localPosition = pos;
			}

		}
	}



	bool AABBTest(Rect rect1, Rect rect2)
	{
		return false;
	}
}
