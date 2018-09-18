using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour {

	Vector3 amplitude;
	Vector3 frequency;
	Vector3 positionOffset = new Vector3();
	Vector3 oriPosition = new Vector3();
    float elapsedTime = 0;


	// Use this for initialization
	void Start () {
		amplitude = new Vector3(1, 1, 0.2f);
		frequency = new Vector3( Random.Range(0.1f, 0.5f),  Random.Range(0.1f, .5f),  Random.Range(0.1f, .5f));
		oriPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

		elapsedTime += Time.deltaTime;
		var sinVal = elapsedTime * Mathf.PI * frequency;
        var deltaPos = new Vector3(Mathf.Sin(sinVal.x), Mathf.Sin(sinVal.y), Mathf.Sin(sinVal.z));
        positionOffset = Vector3.Scale(deltaPos, amplitude);
        // Debug.Log(positionOffset.x.ToString() + " " + positionOffset.y.ToString() + " " + positionOffset.z.ToString());
        transform.localPosition = oriPosition + positionOffset;
	}

}
