using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The Rocket serves as a manager for all parts of the 
 * rocket. 
 */
public class Rocket : MonoBehaviour {

	float distanceFromComet;

	[SerializeField]
	Transform comet;

	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float getCometDistance() {
		return Vector3.Distance(transform.position, comet.position);
	}
}
