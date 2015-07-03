using UnityEngine;
using System.Collections;

public enum CandyType {Croissant, Cupcake, Danish, Donut, Macaroon, SugarCookie};

public partial class Candy : MonoBehaviour {
	private SpriteRenderer _sprite;
	void Awake() {
		_sprite = GetComponent<SpriteRenderer> ();
	}

	[SerializeField]
	private CandyType _type;
	public CandyType Type {
		get { return _type; }
		set { _type = value; }
	}

	[SerializeField]
	private Sprite _normal;
	[SerializeField]
	private Sprite _highlighted;
	public void Select(bool selected) {
		_sprite.sprite = selected ? _highlighted : _normal;
	}
}
