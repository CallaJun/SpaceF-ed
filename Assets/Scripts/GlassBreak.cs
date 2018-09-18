using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour {

    public Transform breakableBox;
    public float radius;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip boxRisingClip;
    [SerializeField] AudioClip glassBreakClip;

    bool raiseGlassBoxPlayed = false;
    public bool glassBroken = false;

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.collider.name);
        var t = Instantiate(breakableBox, transform.position, transform.rotation);
        t.transform.parent = transform.parent;
        // breakableBox.localScale = transform.localScale;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in colliders) {
            if (hit.GetComponent<Rigidbody>()) {
                hit.GetComponent<Rigidbody>().AddExplosionForce(10 * collision.relativeVelocity.magnitude,
                                                                        transform.position,
                                                                        radius,
                                                                        3);
                source.PlayOneShot(glassBreakClip);
                glassBroken = true;
            }
        }
        Destroy(gameObject);
    }

    public void PlayRaiseSound()
    {
        if (!raiseGlassBoxPlayed)
        {
            source.PlayOneShot(boxRisingClip);
            raiseGlassBoxPlayed = true;
        }
    }
}
