using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerInput : MonoBehaviour {

	protected List<InteractableItem> heldObjects;
	// Controller references;
	protected SteamVR_TrackedObject trackedObj;
	[SerializeField] GameObject laserpointer, controller;
    public GlassBreak box;

	public SteamVR_Controller.Device device {
		get {
			return SteamVR_Controller.Input((int) trackedObj.index);
		}
	}

	void Awake() {
		// Instantiate lists
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		heldObjects = new List<InteractableItem>();
	}

	void OnTriggerStay(Collider collider) {
		// If object is an interactable item
		InteractableItem interactable = collider.GetComponent<InteractableItem>();
		if (interactable != null) {
			if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
				// Pick up object
				if (interactable.gameObject.name != "laserpointer") {
					interactable.Pickup(this);
					heldObjects.Add(interactable);
				}
				else {
                    if (box.glassBroken)
                    {
                        GameController.pickedUpLaser = true;
                        GetComponent<LaserCutter>().enabled = true;
                        controller.SetActive(false);
                        laserpointer.SetActive(true);
                        Destroy(interactable.gameObject);
                        this.enabled = false;
                    }
				}
				
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	public void Release() {
		for (int i = 0; i < heldObjects.Count; i++) {
			heldObjects[i].Release(this);
		}
		heldObjects = new List<InteractableItem>();
	}

	void Update() {
		var animator = controller.GetComponent<Animator>();
		if (animator) {
			if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
				animator.SetBool("isHolding", true);
			}
			if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
				animator.SetBool("isHolding", false);
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		
		// Letting go of objects
		if (heldObjects.Count > 0) {
			// If trigger is released
			if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
				Release();
			}
		}
		
	}
}
