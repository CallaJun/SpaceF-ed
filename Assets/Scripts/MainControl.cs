using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {
	public static MainControl Singleton;
	public bool InDevelopment = false;
	public float radius = 3f;
	[SerializeField, Range(1, 2)] float speedMultiplier = 1.3f;
	public Transform Player;

	public float GetPlayerXPosition() {
		Transform oldParent = Player.transform.parent;
		Player.SetParent(transform);
		float ret = Player.localPosition.x;
		Player.SetParent(oldParent);
		return ret;
	}
	public float GetPlayerRotDegree() {
		float xPos = GetPlayerXPosition();
		return  xPos / radius * 180 / Mathf.PI * speedMultiplier;
	}
	private void Awake() {
		Singleton = this;	
		if (!InDevelopment) {
			Quaternion euler = Quaternion.Euler(90, 0, 0);
			transform.rotation = euler;
		}
		
	}

	// Use this for initialization
	void Start () {
		// If we are editing the game then the playground won't rotate
		// just to make development easier
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void LateUpdate() {
		// rotate the skybox to adjust to player's position
		float rotDegree = MainControl.Singleton.GetPlayerRotDegree();
		RenderSettings.skybox.SetFloat("_Rotation", rotDegree);
	}
}
