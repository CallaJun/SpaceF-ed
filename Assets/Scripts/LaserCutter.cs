using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// TODO: Disable ControllerInput before laser use, renable after, 
// Enable laserActivated during use of this
public class LaserCutter : MonoBehaviour {

	// TODO: Add laser pointer, empty game object, configured like so:
	// https://www.youtube.com/watch?v=uo3SvK1cA54
	public bool laserActivated = false;
	[SerializeField] GameObject laserPoint;
	protected SteamVR_TrackedObject trackedObj;
	[SerializeField] RopeController ropePoints;
	// Minimum distance between laser and rope point for cut
	[SerializeField] float threshold = 0.3f;
	LineRenderer laserRenderer;
    public AudioSource source;
    

	public SteamVR_Controller.Device device {
		get {
			return SteamVR_Controller.Input((int) trackedObj.index);
		}
	}
	void Awake() {
		// Instantiate lists
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Use this for initialization
	void Start () {
		laserRenderer = laserPoint.GetComponent<LineRenderer>();
		laserRenderer.enabled = false;
        source.Play();
        source.Pause();
	}
	
	// Update is called once per frame
	void Update () {
		if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
			laserActivated = false;
			laserRenderer.enabled = false;
            source.Pause();
		}
		if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
			laserActivated = true;
			laserRenderer.enabled = true;
            source.UnPause();
		}
		if (laserActivated) {
			device.TriggerHapticPulse(150);
		}
		Vector3 startPoint = transform.position;
		Vector3 forward = transform.forward;
		float len = 40f;
		if (laserActivated && ropePoints) {
			int idx;
			float length;
			GetMinimumDistance(out idx, out length);
			if (idx != -1) {
				ropePoints.ShotAt(idx, laserPoint.transform.position + transform.forward * length);
				Vector3 laserEnd = new Vector3(0, length, 0);
				len = length;
			}
		}
		laserRenderer.SetPosition(0, startPoint);
		laserRenderer.SetPosition(1, startPoint + forward * len);
	}

	/* Returns the point index of the item in ropePoints.line whose distance to the laser cutter
	 * is smallest. 
	 */
	void GetMinimumDistance(out int pointIndex, out float length) {
		float smallestValue = float.MaxValue;
		pointIndex = -1;
		length = 0;
		
		Ray ray = new Ray(laserPoint.transform.position, transform.forward);
		Debug.DrawLine(ray.origin, ray.origin + ray.direction);
		for (int i = 0; i < ropePoints.line.Length; i++) {
			float distanceBetweenPoints = Vector3.Cross(ray.direction, ropePoints.line[i] - ray.origin).magnitude;
			float len = Vector3.Dot(ray.direction, ropePoints.line[i] - ray.origin);
			if (len > 0) {
				if (smallestValue > distanceBetweenPoints) {
					smallestValue = distanceBetweenPoints;
					pointIndex = i;
					length = len;
				}
			}
		}
		// Debug.Log(smallestValue);
		if (smallestValue > threshold) {
			pointIndex = -1;
		}
	}

	// void OnTriggerStay(Collider collider) {
	// 	if (laserActivated) {
	// 		laserPoint.GetComponent<Renderer>().enabled = true;
	// 	}
	// }
}
