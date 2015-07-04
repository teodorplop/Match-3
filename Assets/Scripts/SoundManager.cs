using UnityEngine;
using System.Linq;
using System.Collections;

public class SoundManager : Singleton<SoundManager> {
	void Start() {
		if (!PlayerPrefs.HasKey ("sound"))
			PlayerPrefs.SetInt ("sound", 1);
		
		Play (SoundType.Music);
	}

	[SerializeField]
	private Sound[] _sounds;
	private GameObject _music;

	public void Play(SoundType type) {
		if (PlayerPrefs.GetInt ("sound") != 1)
			return;

		var clip = _sounds.FirstOrDefault(obj => obj.type == type);
		if (clip == null)
			return;

		var go = new GameObject ();
		var aSource = go.AddComponent<AudioSource> ();
		aSource.clip = clip.sound;
		aSource.Play ();
		if (type == SoundType.Music) {
			aSource.loop = true;
			_music = go;
			DontDestroyOnLoad (go);
		} else {
			Destroy(go, clip.sound.length);
		}
	}
	public void SoundToggle(bool on) {
		PlayerPrefs.SetInt ("sound", on ? 1 : 0);
		Destroy (_music);
		Play (SoundType.Music);
	}
}

public enum SoundType {Music, Select, Match};
[System.Serializable]
class Sound {
	[SerializeField]
	public AudioClip sound;
	[SerializeField]
	public SoundType type;
}