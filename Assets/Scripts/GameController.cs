using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameController : MonoBehaviour
{
    public static GameController Singleton;
    [SerializeField] Comet comet;

    // VOICE AUDIO ONLY
    [SerializeField] AudioSource characterA;

    [SerializeField] AudioClip burnTheCord; //comet gonna hit, no time, release broken. burn the cord
    [SerializeField] AudioClip kickTheGlass;
    [SerializeField] AudioClip incomingDebris;
    [SerializeField] AudioClip niceMove;
    [SerializeField] AudioClip imStillStuck;


    //ending scene voices
    [SerializeField] AudioClip stuckWithMe;
    [SerializeField] AudioClip thanksYouSavedMe;
    [SerializeField] AudioClip aaahClip;

    //Effects
    [SerializeField] AudioClip escapePodLeaving;



    //Explosion Audio Source
    [SerializeField] AudioSource explosionAudioSource;
    [SerializeField] AudioClip explosionSound;

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

    private void Awake()
    {
        // Assurs that 
        Singleton = this;
    }

    // Use this for initialization
    void Start()
    {
        GameState = 1;
        // TODO! StartCoroutine(WaitBeforePlayingSound(6, problemClip));
    }

    public BatteryReceiver breceiver1;
    public BatteryReceiver breceiver2;
    public Battery battery1;
    public Battery battery2;
    public GameObject sphere;
    bool blackOutCalled = false;
    // Update is called once per frame
    void Update()
    {
        if (GameState == 1)
        {
            State1();
        }
        else if (GameState == 2)
        {
            State2();
        }
        else if (GameState == 3)
        {
            State3();
        }
        else if (GameState == 4)
        {
            State4();
        }
        else
        {
            if (!endPlayed) PlayFailureScene();
        }

        if (endPlayed)
        {
            if (director.time >= director.duration)
            {
                if (!blackOutCalled)
                {
                    StartCoroutine(BlackOut.singleton.ChangeIntensity(1.0f, 1.0f));
                    blackOutCalled = true;
                }
            }
        }

        if (Input.GetKeyDown("q"))
        {
            PlayFailureScene();
        }
        if (Input.GetKeyDown("w"))
        {
            PlaySuccessScene();
        }
        if (Input.GetKeyDown("2"))
        {
            GameState = 2;
        }
        if (Input.GetKeyDown("3"))
        {
            GameState = 3;
        }
        if (Input.GetKeyDown("4"))
        {
            GameState = 4;
        }
        if (Input.GetKeyDown("e"))
        {
            escapePod.LeaveShip();
        }
        if (Input.GetKeyDown("r"))
        {
            Debug.Log("hello");
            battery1.transform.position += new Vector3(0, -2.692f, 0);
            breceiver1.GetComponent<InteractableItem>().isPickedUp = true;
        }
        if (Input.GetKeyDown("t"))
        {
            battery2.transform.position += new Vector3(0, -2.58f, 0);
            Debug.Log("hellooo");

            breceiver2.GetComponent<InteractableItem>().isPickedUp = true;
        }

    }


    /* In State 1, the guest acquires the tools in some way. Once they have the tools,
	 * they must place all of the batteries in the right place. Once all batteries have
	  * been received, the state increments. 
	 */
    // Battery receivers
    [SerializeField] BatteryReceiver receiver1;
    [SerializeField] BatteryReceiver receiver2;
    bool cometHasAppeared = false;
    void State1()
    {
        if (!cometHasAppeared)
        {
            comet.CometAppear();
            cometHasAppeared = true;
        }
        if (receiver1.batteryReceived || receiver2.batteryReceived)
        {
            // You have to move fast; there's a comet heading toward us
        }
        if (receiver1.batteryReceived && receiver2.batteryReceived)
        {
            GameState++;
        }
    }


    public EscapeMovement escapePod;
    bool burnTheCordPlayed = false;
    bool escapePodLeavingPlayed = false;

    /* In State 2, Character A warns the guest about debris flying toward them (sound).
	 * The guest must avoid the debris. Once debris have passed, the state increments.
	 */
    void State2()
    { // Skip for now
      /* Voice: Incoming debris! Duck!
       */

        /* Activate debris
         * Hopefully guest will duck
         */

        // Failure - screen black

        
        escapePod.LeaveShip();
        if (!escapePodLeavingPlayed)
        {
            characterA.PlayOneShot(escapePodLeaving);
            escapePodLeavingPlayed = true;
        }
        StartCoroutine(IntoStage3());
        if (!burnTheCordPlayed)
        {
            characterA.PlayOneShot(burnTheCord, 1.0f);
            burnTheCordPlayed = true;
            Debug.Log("burnthecord");
        }
        GameState++;
        CometState++;
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
    IEnumerator IntoStage3()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(RaiseLaserBox());
        yield return null;
    }
    IEnumerator RaiseLaserBox()
    {
        Vector3 originalPos = laserBox.localPosition;
        float nowTime = 0f, length = 2f, distance = 0.28f;
        while (nowTime < length)
        {
            nowTime += Time.deltaTime;
            float t = Mathf.Clamp01(nowTime / length);
            laserBox.localPosition = originalPos + t * distance * Vector3.left;
            yield return null;
        }
        GlassBreak gb = glassBox.GetComponent<GlassBreak>();
        gb.enabled = true; gb.PlayRaiseSound();
    }
    bool timeUp = false;
    IEnumerator CometTimer()
    {
        yield return new WaitForSeconds(30);
        timeUp = true;
    }
    bool fifteenSecondsUp = false;
    IEnumerator realCountDown(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        fifteenSecondsUp = true;
    }


    public static bool pickedUpLaser;
   
    bool kickTheGlassPlayed = false;
    float state3elasped = 0;
    void State3()
    {
        state3elasped += Time.deltaTime;

        if (state3elasped > 7)
        {
            if (!kickTheGlassPlayed)
            {
                characterA.PlayOneShot(kickTheGlass);
                kickTheGlassPlayed = true;
            }
            state3elasped = 0;
        }
        if (pickedUpLaser)
        {
            GameState++;
        }

    }
    float state4elasped = 0;
    float gameOverTimer = 0;
    bool endPlayed = false;
    [SerializeField] Debris2 debrisControl;
    float debrisTimer = 0;
    [SerializeField] float debrisInterval = 6;


    bool incomingDebrisPlayed = false;
    bool imStillStuckPlayed = false;

    void State4()
    {
        // Play "Incoming Debris!"
        if (!incomingDebrisPlayed)
        {
            characterA.PlayOneShot(incomingDebris);
            incomingDebrisPlayed = true;
        }

        //Debris coming in
        if (debrisTimer > debrisInterval)
        {
            debrisControl.ShotOne();
            debrisTimer = 0;
        }
        if (rope.GetComponent<RopeController>().isBroken)
        {
            if (!endPlayed) PlaySuccessScene();
            return;
        } else
        {
            if (state4elasped > 20)
            {
                if (!imStillStuckPlayed)
                characterA.PlayOneShot(imStillStuck);
                imStillStuckPlayed = true;
            }
        }
        gameOverTimer += Time.deltaTime;
        state4elasped += Time.deltaTime;
        debrisTimer += Time.deltaTime;
        if (gameOverTimer > 40)
        {
            if (!endPlayed) PlayFailureScene();
        }
    }

  
    public PlayableDirector director;
    bool thanksYouSavedMePlayed = false;
    bool stuckWithMePlayed = false;
    bool successExplosionPlayed = false;
    void PlaySuccessScene()
    {
        director.Play();
        escapePod.Saved();
        //sounds
        if (!successExplosionPlayed)
        {
            explosionAudioSource.PlayOneShot(explosionSound);
            successExplosionPlayed = true;
        }
        if (!thanksYouSavedMePlayed)
        {
            Debug.Log("thanks");
            StartCoroutine(StartSoundWithDelay(5, thanksYouSavedMe, characterA));

            thanksYouSavedMePlayed = true;
        } 
            if (!stuckWithMePlayed)
            {
                Debug.Log("stuck");
                StartCoroutine(StartSoundWithDelay(8, stuckWithMe, characterA));
                stuckWithMePlayed = true;
            }
        
        endPlayed = true;
    }

    bool aaahPlayed = false;
    bool failExplosionPlayed = false;

    void PlayFailureScene()
    {
        director.Play();
        escapePod.GoAwayForever();
        //sounds
        if (!failExplosionPlayed)
        {
            explosionAudioSource.PlayOneShot(explosionSound);
            failExplosionPlayed = true;
        }
        if (!aaahPlayed)
        {
            characterA.PlayOneShot(aaahClip);
            aaahPlayed = true;
        }
        endPlayed = true;
    }

    IEnumerator StartSoundWithDelay(int seconds, AudioClip soundClip, AudioSource src)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("playing coroutine");
        src.PlayOneShot(soundClip);


    }

}
