using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Utils;

///<summary>
/// 	Clasa <c> CandyMatrix </c> genereaza matricea bomboanelor
/// 	si realizeaza principalele functionalitati ce sunt in directa
/// 	legatura cu aceasta
/// </summary>

public class CandyMatrix : MonoBehaviour {
	private int _rows = 8;
	private int _columns = 8;
	private Vector3[][] _placeHolders;
	private Candy[][] _candies;

    /// <remarks>
    /// 	Aceasta functie se apeleaza atunci 
    /// 	cand obiectul devine <para> enabled </para>
    /// 	<see cref = "Awake"/> 
    /// </remarks>
    /// <summary>
    /// 	Metoda intitializeaza vectorii
    /// </summary>
    /// <members>
    /// <member name = "_placeHolders">
    /// 	<summary>
    /// 		Retine matricea cu pozitiile (abscisa si ordonata) 
    /// 		la care sunt plasate bomboanele
    /// 	</summary>
    /// </member>
    /// <member name = "_candies">
    /// 	<summary>
    /// 		Retine matricea cu instante ale
    /// 		bomboanelor prezente in joc
    /// 	</summary>
    /// </member>
    /// </members>
	void Awake() {
		
		_placeHolders = new Vector3[_rows][];
		for (int i = 0; i < _rows; ++i)
			_placeHolders [i] = new Vector3[_columns];
		_candies = new Candy[_rows][];
		for (int i = 0; i < _rows; ++i)
			_candies [i] = new Candy[_columns];
	}
    /// <remarks>
    /// 	Aceasta functie se apeleaza 
    /// 	la activarea scriptului
    /// 	Are scopul de a crea matricea
    /// </remarks>
	void Start() {
		InitializePlaceHolders();
		CreateMatrix();
		//StartCoroutine(Shuffle (GetAllCandies ()));
	}

	private Vector2 _leftUpCorner = new Vector2(-4f, 4f);
	private float _cellDimension = 1.1f;
	void InitializePlaceHolders() {
		for (int i = 0; i < _rows; ++i) {
			for (int j = 0; j < _columns; ++j) {
				_placeHolders[i][j] = new Vector3(_leftUpCorner.x + j * _cellDimension, _leftUpCorner.y - i * _cellDimension);
			}
		}
	}
	
	void CreateMatrix() {
		for (int i = 0; i < _rows; ++i) {
			for (int j = 0; j < _columns; ++j) {
				CandyType newCandyType = RandomGenerator.NextCandy();
				while (CheckHorizontal(newCandyType, i, j) || CheckVertical (newCandyType, i, j))
					newCandyType = RandomGenerator.NextCandy();
			
				_candies[i][j] = ((GameObject)Instantiate(Resources.Load(newCandyType.ToString()), _placeHolders[i][j], Quaternion.identity)).GetComponent<Candy>();
				_candies[i][j].gameObject.name = i.ToString() + "_" + j.ToString();
				_candies[i][j].transform.parent = transform;
			}
		}
	}

	private Candy _selected;
	public Candy SelectedCandy {
		get { return _selected; }
	}
	public void Select(Candy candy) {
		if (_selected == candy)
			return;

		SoundManager.Instance.Play (SoundType.Select);

		if (_selected) {
			var prevPosition = CandyToMatrixPosition (_selected);
			var position = CandyToMatrixPosition (candy);

			if (Adjacent (prevPosition, position)) {
				_selected.Select(false);
				_selected = null;
				StartCoroutine(Swap(prevPosition, position));
			} else {
				_selected.Select(false);
				_selected = candy;
				_selected.Select(true);
			}
		} else {
			_selected = candy;
			_selected.Select (true);
		}
	}
	public void HorizontalSwipe(bool positive) {
		if (!_selected)
			return;
		var position = CandyToMatrixPosition (_selected);
		var nextPosition = position;
		nextPosition.second += positive ? 1 : -1;
		if (IsInMatrix (nextPosition))
			Select (_candies [nextPosition.first] [nextPosition.second]);
	}
	public void VerticalSwipe(bool positive) {
		if (!_selected)
			return;
		var position = CandyToMatrixPosition (_selected);
		var nextPosition = position;
		nextPosition.first += positive ? -1 : 1;
		if (IsInMatrix (nextPosition))
			Select (_candies [nextPosition.first] [nextPosition.second]);
	}

