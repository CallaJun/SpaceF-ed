using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour {

	Vector3 amplitude;
	Vector3 frequency;
	Vector3 positionOffset = new Vector3();
	Vector3 oriPosition = new Vector3();

    public Material deactivatedMat;
    public Material activatedMat;

    public Light glow;
    public Renderer renderer;
    float elapsedTime = 0;
    bool wasPickedUpLastFrame = false;


	// Use this for initialization
	void Start () {
		amplitude = new Vector3(Random.Range(0.02f, 0.1f), Random.Range(0.02f, 0.1f), Random.Range(0.02f, 0.1f));
		frequency = new Vector3( Random.Range(0.1f, 0.5f),  Random.Range(0.1f, .5f),  Random.Range(0.1f, .5f));
		oriPosition = transform.localPosition;
        renderer = GetComponentInChildren<Renderer>();
        glow.intensity = 0;
        Activate();
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameObject.GetComponent<InteractableItem>().isPickedUp) {
            if (wasPickedUpLastFrame) {
                oriPosition = transform.localPosition;
            }
            elapsedTime += Time.deltaTime;
            wasPickedUpLastFrame = false;
			Hover();
		}
        else {
            elapsedTime = 0;
            wasPickedUpLastFrame = true;
        }
	}

	// Hover while user is not picking it up
	void Hover() {
        var sinVal = elapsedTime * Mathf.PI * frequency;
        var deltaPos = new Vector3(Mathf.Sin(sinVal.x), Mathf.Sin(sinVal.y), Mathf.Sin(sinVal.z));
        positionOffset = Vector3.Scale(deltaPos, amplitude);
        // Debug.Log(positionOffset.x.ToString() + " " + positionOffset.y.ToString() + " " + positionOffset.z.ToString());
        transform.localPosition = oriPosition + positionOffset;
	}

    // Glows while in position, but not locked in
    public void Activate()
    {
        renderer.material = activatedMat;
        StartCoroutine("ActivateLight");
    }

    // Deactivate when locked in (feet are placed correctly)
    public void Deactivate()
    {
        renderer.material = deactivatedMat;
        StopCoroutine("ActivateLight");
    }

    IEnumerator ActivateLight()
    { 
        while (true)
        {
            glow.intensity = Mathf.PingPong(Time.time *0.5f, 1);  //0.5f * Time.deltaTime;
            yield return null;
        }
        
    }

    private void OnDisable()
    {
        StopCoroutine("ActivateLight");
    }
}
