using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Act1Controller : MonoBehaviour {
	public static Act1Controller Singleton;
	[SerializeField] Comet comet;

	// Audio components and clips
	[SerializeField] AudioSource source;

	/* 
		1: battery
			Guest acquires tool the allows them to place battery. 
		2: debris
			Debris fly toward guest, who must dodge them
		3: laser
			Guest must cut rope.
		4: end
	*/
	public int GameState = 1;

	/*
		00: not showing
		10: appears, game state 1
		20: start point of stage 2
		30: start point of state 3
	 */
	public int CometState = 0;

	private void Awake() {
		// Assurs that 
		Singleton = this;
	}

	// Use this for initialization
	void Start () {
		GameState = 1;
		// TODO! StartCoroutine(WaitBeforePlayingSound(6, problemClip));
	}
	IEnumerator realWaitBeforePlayingSound(int seconds, AudioClip soundClip) {
		yield return new WaitForSeconds(seconds);
		source.PlayOneShot(soundClip);
	}
	void WaitBeforePlayingSound(int seconds, AudioClip soundClip) {
		StartCoroutine(realWaitBeforePlayingSound(seconds, soundClip));
	}
	
	// Update is called once per frame
	void Update () {
        if (GameState == 1) {
            State1();
        } else if (GameState == 2) {
            State2();
        } else if (GameState == 3) {
            State3();
        } else if (GameState == 4) {
            State4();
		} else {
			if (!endPlayed) PlayFailureScene();
		}

        if (endPlayed)
        {
            if (director.time >= director.duration)
            {
                VRFade.FadeToBlack();
            }
        }

        if (Input.GetKeyDown("q"))
        {
            PlayFailureScene();
        }
    }


	/* In State 1, the guest acquires the tools in some way. Once they have the tools,
	 * they must place all of the batteries in the right place. Once all batteries have
	  * been received, the state increments. 
	 */
	 // Battery receivers
	[SerializeField] BatteryReceiver receiver1;
	[SerializeField] BatteryReceiver receiver2;
	[SerializeField] AudioClip moveFastComet;
	bool soundPlayed = false;
	bool cometHasAppeared = false;
	void State1() {
		if (!cometHasAppeared) {
			comet.CometAppear();
			cometHasAppeared = true;
		}
		if ((receiver1.batteryReceived || receiver2.batteryReceived) && !soundPlayed) {
			// You have to move fast; there's a comet heading toward us
			WaitBeforePlayingSound(2, moveFastComet);
			soundPlayed = true;
		}
		if (receiver1.batteryReceived && receiver2.batteryReceived) {
			GameState++;
		}
	}


    public EscapeMovement escapePod;

	/* In State 2, Character A warns the guest about debris flying toward them (sound).
	 * The guest must avoid the debris. Once debris have passed, the state increments.
	 */
	void State2() { // Skip for now
		/* Voice: Incoming debris! Duck!
		 */

		 /* Activate debris
		  * Hopefully guest will duck
		  */

		// Failure - screen black

		// Increment game state
		GameState++;
		CometState++;
        escapePod.LeaveShip();
		StartCoroutine(IntoStage3());
		comet.ActivateStage2();
	}
	
	[SerializeField] Transform controllerModel;
	[SerializeField] Transform laserModel;
	[SerializeField] Transform laserBox;
	[SerializeField] GameObject glassBox;
	[SerializeField] GameObject rope;

	/* In State 3, Character A informs guest that there is no time for them to return to the mothership before
     * the comet hits, and that she must be released from her tether to the ship or she will die.
	 * The guest must obtain a laser, and use it to cut the rope. Once they have done this successfully,
	 * the state increments.
	 */
	IEnumerator IntoStage3() {
		yield return new WaitForSeconds(3);
		StartCoroutine(RaiseLaserBox());
		WaitBeforePlayingSound(0, burnCordClip);
		yield return null;
	}
	IEnumerator RaiseLaserBox() {
		Vector3 originalPos = laserBox.localPosition;
		float nowTime = 0f, length = 2f, distance = 0.28f;
		while (nowTime < length) {
			nowTime += Time.deltaTime;
			float t = Mathf.Clamp01(nowTime / length);
			laserBox.localPosition = originalPos + t * distance * Vector3.left;
			yield return null;
		}
		glassBox.GetComponent<GlassBreak>().enabled = true;
	}
	bool fifteenSecondsUp = false;


	public static bool pickedUpLaser;
	/* "The comet is gonna hit the mothership. We have no time. My escape pod release is
	 * broken. You have to help me. Use the laser to burn the cord.
	 */
	[SerializeField] AudioClip burnCordClip;
	// I'm still stuck, hurry
	[SerializeField] AudioClip stillStuckClip;
	// Well, looks like you're stuck with me a little while longer
	[SerializeField] AudioClip stuckLonger;
	// Thanks for saving me
	[SerializeField] AudioClip savingMe;
    
	[SerializeField] AudioClip kickGlass;
    bool kickGlassPlayed = false;
	float state3elasped = 0;
	void State3() {
		state3elasped += Time.deltaTime;

		if (state3elasped > 15) {
            if (!kickGlassPlayed) WaitBeforePlayingSound(0, kickGlass);
            state3elasped = 0;
		}
		if (pickedUpLaser) {
			GameState++;
		}
		
    }
	float state4elasped = 0;
	[SerializeField] float stillStuckPlayAt = 15;
	bool stillStuckPlayed = false;
    float gameOverTimer = 0;
    bool endPlayed = false;
	[SerializeField] Debris2 debrisControl;
	float debrisTimer = 0;
	[SerializeField] float debrisInterval = 6;
	[SerializeField] AudioClip debrisClip;
	[SerializeField] AudioClip debrisSoundClip;
	bool debrisShot = false;
    void State4() {
		if (state4elasped > stillStuckPlayAt && !stillStuckPlayed) {
			WaitBeforePlayingSound(0, stillStuckClip);
			stillStuckPlayed = true;
		}
		if ((debrisTimer > debrisInterval) && !debrisShot) {
			debrisControl.ShotOne();
			debrisTimer = 0;
			WaitBeforePlayingSound(2, debrisClip);
			// Incoming debris! Move!
			WaitBeforePlayingSound(1, debrisSoundClip);
			debrisShot = true;
		}
		if (rope.GetComponent<RopeController>().isBroken) {
            if (!endPlayed) PlaySuccessScene();
            return;
		}
        gameOverTimer += Time.deltaTime;
		state4elasped += Time.deltaTime;
		debrisTimer += Time.deltaTime;
        if (gameOverTimer > 40) {
            if (!endPlayed) PlayFailureScene();
        }
	}

	[SerializeField] AudioClip explosionClip;
	[SerializeField] AudioClip aaahClip;
    public PlayableDirector director;
    void PlaySuccessScene() {
        director.Play();
        //sounds
		source.PlayOneShot(explosionClip);
		source.PlayOneShot(stuckLonger);
		source.PlayOneShot(savingMe);
        endPlayed = true;
	}

	void PlayFailureScene() {
        director.Play();
        //sounds
		source.PlayOneShot(aaahClip);
		source.PlayOneShot(explosionClip);
        endPlayed = true;
	}

}
