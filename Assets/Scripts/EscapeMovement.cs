using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMovement : MonoBehaviour {

    public Transform startPos;
    public Transform outPos;
    public Transform stuckPos;
    public Transform diePos;
    public Transform capsulePos;

    public GameObject rope;

    Vector3 readyToGoPos;

	// Use this for initialization
	void Start () {

        transform.position = startPos.position;
        rope.GetComponent<LineRenderer>().enabled = false;
	}

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown("z")) LeaveShip();
	}

    public void LeaveShip ()
    {
        rope.GetComponent<LineRenderer>().enabled = true;
        StartCoroutine("LeaveMothership");
    }

    void MoveAwayFromShip ()
    {
        StartCoroutine("MoveAway");
    }

    public void GoAwayForever()
    {
        readyToGoPos = transform.position;
        StartCoroutine("Disappear");
    }

    public void Saved()
    {
        readyToGoPos = transform.position;
        StartCoroutine("Drift");
    }

    IEnumerator LeaveMothership()
    {
        var startpos = transform.position;
        var endpos = outPos.position;
        float length = 1f, nowTime = 0f;
        while (nowTime < length) {
            nowTime += Time.deltaTime;
            float t = Mathf.Clamp01(nowTime / length);
            var pos = Vector3.Lerp(startpos, endpos, t);
            transform.position = pos;
            yield return null;
        }
        StartCoroutine("MoveAway");
        // while (Vector3.Distance(transform.position, outPos.position) >= 0.1f)
        // {
        //     transform.position += (outPos.position - startPos.position) * Time.deltaTime;
        //     if (Vector3.Distance(transform.position, outPos.position) <= 0.1f)
        //     {
        //         StartCoroutine("MoveAway"); StopCoroutine("LeaveMothership");
        //     }
        //     yield return null;
        // } 
    }
    IEnumerator MoveAway()
    {
        var startpos = transform.position;
        var endpos = stuckPos.position;
        float length = 3f, nowTime = 0f;
        while (nowTime < length) {
            nowTime += Time.deltaTime;
            float t = Mathf.Clamp01(nowTime / length);
            var pos = Vector3.Lerp(startpos, endpos, t);
            transform.position = pos;
            yield return null;
        }
        gameObject.AddComponent<RandomMove>();

        // while (Vector3.Distance(transform.position, stuckPos.position) >= 0.1f)
        // {
        //     transform.position += (stuckPos.position - outPos.position) * Time.deltaTime * 0.3f;
        //     if (Vector3.Distance(transform.position, stuckPos.position) <= 0.1f)
        //     {
        //         StopCoroutine("MoveAway");
        //         gameObject.AddComponent<RandomMove>();
        //     }
        //     yield return null;
        // }
    }

    IEnumerator Disappear()
    {
        while (Vector3.Distance(transform.position, diePos.position) >= 0.1f)
        {
            transform.position += (readyToGoPos - diePos.position) * Time.deltaTime * 0.3f;
            if (Vector3.Distance(transform.position, diePos.position) <= 0.1f)
            {
                StopCoroutine("Disappear");
            }
            yield return null;
        }
    }
    IEnumerator Drift()
    {
        while (Vector3.Distance(transform.position, capsulePos.position) >= 0.1f)
        {
            transform.position += (readyToGoPos - capsulePos.position) * Time.deltaTime * 0.3f;
            if (Vector3.Distance(transform.position, capsulePos.position) <= 0.1f)
            {
                StopCoroutine("Drift");
            }
            yield return null;
        }
    }
}
