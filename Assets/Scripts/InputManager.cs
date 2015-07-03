using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public static bool inputEnabled;

	private Vector3 _firstPress;
	private Vector3 _secondPress;
	private Vector3 _swipe;
	private bool _swiping;

	void Awake() {
		inputEnabled = true;
	}

	void Update() {
		if (!inputEnabled)
			return;

#if UNITY_STANDALONE_WIN
		if (Input.GetMouseButtonDown(0)) {
			_firstPress = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_swiping = true;
			GameManager.Instance.InputHandler(_firstPress);
		}
		if (Input.GetMouseButtonUp(0)) {
			_swiping = false;
		}

		if (_swiping) {
			_secondPress = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_swipe = _secondPress - _firstPress;

			if ((_swipe.x >= .35f || _swipe.x <= -.35f) && (_swipe.y <= .1f && _swipe.y >= -.1f)) {
				GameManager.Instance.HorizontalSwipe(_swipe.x > 0);
			} else if ((_swipe.y >= .35f || _swipe.y <= -.35f) && (_swipe.x <= .1f && _swipe.x >= -.1f)) {
				GameManager.Instance.VerticalSwipe(_swipe.y > 0);
			}
		}
#endif
	}
}
