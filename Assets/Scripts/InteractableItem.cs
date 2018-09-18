using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour {

	protected Rigidbody rigidBody;
	protected bool originalKenimaticState;
	protected Transform originalParent;
	bool hasRigibody;
	public bool isPickedUp = false;

	// List of controllers that are currently holding this interactable item
	public List<ControllerInput> controllers;

	void Awake() {
		rigidBody = GetComponent<Rigidbody>();
		if (!rigidBody) {
			hasRigibody = false;
		}
		else {
			hasRigibody = true;
			originalKenimaticState = rigidBody.isKinematic;
		}
		// Capture object's original parent and kenimatic state
		originalParent = transform.parent;
		
	}
	public void Pickup(ControllerInput controller) {
		// Make object kinematic, or not affected by physics, but still
		// able to affect other objects affected by physics
		if (hasRigibody)
			rigidBody.isKinematic = true;
		// Set the object's parent to be the controller
		transform.SetParent(controller.gameObject.transform);
		controllers.Add(controller);
		isPickedUp = true;
	}

	public void Release(ControllerInput controller) {
		if (hasRigibody) {
			// Make sure hand is still the object's parent (in case it was transferred to other hand)
			if (transform.parent == controller.gameObject.transform) {
				// Return previous kinematic state
				rigidBody.isKinematic = originalKenimaticState;
				// Set object's parent to its original parent
				if (originalParent != controller.gameObject.transform) {
					// Ensure original parent recorded wasn't somehow the controller (failsafe)
					transform.SetParent(originalParent);
				} else {
					transform.SetParent(null);
				}
			}
			// Throw object
			rigidBody.velocity = controller.device.velocity;
			rigidBody.angularVelocity = controller.device.angularVelocity;
		} else {
			// Make sure hand is still the object's parent (in case it was transferred to other hand)
			if (transform.parent == controller.gameObject.transform) {
				// Set object's parent to its original parent
				if (originalParent != controller.gameObject.transform) {
					// Ensure original parent recorded wasn't somehow the controller (failsafe)
					transform.SetParent(originalParent);
				} else {
					transform.SetParent(null);
				}
			}
		}
		controllers.Remove(controller);
		isPickedUp = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
