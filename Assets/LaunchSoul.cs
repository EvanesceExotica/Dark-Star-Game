using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LaunchSoul : MonoBehaviour
{
    bool poweredUp;

    public static event Action SoulToBeLaunched;
    public static event Action SoulPriming;
    public static event Action DonePriming;

    public void PrimingSoul()
    {
        if (SoulPriming != null)
        {
            SoulPriming();
        }
    }
    public void DonePrimingSoul()
    {
        if (DonePriming != null)
        {
            DonePriming();
        }
    }
    public static event Action SoulNotLaunching;

    public void LaunchingSoul()
    {
        if (SoulToBeLaunched != null)
        {
            SoulToBeLaunched();
        }
    }

    public void NotLaunchingSoul()
    {
        if (SoulNotLaunching != null)
        {
            SoulNotLaunching();
        }
    }
    PlayerReferences playerReferences;
    PlayerSoulHandler ourSoulHandler;
    GameStateHandler gameStateHandler;

    SpacetimeSlingshot ourSlightshot;
    void Awake()
    {
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        playerReferences = GetComponent<PlayerReferences>();
        ourSlightshot = playerReferences.slingshot;
        ourSoulHandler = playerReferences.playerSoulHandler;
        PlayerSoulHandler.PoweredUp += this.SetPoweredUp;
        PlayerSoulHandler.PowerUpTimedOut += this.SetNOTPoweredUp;
    }

    void Start()
    {
        elasticity = 10.0f;
    }
    void SetPoweredUp()
    {
        Debug.Log("We can launch soul since we're powered up");

        poweredUp = true;
    }

    void SetNOTPoweredUp()
    {

        poweredUp = false;
    }
    bool launching;
    public bool priming = false;
    float minimumHoldDuration = 1.0f;
    Vector2 mouseStartPosition;
    float maxPullDistance;
    public float elasticity;
    LineRenderer slingshotLineRenderer;
    bool stillHeld = false;
    float holdStartTime;
    Rigidbody2D rb;

    public void BeginSlingshotOfOSoul(GameObject whichSoul)
    {
    }
    public IEnumerator PrimeSlingshot(GameObject whichSoul)
    {
        SoulBehavior ourSoulsBehaviour = whichSoul.GetComponent<SoulBehavior>();
        PrimingSoul();
        //Debug.Log("Priming!");
        priming = true;
        ourSoulsBehaviour.beingPrimed = true;
        mouseStartPosition = Input.mousePosition;
        float distance = 0;
        Vector2 direction = new Vector2(0, 0);

        slingshotLineRenderer = whichSoul.GetComponentInChildren<LineRenderer>();
        slingshotLineRenderer.enabled = true;
        FreezeTime.SlowdownTime(0.25f);
        while (true)
        {

            if (Input.GetMouseButtonUp(1))
            {
                break;
            }
            if (Input.GetMouseButtonDown(0))
            {
                //right click to cancel
                stillHeld = true;
                slingshotLineRenderer.enabled = false;
                yield break;
            }
            slingshotLineRenderer.SetPosition(0, whichSoul.transform.position);
            slingshotLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
            yield return null;
        }
        FreezeTime.StartTimeAgain();
        slingshotLineRenderer.enabled = false;

        Vector2 mousePos = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        Rigidbody2D soulRigidbody = whichSoul.GetComponent<Rigidbody2D>();
        distance = Vector2.Distance(whichSoul.transform.position, mousePositionWorld);
        direction = (Vector2)((Vector2)whichSoul.transform.position - mousePositionWorld);


        float velocity = distance * Mathf.Sqrt(elasticity / soulRigidbody.mass);
        soulRigidbody.velocity = direction.normalized * velocity;

        //Debug.Log(pReference.rb.velocity);
        priming = false;
        ourSoulsBehaviour.beingPrimed = false;
        ourSoulsBehaviour.launching = true;
        DonePrimingSoul();
        launching = true;
        LaunchingSoul();
        StartCoroutine(CountdownFromLaunch(ourSoulsBehaviour));
        //   StartCoroutine(PlotPath());
    }

    IEnumerator CountdownFromLaunch(SoulBehavior soulBehavior)
    {

        yield return new WaitForSeconds(5.0f);
        launching = false;
        soulBehavior.launching = false;
        NotLaunchingSoul();
    }


    void Update()
    {
        if (Input.GetMouseButton(1) && poweredUp && !playerReferences.slingshot.priming)
        {
            holdStartTime = Time.time;
            StartCoroutine(PrimeSlingshot(ourSoulHandler.soulsAttachedToPlayer[0]));

        }
    }
    // Use this for initialization

}
