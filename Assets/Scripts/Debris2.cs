using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Debris2 : MonoBehaviour {

    [SerializeField] GameObject[] DebrisPrefabs;
    [SerializeField] Transform[] StartPoints;
    [SerializeField] Transform[] EndPoints;
    [SerializeField] float travelTime;
    public AudioClip debrisFlyingSound;
    public GameObject[] DebriArr;
    System.Random rnd;
    
	public float rotationSpeed = 1f;

    public void ShotOne() {
        GameObject template = DebrisPrefabs[rnd.Next(0, DebrisPrefabs.Length)];
        Transform start = StartPoints[rnd.Next(0, StartPoints.Length)];
        Transform end = EndPoints[rnd.Next(0, EndPoints.Length)];
        AudioSource source = end.gameObject.GetComponent<AudioSource>();
        Debug.Log(source.ToString());
        source.PlayOneShot(debrisFlyingSound);
        Debug.Log("Shot");
        var instance = Instantiate(template, start.position, Quaternion.identity, transform);
        instance.name = "debris";
        StartCoroutine(DebrisAnimate(instance, start, end));
    }

    IEnumerator DebrisAnimate(GameObject o, Transform start, Transform end) {
        float elapse = 0;
        var direction = start.position - end.position;
		direction.Normalize();
		var randomRotationAxis = Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, direction).normalized;
        while (elapse <= travelTime * 2) {
            elapse += Time.deltaTime;
            float t = elapse / travelTime;
            Debug.Log(t);
            if (GameController.Singleton.GameState > 4) {
                Destroy(o);
                yield break;
            }
            var pos = Vector3.LerpUnclamped(start.position, end.position, t);
            o.transform.position = pos;

            Quaternion newRot = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, randomRotationAxis);
			o.transform.rotation *= newRot;
            yield return null;
        }
        Destroy(o);
    }

    // Use this for initialization
    void Start () {
        rnd = new System.Random();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
