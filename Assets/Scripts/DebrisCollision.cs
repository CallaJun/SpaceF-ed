using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log("debrreiieeissleg");
            StartCoroutine(BlackOut.singleton.ChangeIntensity(1, 1.0f));
        }
    }
}
