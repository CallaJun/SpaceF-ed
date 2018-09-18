using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothership : MonoBehaviour {

    public MothershipController ctrl;
    public EscapeMovement escape;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    void Explode()
    {
        ctrl.Explode();
        escape.GoAwayForever();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Explode();
    }
}