	private IEnumerator Swap(Pair<int, int> pos1, Pair<int, int> pos2) {
		InputManager.inputEnabled = false;

		var candy1 = _candies [pos1.first] [pos1.second];
		var candy1Pos = _placeHolders [pos1.first] [pos1.second];
		var candy2 = _candies [pos2.first] [pos2.second];
		var candy2Pos = _placeHolders [pos2.first] [pos2.second];

		candy1.MoveToAnimation (candy2Pos);
		candy2.MoveToAnimation (candy1Pos);

		while (candy1.AnimationPlaying() || candy2.AnimationPlaying())
			yield return null;

		ApplySwap (pos1, pos2);

		if (!CheckForMatches ()) {
			candy1.MoveToAnimation(candy1Pos);
			candy2.MoveToAnimation(candy2Pos);

			while (candy1.AnimationPlaying() || candy2.AnimationPlaying())
				yield return null;

			ApplySwap(pos1, pos2);
		} else {
			yield return StartCoroutine(DestroyCandies());
		}

		InputManager.inputEnabled = true;
	}
	private void ApplySwap(Pair<int, int> pos1, Pair<int, int> pos2) {
		var aux = _candies [pos1.first] [pos1.second];
		_candies [pos1.first] [pos1.second] = _candies [pos2.first] [pos2.second];
		_candies [pos2.first] [pos2.second] = aux;

		_candies [pos1.first] [pos1.second].gameObject.name = pos1.first.ToString () + "_" + pos1.second.ToString ();
		_candies [pos2.first] [pos2.second].gameObject.name = pos2.first.ToString () + "_" + pos2.second.ToString ();
	}

	private List<Candy> GetHorizontalMatches(Candy candy) {
		var candyPosition = CandyToMatrixPosition(candy);

		int left = candyPosition.second - 1;
		while (left >= 0 && _candies[candyPosition.first][left].Type == candy.Type)
			--left;
		++left;
		int right = candyPosition.second + 1;
		while (right < _columns && _candies[candyPosition.first][right].Type == candy.Type)
			++right;
		--right;


		var candies = new List<Candy> ();
		if (right - left + 1 >= 3) {
			for (int j = left; j <= right; ++j) {
				candies.Add(_candies[candyPosition.first][j]);
			}
		}
		return candies;
	}
	private List<Candy> GetVerticalMatches(Candy candy) {
		var candyPosition = CandyToMatrixPosition(candy);

		int top = candyPosition.first - 1;
		while (top >= 0 && _candies[top][candyPosition.second].Type == candy.Type)
			--top;
		++top;
		int bottom = candyPosition.first + 1;
		while (bottom < _rows && _candies[bottom][candyPosition.second].Type == candy.Type)
			++bottom;
		--bottom;

		var candies = new List<Candy> ();
		if (bottom - top + 1 >= 3) {
			for (int i = top; i <= bottom; ++i) {
				candies.Add(_candies[i][candyPosition.second]);
			}
		}
		return candies;
	}
	private List<Candy> GetCandiesToDestroy() {
		var candyHash = new HashSet<Candy> ();
		for (int i = 0; i < _rows; ++i) {
			for (int j = 0; j < _columns; ++j) {
				candyHash.UnionWith(GetHorizontalMatches(_candies[i][j]));
				candyHash.UnionWith(GetVerticalMatches(_candies[i][j]));
			}
		}
		return candyHash.ToList ();
	}
	private bool CheckForMatches() {
		for (int i = 0; i < _rows; ++i) {
			for (int j = 0; j < _columns; ++j) {
				if (CheckHorizontal(_candies[i][j].Type, i, j) || CheckVertical(_candies[i][j].Type, i, j))
					return true;
			}
		}
		return false;
	}
	bool CheckHorizontal(CandyType candyType, int row, int column) {
		return (column >= 2 && candyType == _candies [row] [column - 1].Type && candyType == _candies [row] [column - 2].Type);
	}
	bool CheckVertical(CandyType candyType, int row, int column) {
		return (row >= 2 && candyType == _candies [row - 1] [column].Type && candyType == _candies [row - 2] [column].Type);
	}

