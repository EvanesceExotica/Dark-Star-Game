using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpacetimeSlingshot : MonoBehaviour {

    PlayerReferences pReference;
    LineRenderer slingshotLineRenderer;
    public int slingshotCounter = 0;
    public int maxConcurrentSlingshots = 3;
    public bool cantSlingshot = false;
    bool stillHeld = false;

    public List<Vector2> potentialTearLocations = new List<Vector2>();

    LaunchSoul soulLauncher;

    private void Awake()
    {
        pReference = GetComponent<PlayerReferences>();
        slingshotLineRenderer = GameObject.Find("SlingshotLineRendererGO").GetComponent<LineRenderer>();
        slingshotLineRenderer.enabled = false;
    }


    bool launching;
    public bool priming  = false;
    float minimumHoldDuration = 0.05f;
    Vector2  mouseStartPosition;
    float maxPullDistance;
    public float elasticity;


    float holdStartTime;

    public static event Action<List<Vector2>> RipSpaceTime;

    void SpaceTimeRip(List<Vector2> ourList)
    {

        if(RipSpaceTime != null)
        {
            //Debug.Log("SpaceTime being ripped");

            RipSpaceTime(ourList);
        }
    }

    public IEnumerator PrimeSlingshot()
    {
 
        //Debug.Log("Priming!");
        priming = true;
        mouseStartPosition = Input.mousePosition;
        float distance = 0;
        Vector2 direction = new Vector2(0, 0);


        slingshotLineRenderer.enabled = true;
        //TODO: Uncomment this pls
        FreezeTime.SlowdownTime(0.25f);
        while (true) 
        {
 
            if (Input.GetMouseButtonUp(0))
            {
                break;
            }
            if (Input.GetMouseButtonDown(1))
            {
                //right click to cancel
                stillHeld = true;
                slingshotLineRenderer.enabled = false;
                FreezeTime.StartTimeAgain();
                priming = false;
                yield break;
            }
            slingshotLineRenderer.SetPosition(0, transform.position);
            slingshotLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
            yield return null;
        }
        FreezeTime.StartTimeAgain();
        slingshotLineRenderer.enabled = false;

        slingshotCounter++;

        Vector2 mousePos = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        distance = Vector2.Distance(gameObject.transform.position, mousePositionWorld);
        direction = (Vector2)((Vector2)transform.position - mousePositionWorld);


        float velocity = distance * Mathf.Sqrt(elasticity / pReference.rb.mass);
        pReference.rb.velocity = direction.normalized * velocity;

        //Debug.Log(pReference.rb.velocity);
        priming = false;
        launching = true;
     //   StartCoroutine(PlotPath());
    }

    IEnumerator PlotPath()
    {
        while (launching)
        {
            //Debug.Log("Plotting path");
            potentialTearLocations.Add(transform.position);
            yield return new WaitForSeconds(1.0f);
        }
        SpaceTimeRip(potentialTearLocations);
    }

  
    void CancelLaunch()
    {
        launching = false;
        //add drag here instead
      //TODO:  pReference.rb.velocity = new Vector3(0, 0, 0);
    }

    IEnumerator Recharge()
    {
        while(slingshotCounter > 0)
        {
            slingshotCounter--;
            yield return new WaitForSeconds(1);
        }

        cantSlingshot = false;
    }

	// Use this for initialization
	void Start () {
      
		
	}
	
	// Update is called once per frame
	void Update () {
        //Issues with the "still held" boolean
        if(slingshotCounter == maxConcurrentSlingshots && !cantSlingshot)
        {
            //cantSlingshot = true;
        }
        if (Input.GetMouseButtonUp(0) && stillHeld)
        {
            stillHeld = false;

        }


        if (Input.GetMouseButtonDown(0) && !cantSlingshot && !stillHeld)
        {
            holdStartTime = Time.time;
        }
        if (Input.GetMouseButton(0) && !cantSlingshot && !stillHeld)
        {
            
            // //Debug.Log("Holding mouse button, started at: " + holdStartTime);
            var test = Time.time - holdStartTime;
           // //Debug.Log(test + "vs" + minimumHoldDuration);
            if(!priming && (Time.time - holdStartTime >= minimumHoldDuration) && !pReference.launchSoul.priming){

                //slingshotLineRenderer.enabled = true;
                //slingshotLineRenderer.SetPosition(0, transform.position);
                //slingshotLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
                StartCoroutine(PrimeSlingshot());
            }
        }
             if(launching && pReference.locationHandler.inOrbit || launching && pReference.locationHandler.inVoid /*or if hooked */)
        {
            CancelLaunch();
        }

        if(launching && Input.GetMouseButton(0) && !cantSlingshot && !stillHeld && !pReference.launchSoul.priming)
        {
            CancelLaunch();
            StartCoroutine(PrimeSlingshot());

        }
    }
}
