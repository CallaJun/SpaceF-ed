using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour {

	[SerializeField]
	Transform mothership;
	[SerializeField]
	GameObject ParticleSystem;
	[SerializeField]
	float Stage1Distance, Stage2Distance, Stage3Distance;
	float startDistance, targetDistance, currentDistance;
	[SerializeField] float InterpolateSpeedParameter = 0.1f;
	[SerializeField] float elapsedTime = 0f;
	// Direction to mothership
	Vector3 direction;
	Vector3 randomRotationAxis;
	public float rotationSpeed = 1f;
	public bool IsRotating = true;
	bool Appeared = false;
	Vector3 tmpScale;

	// Sounds
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip cometComingClip;
	[SerializeField] AudioClip hurryClip;
	
	private void Awake() {
		// GetComponent<MeshRenderer>().enabled = false;
		tmpScale = transform.localScale;
		transform.localScale = Vector3.zero;
	}

    // Use this for initialization
    void Start () {
		direction = transform.position - mothership.position;
		direction.Normalize();
		randomRotationAxis = Vector3.ProjectOnPlane(Random.onUnitSphere, direction).normalized;
		
	}
	
	// Update is called once per frame
	void Update () {
		/* Note from Calla: We could also use a coroutine.
		 * But since the comet is just growing closer the entire
		 * time, it might not make much of a differene.
		 */
		// ChaseMothership();
		if (Input.GetKeyDown(KeyCode.I)) {
			CometAppear();
		}
		if (Input.GetKeyDown(KeyCode.O)) {
			ActivateStage2();
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			ActivateStage2();
		}
		if (Appeared) {
			//CometRotate();
			CalculateCurrentDistance();
			UpdateDistance();
		}
		
	}

	public void CometAppear() {
		transform.localScale = tmpScale;
		startDistance = Stage1Distance;
		targetDistance = Stage2Distance;
		elapsedTime = 0;
		Appeared = true;
		ParticleSystem.SetActive(true);
		source.PlayOneShot(cometComingClip);
	}

	public void ActivateStage2() {
		startDistance = currentDistance;
		targetDistance = Stage3Distance;
		rotationSpeed *= 1.5f;
		elapsedTime = 0;
		source.PlayOneShot(hurryClip);
	}

	void CalculateCurrentDistance() {
		float t = Mathf.Exp( -InterpolateSpeedParameter * elapsedTime);
		elapsedTime += Time.deltaTime;
		t = 1 - t;
		currentDistance = Mathf.Lerp(startDistance, targetDistance, t);
		//Debug.Log(elapsedTime.ToString() + " " + currentDistance.ToString());
	}

	void UpdateDistance() {
		var pos = mothership.position + currentDistance * direction;
		transform.position = pos;
	}


	void CometRotate() {
		if (IsRotating) {
			Quaternion newRot = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, randomRotationAxis);
			transform.rotation *= newRot;
			
		}
	}
}