	private IEnumerator DestroyCandies() {
		var candies = GetCandiesToDestroy();
		if (candies.Count != 0) {
			SoundManager.Instance.Play (SoundType.Match);

			foreach (var candy in candies) {
				candy.DestroyAnimation ();
			}
			while (candies[0].AnimationPlaying())
				yield return null;
			foreach (var candy in candies) {
				Destroy (candy.gameObject);
			}
			yield return null;

			StartCoroutine (FillGaps ());
			yield return new WaitForSeconds (.4f);
			yield return StartCoroutine (DestroyCandies ());
		} else {
			if(needToShuffle()) 
				yield return StartCoroutine (Shuffle (GetAllCandies ()));
		}
	}
	private IEnumerator DestroyMatrix() {
		for (int i = 0; i < _rows; ++i) 
			for(int j = 0; j < _columns; ++j) {
				_candies[i][j].DestroyAnimation ();
			}
		while (_candies[0][0].AnimationPlaying())
			yield return null;
		for (int i = 0; i < _rows; ++i) 
			for(int j = 0; j < _columns; ++j){
				Destroy (_candies[i][j].gameObject);
		}
		yield return null;
	}

	private List<Candy> GetAllCandies() {
		List<Candy> candies = new List<Candy>();
		for (int i = 0; i < _rows; ++i) 
			for(int j = 0; j < _columns; ++j) {
				candies.Add(_candies[i][j]);
			}
		return candies;
	}

	private bool needToShuffle() {
		for (int i = 0; i < _rows; ++i)
			for (int j = 0; j < _columns; ++j) {
				if (IsInMatrix (new Pair<int, int> (i, j + 1)) && (validMove (_candies [i] [j], i, j + 1) || validMove (_candies [i] [j + 1], i, j)))
					return false;
				if (IsInMatrix (new Pair<int, int> (i + 1, j)) && (validMove (_candies [i] [j], i + 1, j) || validMove (_candies [i + 1] [j], i, j)))
					return false;
			}
		return true;
	}

	bool validMove(Candy candy, int row, int column) {
		if (column >= 2 && candy.Type == _candies [row] [column - 1].Type 
		    	&& candy.Type == _candies [row] [column - 2].Type)
		    return true;
		if (column + 2 < _columns && candy.Type == _candies [row] [column + 1].Type
		    	&& candy.Type == _candies [row] [column + 2].Type)
			return true;
		if (column + 1 < _columns && column >= 1 && candy.Type == _candies [row] [column - 1].Type
		    	&& candy.Type == _candies [row] [column + 1].Type)
			return true;
		if (row >= 2 && candy.Type == _candies [row - 1] [column].Type && 
		    	candy.Type == _candies [row - 2] [column].Type)
			return true;
		if (row + 2 < _columns && candy.Type == _candies [row + 1] [column].Type && 
		    	candy.Type == _candies [row + 2] [column].Type)
			return true;
		if (row + 1 < _columns && row >= 1 && candy.Type == _candies [row - 1] [column].Type 
		    	&& candy.Type == _candies [row + 1] [column].Type)
			return true;
		return false;
	}

