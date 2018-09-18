using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsulePosition : MonoBehaviour {

	[SerializeField] Transform head, foot1, foot2;
	float targetXPos = 0;
	float radius;
	[SerializeField] Transform CapsuleModel;
	[SerializeField] float threshold = 0.1f;
	[SerializeField] float lag = 0.8f;

	// Use this for initialization
	void Start () {
		// radius = (float)0.5 * CapsuleModel.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		float realXPos = MainControl.Singleton.GetPlayerXPosition();
		if (Mathf.Abs(realXPos - targetXPos) > threshold) {
			if (realXPos > targetXPos) {
				targetXPos = realXPos - threshold;
			}
			else {
				targetXPos = realXPos + threshold;
			}
		}
		var nowXPos = transform.localPosition.x;
		SetXPosAt(Mathf.Lerp(nowXPos, targetXPos, 1 - lag));
	}

	void SetXPosAt(float x) {
		// Debug.Log("Setting at " + x);
		// float rotDegree = x / radius / Mathf.PI * 180;
		Vector3 pos = transform.localPosition;
		pos.x = x;
		Quaternion rot = Quaternion.Euler(0, 0, MainControl.Singleton.GetPlayerRotDegree());
		transform.localRotation = rot;
		transform.localPosition = pos;
	}
}
