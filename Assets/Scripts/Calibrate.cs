using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrate : MonoBehaviour {

	[SerializeField]
	Transform foot1, foot2, tracker1, tracker2;
	Transform PlaySpace;
	Quaternion delta1 = Quaternion.identity, delta2 = Quaternion.identity; // Match tracker number
	float deltaZ1 = 0, deltaZ2 = 0;
	bool enableCalibration = false;
	[SerializeField] bool enableZAxis = false;

	// Use this for initialization
	void Start () {
		// TODO: Prompt guest to stand flat.
		PlaySpace = transform.root;
	}

	void SwapFeet() {
		Vector3 pos1 = foot1.localPosition, pos2 = foot2.localPosition;
		Quaternion rot1 = foot1.localRotation, rot2 = foot2.localRotation;
		var tmpTrans = foot1.parent;
		foot1.parent = foot2.parent;
		foot2.parent = tmpTrans;
		foot1.localPosition = pos1;
		foot1.localRotation = rot1;
		foot2.localPosition = pos2;
		foot2.localRotation = rot2;
		tmpTrans = foot1;
		foot1 = foot2;
		foot2 = tmpTrans;
	}

	/* Note: y-axis is the direction of foot. Projection dir on floor should not change after calibration */
	Quaternion DeltaRot(Transform tracker) {
		var nowY = tracker.up;
		var newY = Vector3.ProjectOnPlane(nowY, PlaySpace.up).normalized;
		var newRot = Quaternion.LookRotation(PlaySpace.up, nowY);
		var deltaRot = Quaternion.Inverse(tracker.rotation) * newRot;
		return deltaRot;
	}
	void CalculateDelta() {
		delta1 = DeltaRot(tracker1);
		delta2 = DeltaRot(tracker2);
		if (enableZAxis) {
			deltaZ1 = tracker1.localPosition.y - 0.05f;
			deltaZ2 = tracker2.localPosition.y - 0.05f;
		}
		
	}
	void DoCalibrate() {
		foot1.localRotation *= delta1;
		foot2.localRotation *= delta2;
		if (enableZAxis) {
			foot1.parent = tracker1.parent;
			foot2.parent = tracker2.parent;
			Vector3 tmpPos1 = foot1.localPosition;
			tmpPos1.y -= deltaZ1;
			foot1.localPosition = tmpPos1;
			Vector3 tmpPos2 = foot2.localPosition;
			tmpPos2.y -= deltaZ2;
			foot2.localPosition = tmpPos2;
			foot1.parent = tracker1;
			foot2.parent = tracker2;
		}
	}
	void UndoCalibrate() {
		foot1.localRotation *= Quaternion.Inverse(delta1);
		foot2.localRotation *= Quaternion.Inverse(delta2);
		if (enableZAxis) {
			foot1.parent = tracker1.parent;
			foot2.parent = tracker2.parent;
			Vector3 tmpPos1 = foot1.localPosition;
			tmpPos1.y += deltaZ1;
			foot1.localPosition = tmpPos1;
			Vector3 tmpPos2 = foot2.localPosition;
			tmpPos2.y += deltaZ2;
			foot2.localPosition = tmpPos2;
			
			foot1.parent = tracker1;
			foot2.parent = tracker2;
		}
	}
	// Update is called once per frame
	void Update () {
		// if ((!foot1) || (!foot2) || (!tracker1) || (!tracker2)) {
		// 	return;
		// }
		if (Input.GetKeyDown(KeyCode.M)) {
			if (enableCalibration) {
				UndoCalibrate();
			}
			
			SwapFeet();
			if (enableCalibration) {
				DoCalibrate();
			}
		}
		if (Input.GetKeyDown(KeyCode.N)) {
			enableCalibration = !enableCalibration;
			if (enableCalibration) {
				CalculateDelta();
				DoCalibrate();
			}
			else {
				UndoCalibrate();
			}
		}
	}
}
