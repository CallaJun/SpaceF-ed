using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisController : MonoBehaviour {

    [SerializeField] public List<GameObject> debrisObjects;
    [SerializeField] public int numOfDebrisObjects;
    [SerializeField] public GameObject dummyObject;
    [SerializeField] public GameObject capsule;
    [SerializeField] public bool debug;
    [SerializeField] public float debrisSpeed;
    [SerializeField] public bool debrisInitDone = false;
    [SerializeField] int debrisCount = 0;
    [SerializeField] static GameObject[] debrisArr;
    
    // Use this for initialization
    void Start () {
        /*
         * TODO (varun) : move this somewhere
         */
        debrisArr = new GameObject[numOfDebrisObjects];
        StartCoroutine(createDebris());
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    int getRandomDebrisPrefabID()
    {
        float prefabCount = debrisObjects.Count;
        return (int)Random.Range(0f, prefabCount);

    }

    void incrDebrisCount ()
    {
        debrisCount++;
    }

    int getDebrisCount ()
    {
        return debrisCount;
    }

    /*
     * Spawn debris objects from debrisObjects prefabs. 
     * Space them out and direct them towards the capsule. 
     */
    IEnumerator createDebris()
    {
        int prefabCount = debrisObjects.Count, i;
        /* Start with the position of the dummy object and space out the rest */
        Vector3 position = dummyObject.transform.position;

        for (i = 0; i < numOfDebrisObjects; i++)
        {

            var randomID = getRandomDebrisPrefabID();
            position.x += Random.Range(-1f, 1f) * Random.Range(5f, 10f);
            position.z += Random.Range(3f, 8f);
            GameObject newDebrisObject = Instantiate(debrisObjects[randomID],
                                                     position, Quaternion.identity);
            newDebrisObject.name = "DebrisObj" + getDebrisCount().ToString();
            //newDebrisObject.tag = "DebrisObj" + getDebrisCount().ToString();
            debrisArr[i] = newDebrisObject;
            incrDebrisCount();
            newDebrisObject.name = "DebrisObj" + getDebrisCount();

            Debris deb = newDebrisObject.GetComponent<Debris>();
          
            deb.startPos = position;
            deb.startTime = Time.time;
            deb.journeyLength = (float)Vector3.Distance(deb.startPos,
                                               capsule.transform.position);
            if (!debrisInitDone) {
                StartCoroutine(moveDebris());
                debrisInitDone = true;
            }


            yield return new WaitForSeconds(1);
        }
        StartCoroutine(moveDebris());


    }

    IEnumerator moveDebris ()
    {
        int i;
        float step = debrisSpeed * Time.deltaTime;
        Debug.Log("<VLOG> step = " + step.ToString());


        while (true)
        {
            for (i = 0; i < getDebrisCount(); i++)
            {
                Debris deb = debrisArr[i].GetComponent<Debris>();

                float distCovered = (Time.time - deb.startTime) * debrisSpeed;
                float fracJourney = distCovered / deb.journeyLength;
          
                debrisArr[i].transform.position = Vector3.Lerp(dummyObject.transform.position, capsule.transform.position, fracJourney);
                
                yield return null;
            }
        }
       
    }

        //Instantiate

    
}
