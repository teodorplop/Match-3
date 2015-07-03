using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScore : MonoBehaviour {
	private Text _scoreText;
	void Awake() {
		_scoreText = GetComponent<Text> ();
		speed = 1;
	}

	private int _score;
	public int Score {
		get { return _score; }
		set {
			_score = value;
			speed = (_score - int.Parse(_scoreText.text.Substring(7))) * 2;
		}
	}

	private int speed;
	void Update() {
		if (int.Parse (_scoreText.text.Substring(7)) < _score) {
			int score = int.Parse(_scoreText.text.Substring(7)) + (int)(speed * Time.deltaTime);
			score = Mathf.Min(_score, score);
			_scoreText.text = "Score: " + score.ToString();
		}
	}
}
