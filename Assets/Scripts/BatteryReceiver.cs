using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BatteryReceiver : MonoBehaviour {

	[SerializeField]
	GameObject battery;
	[SerializeField] GameObject foot1;
	[SerializeField] GameObject foot2;
	public bool batteryReceived = false;
    bool batteryReceivedClipPlayed = false;

	[SerializeField] AudioSource source;
	[SerializeField] AudioClip batteryReceivedClip;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (battery.GetComponent<InteractableItem>().isPickedUp) {
			float distance = Vector3.Distance(transform.position, battery.transform.position);
			//if (distance < .2 && foot1.GetComponent<FootCollider>().active && foot2.GetComponent<FootCollider>().active) {
			if (distance < .2) {
				battery.transform.position = transform.position;
				battery.transform.rotation = transform.rotation;
				List<ControllerInput> controllersHoldingBattery = battery.GetComponent<InteractableItem>().controllers;
				for (int i = 0; i < controllersHoldingBattery.Count; i++) {
					controllersHoldingBattery[i].Release();
				}
				Destroy(battery.GetComponent<InteractableItem>());
                if (!batteryReceivedClipPlayed)
                {
                    source.PlayOneShot(batteryReceivedClip);
                    batteryReceivedClipPlayed = true;
                }
				batteryReceived = true;
			}
		}
      
	}
}
