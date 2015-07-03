using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITimeLimit : MonoBehaviour {
	private Text _text;
	void Awake() {
		_text = GetComponent<Text> ();
		_isEnabled = true;
	}
	void Start() {
		ResetTimer ();
	}

	[SerializeField]
	private float _startingTime;
	[SerializeField]
	private float _maximumTime;

	private bool _isEnabled;
	private float _timer;
	public void TimerEnabled(bool enabled) {
		_isEnabled = enabled;
	}
	public void ResetTimer() {
		SetTimer (_startingTime);
		_addTime = _startingAddTime;
	}
	public void SetTimer(float value) {
		_timer = value;
		_text.text = "Time Left: " + ((int)value).ToString ();
	}

	[SerializeField]
	private float _startingAddTime;
	[SerializeField]
	private float _minimumAddTime;
	[SerializeField]
	private float _addTimeDecay;

	private float _addTime;
	public void AddTime() {
		SetTimer (_timer + _addTime);
		_addTime -= _addTimeDecay;
		_addTime = Mathf.Max (_minimumAddTime, _addTime);
	}

	void Update() {
		if (_isEnabled) {
			SetTimer(_timer - Time.deltaTime);
			_timer = Mathf.Clamp(_timer, 0f, _maximumTime);

			if (_timer == 0f) {
				GameManager.Instance.GameEnded();
			}
		}
	}
}
