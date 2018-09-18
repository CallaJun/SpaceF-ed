using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
public class Act0Controller : MonoBehaviour {
	AsyncOperation asyncLoad;
	RawImage image;
	VideoPlayer video;
	public AudioSource audio;
	public AudioClip clip;
	
	

	// Use this for initialization
	void Start () {
		audio.PlayOneShot(clip);
	}
	
	// Update is called once per frame
	void Update () {
		if (!audio.isPlaying) {
			SceneFinished();
		}
	}

	void SceneFinished() {
		
		// asyncLoad = SceneManager.LoadSceneAsync("Act1_CometComing");
		GetComponent<SteamVR_LoadLevel>().Trigger();
		// Destroy(gameObject);
	}
}
