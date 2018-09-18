using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollider : MonoBehaviour {

	public bool active;
	[SerializeField] Collider foot;
	// Use this for initialization
	void Start () {
		active = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		// TODO: Give each foot the tag "Foot"
		if (other.CompareTag("Foot")) {
			active = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Foot")) {
			active = false;
		}
	}
}
