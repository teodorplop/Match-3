using UnityEngine;

public enum EaseType {
	Linear,
	EaseInQuad,
	EaseOutQuad,
	EaseInOutQuad
}

public class TweenPosition : MonoBehaviour {
	public static void Begin(GameObject target, Vector3 from, Vector3 to, float duration, EaseType easeType = EaseType.Linear) {
		var tweenPos = target.GetComponent<TweenPosition>();
		if (tweenPos == null)
			tweenPos = target.AddComponent<TweenPosition>();
		tweenPos.Begin(from, to, duration, easeType);
	}
	public static bool IsPlaying(GameObject target) {
		return target.GetComponent<TweenPosition>() != null;
	}

	private Vector3 _from;
	private Vector3 _distance;
	private float _duration;
	private float _timePassed;
	private EaseType _easeType;
	
	void Begin(Vector3 from, Vector3 to, float duration, EaseType easeType) {
		this._from = from;
		this._distance = to - from;
		this._duration = duration;
		this._easeType = easeType;
		this._timePassed = 0f;
		
		if (duration == 0f) {
			transform.localPosition = to;
		} else {
			transform.localPosition = from;
			_timePassed = 0f;
		}
	}
	
	void Update() {
		_timePassed += Time.deltaTime;
		if (_timePassed >= _duration) {
			_timePassed = _duration;
		}
		transform.localPosition = EaseFunction();
		if (_timePassed >= _duration)
			Destroy (this);
	}

	Vector3 EaseFunction() {
		float timeFactor;
		switch (_easeType) {
		case EaseType.Linear:
			timeFactor = _timePassed / _duration;
			return _from + timeFactor * _distance;
		case EaseType.EaseInQuad:
			timeFactor = _timePassed / _duration;
			return _from + timeFactor * timeFactor * _distance;
		case EaseType.EaseOutQuad:
			timeFactor = _timePassed / _duration;
			return _from + timeFactor * (timeFactor - 2f) * -_distance;
		case EaseType.EaseInOutQuad:
			timeFactor = _timePassed / (_duration / 2f);
			if (timeFactor < 1) {
				return _from + timeFactor * timeFactor * _distance / 2f;
			}
			--timeFactor;
			return _from + (timeFactor * (timeFactor - 2f) - 1f) * -_distance / 2f;
		}
		return _from;
	}
}