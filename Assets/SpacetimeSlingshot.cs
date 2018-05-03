using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class SpacetimeSlingshot : MonoBehaviour
{

    PlayerReferences pReference;
    LineRenderer slingshotLineRenderer;
    public int slingshotCounter = 0;
    public int maxConcurrentSlingshots = 3;
    public bool cantSlingshot = false;
    bool stillHeld = false;

    public List<Vector2> potentialTearLocations = new List<Vector2>();

    public GameObject shieldColliderGO;
    public GameObject powerThrustParticles;
    public List<ParticleSystem> powerThrustParticleList = new List<ParticleSystem>();

    float powerThrustThreshold;
    float powerThrustMax;

    LaunchSoul soulLauncher;

    //TODO: Change this later
    public static float slowdownTime = 0.25f;

    public static event Action PrimingToBashEnemy;
    public static event Action<Vector2> ReleasedWithBashActive;
    void PrimingToBashEnemyWrapper()
    {
        if (PrimingToBashEnemy != null)
        {
            PrimingToBashEnemy();
        }
    }

    void ReleasedWithBashActiveWrapper(Vector2 velocityDirection){
        if(ReleasedWithBashActive != null){
            ReleasedWithBashActive(velocityDirection);
        }
    }

    public static event Action NoLongerPrimingToBashEnemy;
    void NoLongerPrimingToBashEnemyWrapper()
    {
        if (NoLongerPrimingToBashEnemy != null)
        {
            NoLongerPrimingToBashEnemy();
        }
    }

    public static event Action Launching;

    void LaunchingWrapper()
    {
        if (Launching != null)
        {
            Launching();
        }
    }

    private void Awake()
    {
        pReference = GetComponent<PlayerReferences>();
        slingshotLineRenderer = GameObject.Find("SlingshotLineRendererGO").GetComponent<LineRenderer>();
        slingshotLineRenderer.enabled = false;
        if (powerThrustParticles != null)
        {
            powerThrustParticleList = GetComponentsInChildren<ParticleSystem>().ToList();
        }
        originalStartColor = slingshotLineRenderer.startColor;
        originalEndColor = slingshotLineRenderer.endColor;
        powerThrustMax = 15.0f;
        powerThrustThreshold = 10.0f;
    }


    bool launching;
    public bool priming = false;
    float minimumHoldDuration = 0.05f;
    Vector2 mouseStartPosition;
    float maxPullDistance;
    public float elasticity;

    Color originalStartColor;
    Color originalEndColor;

    float holdStartTime;

    public static event Action<List<Vector2>> RipSpaceTime;

    

    void SetLineRendererColor(Color a, Color b, float valueBetween)
    {
        slingshotLineRenderer.startColor = Color.Lerp(originalStartColor, a, valueBetween);
        slingshotLineRenderer.endColor = Color.Lerp(originalEndColor, b, valueBetween);
    }
    void SpaceTimeRip(List<Vector2> ourList)
    {

        if (RipSpaceTime != null)
        {
            //Debug.Log("SpaceTime being ripped");

            RipSpaceTime(ourList);
        }
    }

    bool stretchedPastMax;

    bool stretchedPastThreshold;

    public IEnumerator PrimeSlingshot()
    {

        //Debug.Log("Priming!");
        priming = true;
        mouseStartPosition = Input.mousePosition;
        float distance = 0;
        Vector2 direction = new Vector2(0, 0);
        Vector2 mousePos = new Vector2(0, 0);
        Vector2 mousePositionWorld = new Vector2(0, 0);
        float scaledValue;

        slingshotLineRenderer.enabled = true;
        //TODO: Make a top speed
        FreezeTime.SlowdownTime(0.25f);
        while (true)
        {
            mousePos = Input.mousePosition;
            mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

            distance = Vector2.Distance(gameObject.transform.position, mousePositionWorld);
            if (distance >= powerThrustThreshold)
            {
                if (!stretchedPastThreshold)
                {
                    stretchedPastThreshold = true;
                    PrimingToBashEnemyWrapper();
                }

            }
            else if (distance < powerThrustThreshold)
            {
                //the mouse has moved within range
                if (stretchedPastThreshold)
                {
                    //we've stretched back into the normal threshold
                    stretchedPastThreshold = false;
                    NoLongerPrimingToBashEnemyWrapper();
                }
            }
            //Debug.Log("Distance: " + distance);
            scaledValue = distance / 20.0f;
            if (Input.GetMouseButtonUp(0))
            {
                break;
            }
            if (Input.GetMouseButtonDown(1))
            {
                //right click to cancel
                stillHeld = true;
                slingshotLineRenderer.enabled = false;
                NoLongerPrimingToBashEnemyWrapper();
                FreezeTime.StartTimeAgain();
                priming = false;
                yield break;
            }
            slingshotLineRenderer.SetPosition(0, transform.position);
            slingshotLineRenderer.SetPosition(1, MousePositionHandler.LimitPosition(transform.position, 20.0f, slingshotLineRenderer.GetPosition(1)));
            // slingshotLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
            SetLineRendererColor(Color.red, Color.red, scaledValue);
            yield return null;
        }
        FreezeTime.StartTimeAgain();
        slingshotLineRenderer.enabled = false;

        slingshotCounter++;

        //TODO: Make some maximum speed, and a cap for the collider to be activated, and add some sort of particle effect
        mousePos = Input.mousePosition;
        mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        distance = Vector2.Distance(gameObject.transform.position, mousePositionWorld);
        direction = (Vector2)((Vector2)transform.position - mousePositionWorld);

        if(stretchedPastThreshold){
        //if we've pulled the slingshot back far enough, we want the collider to be activated through this action, else, no.
            //if we stretched past our threshold, 
            ReleasedWithBashActiveWrapper(direction);

            //reset stretch back threshold
            stretchedPastThreshold = false;
        }

        float velocity = distance * Mathf.Sqrt(elasticity / pReference.rb.mass);
        pReference.rb.velocity = direction.normalized * velocity;

        priming = false;
        launching = true;
        //if we've pulled the slingshot back far enough, we want the collider to be activated through this action, else, no.
        if (distance > powerThrustThreshold)
        {
            LaunchingWrapper();
        }
        //if (/*we pulled back really far*/)
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
        while (slingshotCounter > 0)
        {
            slingshotCounter--;
            yield return new WaitForSeconds(1);
        }

        cantSlingshot = false;
    }



    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //Issues with the "still held" boolean
        if (slingshotCounter == maxConcurrentSlingshots && !cantSlingshot)
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
            if (!priming && (Time.time - holdStartTime >= minimumHoldDuration) && !pReference.launchSoul.priming && pReference.locationHandler.currentMovement != pReference.locationHandler.planetMovement && pReference.locationHandler.currentPlanet == null)
            {

                //slingshotLineRenderer.enabled = true;
                //slingshotLineRenderer.SetPosition(0, transform.position);
                //slingshotLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
                StartCoroutine(PrimeSlingshot());
            }
        }
        if (launching && pReference.locationHandler.inOrbit || launching && pReference.locationHandler.inVoid /*or if hooked */)
        {
            CancelLaunch();
        }

        if (launching && Input.GetMouseButton(0) && !cantSlingshot && !stillHeld && !pReference.launchSoul.priming && pReference.locationHandler.currentMovement != pReference.locationHandler.planetMovement && pReference.locationHandler.currentPlanet == null)
        {
            CancelLaunch();
            StartCoroutine(PrimeSlingshot());

        }
    }
}
