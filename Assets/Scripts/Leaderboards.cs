using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Leaderboards : MonoBehaviour {
	[SerializeField]
	private GameObject[] _scores;

	void Start() {
		for (int i = 0; i < 3; ++i) {
			if (PlayerPrefs.HasKey("score" + i.ToString())) {
				_scores[i].transform.FindChild("Text_Name").GetComponent<Text>().text = Environment.UserName;
				_scores[i].transform.FindChild("Text_Score").GetComponent<Text>().text = (PlayerPrefs.GetInt("score" + i.ToString())).ToString();
			}
		}
	}

	public static void ReportScore(int score) {
		for (int i = 0; i < 3; ++i) {
			if (!PlayerPrefs.HasKey ("score" + i.ToString ()) || PlayerPrefs.GetInt ("score" + i.ToString ()) < score) {
				PlayerPrefs.SetInt ("score" + i.ToString (), score);
				break;
			}
		}
	}
}
