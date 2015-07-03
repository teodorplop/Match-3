using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	private static GameManager _instance;
	public static GameManager Instance {
		get { return _instance; }
	}
	void Awake() {
		if (_instance == null) {
			_instance = this;
		} else {
			Debug.LogError(name + " already instantiated.");
			Destroy(gameObject);
		}
	}
	
	[SerializeField]
	private CandyMatrix _candyMatrix;
	#region INPUT
	public void InputHandler(Vector3 position) {
		var candy = ScanFor<Candy> (position);
		if (candy) {
			_candyMatrix.Select(candy);
		}
	}
	public void HorizontalSwipe(bool positive) {
		_candyMatrix.HorizontalSwipe (positive);
	}
	public void VerticalSwipe(bool positive) {
		_candyMatrix.VerticalSwipe (positive);
	}
	#endregion

	[SerializeField]
	private UIScore _score;
	[SerializeField]
	private UITimeLimit _timer;
	public void CandyDestroyed() {
		_score.Score += 30;
		_timer.AddTime();
	}
	public void GameEnded() {
		_timer.TimerEnabled (false);
		InputManager.inputEnabled = false;
		StartCoroutine(EndGameAnimation());
	}

	[SerializeField]
	private GameObject _nextLevelButton;
	IEnumerator EndGameAnimation() {
		Destroy (_candyMatrix.gameObject);
		Destroy (_timer.gameObject);
		yield return new WaitForSeconds (.25f);

		TweenPosition.Begin (_score.gameObject,
		                     _score.transform.localPosition,
		                     Vector3.zero,
		                     1f);
		TweenScale.Begin (_score.gameObject,
		                  _score.transform.localScale,
		                  new Vector3 (2f, 2f, 2f),
		                  1f);
		while (TweenPosition.IsPlaying(_score.gameObject))
			yield return null;
		yield return new WaitForSeconds (.25f);
		
		
		Leaderboards.ReportScore(_score.Score);
		_nextLevelButton.SetActive (true);
	}
	public void NextLevel() {
		Application.LoadLevel ("Game");
	}
	public void Back() {
		Application.LoadLevel ("menu");
	}

	T ScanFor<T>(Vector3 position) {
		RaycastHit2D hitInfo = Physics2D.Raycast (position, Vector2.zero);
		if (hitInfo) {
			return hitInfo.collider.GetComponent<T> ();
		}
		return default(T);
	}
}
