using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MothershipController : MonoBehaviour {

    public GameObject breakableShip;
    public GameObject mothership;
    public ParticleSystem[] explosions;
    public Light flash;
    float startTime;
    int multiplier = 6;


    // Use this for initialization
    void Start () {
        flash.gameObject.SetActive(false);
		foreach (ParticleSystem e  in explosions)
        {
            e.Stop();
            e.Clear();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Explode()
    {
        foreach (ParticleSystem e in explosions)
        {
            e.Play();
        }
        flash.gameObject.SetActive(true);
        startTime = Time.time;
        StartCoroutine("Flash");
        Destroy(mothership);
        breakableShip.SetActive(true);

    }

    private void OnTriggerEnter(Collider other)
    {
        // Explode();
    }

    IEnumerator Flash ()
    {
        while (flash.intensity > 0.3)
        {
            flash.intensity += Time.deltaTime  * multiplier;
            if (flash.intensity >= 12)
            {
                multiplier = -1;
            }
            yield return null;
        }
    }
}