	private IEnumerator Shuffle(List<Candy> candies) {
		List<Candy> aux = new List<Candy>(candies);
		int maxNumberOfTry = 20;
		yield return new WaitForSeconds(0.4f);
		
		for (int i = 0; i < _rows; ++i)
			for (int j = 0; j < _columns; ++j) {
				int nrOfTry = 0;
				
				int index = UnityEngine.Random.Range(0, candies.Count);
				while( nrOfTry++ < maxNumberOfTry && 
			      			(CheckHorizontal(candies[index].Type, i, j) || CheckVertical (candies[index].Type, i, j))) 
					index = UnityEngine.Random.Range(0, candies.Count);
				
				if(nrOfTry == maxNumberOfTry)
					Shuffle (aux);
				
				_candies[i][j] = candies[index];
				_candies[i][j].gameObject.name = i.ToString() + "_" + j.ToString();
				_candies[i][j].transform.position = _placeHolders[i][j];
				candies.RemoveAt (index);

			yield return StartCoroutine (FillGaps ());

			if (CheckForMatches())
				yield return StartCoroutine (DestroyCandies ());

		}
	}
	


	// condenseaza gaurile (coboara bomboanele de sus in locul nullurilor) si umple random ce ramane

	private IEnumerator FillGaps() {
		for (int j = 0; j < _columns; ++j) {
			for (int i = _rows - 1; i >= 0; --i) {
				if (_candies[i][j] != null)
					continue;

				Pair<int, int> replacePosition = FindNearestOccupiedPosition (new Pair<int, int> (i, j));	
				if (replacePosition != null) {
					Candy replaceCandy = _candies [replacePosition.first] [replacePosition.second];
					if (replaceCandy != null) {
						_candies [i] [j] = replaceCandy;
						_candies [i] [j].FallAnimation (_placeHolders [i] [j]);
						_candies [i] [j].gameObject.name = i.ToString () + "_" + j.ToString ();

						_candies [replacePosition.first] [replacePosition.second] = null;
					}
				}
			}
		}

		float[] heights = new float[_columns];
		for (int j = 0; j < _columns; ++j)
			heights [j] = _placeHolders [0] [0].y;

		for (int j = 0; j < _columns; ++j) {
			for (int i = _rows - 1; i >= 0; --i) {
				if (_candies[i][j] != null)
					continue;

				heights[j] += _cellDimension;

				var newCandyType = RandomGenerator.NextCandy();
				var position = new Vector3(_placeHolders[i][j].x, heights[j]);
				_candies[i][j] = ((GameObject)Instantiate(Resources.Load(newCandyType.ToString()), position, Quaternion.identity)).GetComponent<Candy>();
				_candies[i][j].gameObject.name = i.ToString() + "_" + j.ToString();
				_candies[i][j].transform.parent = transform;
				_candies[i][j].FallAnimation(_placeHolders[i][j]);
			}
		}

		// TODO: Maybe change this so it waits for the animations to finish
		yield return new WaitForSeconds (.55f);
	}

	private Pair<int, int> FindNearestOccupiedPosition(Pair<int, int> position) {
		while (IsInMatrix (position) && _candies[position.first][position.second] == null) {
			--position.first;
		}
		if (IsInMatrix (position))
			return new Pair<int, int>(position.first, position.second);
		return null;
	}

	#region Helpers
	private Pair<int, int> CandyToMatrixPosition(Candy candy) {
		string[] strings = candy.gameObject.name.Split('_');
		return new Pair<int, int>(int.Parse(strings[0]), int.Parse(strings[1]));
	}
	private bool Adjacent(Pair<int, int> pos1, Pair<int, int> pos2) {
		return Mathf.Abs (pos1.first - pos2.first) + Mathf.Abs (pos1.second - pos2.second) == 1;
	}
	private bool IsInMatrix(Pair<int, int> position) {
		return position.first >= 0 && position.first < _rows && position.second >= 0 && position.second < _columns;
	}
	#endregion
}
