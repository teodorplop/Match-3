using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIRoot : MonoBehaviour {
	public void Play() {
		SceneManager.LoadScene("Game");
	}
}
