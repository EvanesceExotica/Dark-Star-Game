using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Com.LuisPedroFonseca.ProCamera2D;
public class LaunchSoul : MonoBehaviour
{
    //Variables
    #region
    ProCamera2D ourProCamera2D;

    public SoulBehavior currentSoulBehaviour;
    bool poweredUp;

    public static event Action<GameObject> SoulToBeLaunched;
    public static event Action SoulPriming;
    public static event Action DonePriming;
    bool zoomed;
    #endregion

//Actions and Action methods
#region
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

    public void LaunchingSoul(GameObject launchedSoul)
    {
        if (SoulToBeLaunched != null)
        {
            SoulToBeLaunched(launchedSoul);
        }
    }

    public void NotLaunchingSoul()
    {
        if (SoulNotLaunching != null)
        {
            SoulNotLaunching();
        }
    }
    #endregion

//References to other scripts
#region
    PlayerReferences playerReferences;
    SoulHandler ourSoulHandler;
    GameStateHandler gameStateHandler;

    SpacetimeSlingshot ourSlightshot;
    #endregion 

#region //start and awake and other small funcions and variables
    void Awake()
    {
        ourProCamera2D = Camera.main.GetComponent<ProCamera2D>();
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        playerReferences = GetComponent<PlayerReferences>();
        ourSlightshot = playerReferences.slingshot;
        ourSoulHandler = playerReferences.playerSoulHandler;
        SoulHandler.Charged += this.SetPoweredUp;
        SoulHandler.ChargeTimedOut += this.SetNOTPoweredUp;
        ChoosePowerUp.powerupChosen += this.ResetTimeAndSetLaunchToFalse;
    }

    void Start()
    {
        elasticity = 10.0f;
    }
    void SetPoweredUp()
    {

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
    float holdStartTime;//
    Rigidbody2D rb;
#endregion
    public IEnumerator PrimeSlingshot(GameObject whichSoul)
    {

        currentSoulBehaviour = whichSoul.GetComponent<SoulBehavior>();
        PrimingSoul();
        if (!zoomed)
        {
          //  ZoomOnPlayer();
        }
        //Debug.Log("Priming!");
        priming = true;
        currentSoulBehaviour.beingPrimed = true;
        mouseStartPosition = Input.mousePosition;
        float distance = 0;
        Vector2 direction = new Vector2(0, 0);

        slingshotLineRenderer = whichSoul.GetComponentInChildren<LineRenderer>();
        slingshotLineRenderer.enabled = true;
        FreezeTime.SlowdownTime(0.10f);
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
        slingshotLineRenderer.enabled = false;

        Vector2 mousePos = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        Rigidbody2D soulRigidbody = whichSoul.GetComponent<Rigidbody2D>();
        distance = Vector2.Distance(whichSoul.transform.position, mousePositionWorld);
        direction = (Vector2)((Vector2)whichSoul.transform.position - mousePositionWorld);


        float velocity = distance * Mathf.Sqrt(elasticity / soulRigidbody.mass);
        velocity *= (10); //multiply to cancel out low timescale
        soulRigidbody.isKinematic = false;
        soulRigidbody.velocity = (direction.normalized * velocity);

        //Debug.Log(pReference.rb.velocity);
        priming = false;
        currentSoulBehaviour.beingPrimed = false;
        currentSoulBehaviour.launching = true;
        DonePrimingSoul();
        launching = true;
        LaunchingSoul(whichSoul);
        StartCoroutine(CountdownFromLaunch());
        //   StartCoroutine(PlotPath());
    }

    void ResetTimeAndSetLaunchToFalse()
    {
        FreezeTime.StartTimeAgain();
       // ZoomOut();
        launching = false;
        currentSoulBehaviour.launching = false;
         
        NotLaunchingSoul();
    }
    IEnumerator CountdownFromLaunch()
    {

        yield return new WaitForSeconds(0.5f * 0.1f);
        ResetTimeAndSetLaunchToFalse();
    }

    void ZoomOnPlayer()
    {

        ourProCamera2D.Zoom(-3, 1.0f * 0.1f, EaseType.EaseInOut);
        zoomed= true;
    }

    void ZoomOut()
    {
        ourProCamera2D.Zoom(3, 1.0f, EaseType.EaseInOut);
        zoomed = false;
    }

    void ReturnToPlayer()
    {
        // ourProCamera2D.RemoveCameraTarget(darkStar.transform);
        // ourProCamera2D.AddCameraTarget(player.transform);
        // focusedOnDarkStar = false;
    }
    void Update()
    {
        if (Input.GetMouseButton(1) && poweredUp && !playerReferences.slingshot.priming)
        {
            holdStartTime = Time.time;
            StartCoroutine(PrimeSlingshot(ourSoulHandler.soulChargingUs));

        }
    }
    // Use this for initialization

}
