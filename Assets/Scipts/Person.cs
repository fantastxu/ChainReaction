using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person
{
	bool _isNegative;
	float _negativePercent; //between 0.0f - 100.0f

	HashSet<string> _responsedSentences = new HashSet<string> ();

	Dictionary<string, object> _sentecneDic = new Dictionary<string, object>();//sentence root collection from plist file 

	Dictionary<string, object> _firstFragmentDic = new Dictionary<string, object>();//first fragment we can use, object is another Dictionary<string, object>, key format is 1-XX:XX
	List<object> _sentenceLimit = new List<object>();
	float _initDamage = 0.0f;
	float _fragmentNum = 0.0f;
	string _finalSentence = "";

	Stack<Dictionary<string, object>> _sentenceStack = new Stack<Dictionary<string, object>> ();

	public Person(Dictionary<string, object> dic, bool girl)
	{
		_sentecneDic = dic;
		_isNegative = girl;
		if (_isNegative)
			_negativePercent = 100;
		else
			_negativePercent = 0;
	}

	public string Sentence
	{
		get { return _finalSentence;}
	}

	public float Damage
	{
		get { return _initDamage * _fragmentNum;}
	}

	public bool IsNegative
	{
		get { return _isNegative;}
	}

	public float NegativeMotionPercent
	{
		get { return _negativePercent;}
	}

	public float PositiveMotionPercent
	{
		get { return 100.0f - NegativeMotionPercent;}
	}


	void GetFirstFragment()
	{
		if (_sentecneDic == null)
			return;

		foreach (string keygroup in _sentecneDic.Keys) {
			//check if we have _responsedSentences limit, we can only select sentences from _responsedSentences hashtable
			if(!_responsedSentences.Contains(keygroup) && _responsedSentences.Count>0)
				continue;


			_firstFragmentDic.Clear();
			string firstkeystring = keygroup+"-";
			Dictionary<string, object> firstdic = (Dictionary<string, object>)_sentecneDic[keygroup];
			foreach(string percentkey in firstdic.Keys)
			{

				if(percentkey == "Trigger")
					continue;

				//split key into 2 string, first is percent
				string[] percents = percentkey.Split(':');
				float percentfloat = System.Convert.ToSingle(percents[0]);
				if((IsNegative == true && percentfloat<= NegativeMotionPercent) ||
				   (IsNegative == false && percentfloat <= PositiveMotionPercent))
				{
					string secondkeystring = firstkeystring + percentkey;
					//recored all subject key 
					Dictionary<string, object> firstfragdic = (Dictionary<string, object>)firstdic[percentkey];
					foreach(string subjectkey in firstfragdic.Keys)
					{
						string firstfragmentstring = subjectkey+"$"+secondkeystring;
						_firstFragmentDic.Add(firstfragmentstring,firstfragdic[subjectkey]); 
					}
				}
			}
		}
	}


	public List<string> GetNextFragment()
	{

		List<string> fragments = new List<string> ();
		if (_sentenceStack.Count == 0) {
			//try to get first fragment
			GetFirstFragment();
			foreach(string key in _firstFragmentDic.Keys)
				fragments.Add(key);

		}else{
			Dictionary<string, object> dictemp = _sentenceStack.Peek();
			foreach(string key in dictemp.Keys)
			{
				fragments.Add(key);
			}
		}

		return fragments;
	}

	public void Reset()
	{
		ClearSentense ();
		_responsedSentences.Clear ();
	}

	public bool SelectFragment(string key)
	{
		//key is keyfragment$1-XX:XX
		if (key.Contains ("$")) {
			//this is first fragment
			//1.get top key from key, top key format is 1-XX:XX, which 1 is number group, XX:XX mean percent and damage
			if(_firstFragmentDic.ContainsKey(key))
			{

				_sentenceStack.Clear();
				//2.split key string by $
				string[] kg1 = key.Split('$');
				_finalSentence = kg1[0];
				//3.split 
				string[] kg2 = kg1[1].Split('-');
				//3.5 get limitation number
				_sentenceLimit.Clear();
				if(_sentecneDic.ContainsKey(kg2[0]))
				{
					Dictionary<string, object> numberdic = (Dictionary<string, object>)_sentecneDic[kg2[0]];
					_sentenceLimit = (List<object>)numberdic["Trigger"];
				}

				//4.get damage
				string[] kg3 = kg2[1].Split(':');
				_initDamage =  System.Convert.ToSingle(kg3[1]);
				_fragmentNum = 1.0f;

				Dictionary<string, object> selectdic = (Dictionary<string, object>)_firstFragmentDic[key];
				_sentenceStack.Push(selectdic);
				return true;
			}
		}
		else if (_sentenceStack.Count > 0) {
			Dictionary<string, object> dictemp = _sentenceStack.Peek();
			if(dictemp.ContainsKey(key))
			{
				//..Debug.Log(dictemp[key].GetType().ToString());
				object newfragment = dictemp[key];
				if(newfragment is System.String)
				{
					//last fragment
					//push empty to stack
					_sentenceStack.Push(new Dictionary<string, object>());

				}
				else
					_sentenceStack.Push((Dictionary<string, object>)dictemp[key]);

				_fragmentNum += 1.0f;
				_finalSentence = _finalSentence.Replace("#","");
				_finalSentence += " ";
				_finalSentence += key;
				return true;
			}
			else
				return false;
		}

		return false;
	}

	public bool IsCompleteSentence()
	{
		if (_finalSentence != null && _finalSentence.Contains ("#"))
			return true;

		return false;
	}

	public void SpeakTo(Person p)
	{
		if (p != this)
			p.Heard (Damage, _sentenceLimit);
	}

	public void Heard(float damage, List<object> limit)
	{
		//interrupt current sentence making
		_responsedSentences.Clear ();
		foreach(object groupnum in limit)
			_responsedSentences.Add((string)groupnum);

		_firstFragmentDic.Clear ();
		_sentenceLimit.Clear ();
		_sentenceStack.Clear ();
		_initDamage = 0.0f;
		_fragmentNum = 0.0f;
		_finalSentence = "";

		if (_isNegative) {
			_negativePercent -= damage;		
		}
		else
		{
			_negativePercent += damage;
		}


	}



}

public class Girl : Person
{
	public Girl(Dictionary<string, object> rootdic) : base( (Dictionary<string, object>)rootdic["Girl"], true)
	{

	}
}

public class Boy : Person
{
	public Boy (Dictionary<string, object> rootdic) : base( (Dictionary<string, object>)rootdic["Boy"], false)
	{

	}
}
