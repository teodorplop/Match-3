using UnityEngine;
using System.Collections;

public partial class Candy {
	public bool AnimationPlaying() {
		return TweenPosition.IsPlaying(gameObject) || TweenScale.IsPlaying(gameObject);
	}

	public void MoveToAnimation(Vector3 position) {
		TweenPosition.Begin (gameObject, transform.localPosition, position, .2f, EaseType.EaseInOutQuad);
	}
	public void FallAnimation(Vector3 position) {
		TweenPosition.Begin (gameObject, transform.localPosition, position, .5f, EaseType.EaseInQuad);
	}
	public void DestroyAnimation() {
		TweenScale.Begin (gameObject, transform.localScale, new Vector3 (.1f, .1f), .2f, EaseType.EaseInQuad);
	}

	void OnDestroy() {
		GameManager.Instance.CandyDestroyed ();
	}
}
