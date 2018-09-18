using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour {
    float rotationSpeed = 1000.0f;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        Quaternion newRot = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.right);
        transform.rotation *= newRot;
    }
}
