﻿using UnityEngine;
using System.Linq;
using System.Collections;

using System.Collections.Generic;

public class DropTrack : PowerUp
{

    LineRenderer ourLineRenderer;
    TrailRenderer ourTrailRenderer;
    public GameObject anchorSwitchGO;
    PlayerReferences pReference;
    LocationHandler playerLocationHandler;
    GameObject groundCheck;
    public LayerMask whatIsSwitch;
    public GameObject nextAnchorSwitch;
    GameObject trackDropParticlesGO;
    List<ParticleSystem> trackDropParticles;

    public float maxTrackDropInterval = 10.0f;
    public bool droppingTrack;
    GameStateHandler gameStateHandler;

    Conduit conduit;
    public override void Awake()
    {
        base.Awake();
       gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        extendableDuration = true;
        extendableIntervalDuration = 10.0f;
        autoActivated = true;
        ourRequirement = Requirement.OnlyUseOnSwitch;
        ChoosePowerUp.connectorChosen += this.SetPoweredUp;
        conduit = GameObject.Find("Switch Holder").GetComponent<Conduit>();
        // Switch.SwitchEntered += this.SetOnSwitch;
        // Switch.SwitchEntered += this.SetOffSwitch;
    }
    void StartDroppingTrack()
    {
        ourLineRenderer.enabled = true;
    }
    // Use this for initialization
    void StopDroppingTrack()
    {
        ourLineRenderer.enabled = false;
    }

    public override void SetOnSwitch(GameObject currentSwitch)
    {
        base.SetOnSwitch(currentSwitch);
        if (!droppingTrack)
        {
            anchorSwitchGO = currentSwitch;
        }
        else
        {
            nextAnchorSwitch = currentSwitch;
        }
    }

    void Start()
    {
        maxTrackDropInterval = 10.0f;
        pReference = GameObject.Find("Player").GetComponent<PlayerReferences>();
        playerLocationHandler = pReference.locationHandler;
        Switch.SwitchEntered += this.SetOnSwitch;
        ourLineRenderer = GetComponent<LineRenderer>();
        trackDropParticlesGO = GameObject.Find("TrackDropParticleSystem");
        trackDropParticles = trackDropParticlesGO.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    public override void StartPowerUp(){
        base.StartPowerUp();
        ParticleSystemPlayer.PlayChildParticleSystems(trackDropParticles);
        StartCoroutine(DropSomeTrack());
    }
    #region 
    //  void CreateTears(List<Vector2> tearLocations)
    //{
    //    StartCoroutine(FormTears(tearLocations));
    //    //GameObject instantiatedTear = Instantiate(baseTearPrefab);
    //}

    //IEnumerator FormTears(List<Vector2> tearLocations)
    //{
    //    ourLineRenderer.SetPosition(0, anchorSwitch.position);
    //    int i = 0;
    //    float startTime = Time.time;
    //    while(i < tearLocations.Count)
    //    {
    //        //ourLineRenderer.positio
    //      //  Instantiate(baseTearPrefab, tearLocations[i], Quaternion.identity, this.gameObject.transform);
    //        i++;
    //        yield return new WaitForSeconds(1.5f);
    //    }
    //    //foreach (Vector2 location in tearLocations)
    //    //{
    //    //    Instantiate(baseTearPrefab, location, Quaternion.identity, this.gameObject.transform);
    //    //}
    //}
    #endregion
    public IEnumerator DropSomeTrack()
    {
        //Debug.Log("We did at least START dropping track");
        //TODO: MAKE SURE THE IGNORE COLLIDER DOESN'T AFFECT THE PLANET WALKING
        //TODO: MAKE SURE THAT THE ANCHOR SWITCH IS THE RIGHT ANCHOR SWITCH
        droppingTrack = true;
        RaycastHit2D hit;// = Physics2D.Raycast(anchorpoint, direction, distance * 30, whatIsSwitch);
        float startTime = Time.time;
        while (Time.time < startTime + extendableIntervalDuration)
        {
            // anchorSwitch = pReference.locationHandler.currentSwitch;
            hit = RaycastToEnd(anchorSwitchGO.transform.position, playerLocationHandler.groundCheck.transform.position);
            //This is raycasting from the anchor switch to the player's position so that once the player stretches from the first switch
            //to the second, it will make a connection. 
            if (hit)
            {
                if (hit.collider.GetComponent<Switch>() != null && hit.collider.gameObject != anchorSwitchGO)
                {
                    //Debug.Log("We hit a switch");
                    //  //Debug.Log("We have formed a connection");
                    //anchorSwitch.GetComponent<Switch>().MakeConnection(hit.collider.gameObject, false);
                    Switch anchorSwitch = anchorSwitchGO.GetComponent<Switch>();
                    anchorSwitch.AddSwitchConnectionAndSubscribe(hit.collider.gameObject, anchorSwitch.CreateNewSwitchConnection());

                    if (nextAnchorSwitch != null && anchorSwitchGO != nextAnchorSwitch)
                    {
                        anchorSwitchGO = nextAnchorSwitch;
                    }
                    //I have no idea why I added that new WaitForSeconds down there VV
                    yield return new WaitForSeconds(0.034f);
                    StartCoroutine(DropSomeTrack());
                    yield break;
                }

            }

            //Debug.Log(startTime + "," + (int)(startTime + maxTrackDropInterval));


            yield return null;
        }
        //so this SHOULD continue dropping track and start a new track drop if the player reaches another switch, but if nothing happens in 4 seconds, stop. 
       
        TracksStoppedDropping();

    }

    void TracksStoppedDropping()
    {
        //this is to give some buffer space between track dropping -- the soul can't be used for anything else. 
        StoppedUsingPowerUpWrapper();
        droppingTrack = false;
        ParticleSystemPlayer.StopChildParticleSystems(trackDropParticles);
       // pReference.playerSoulHandler.Depowered();

    }


    RaycastHit2D RaycastToEnd(Vector2 anchorPoint, Vector2 followTarget)
    {
        //Debug.Log("<color=blue>We're raycasting </color>");
        Vector2 heading = followTarget - anchorPoint;
        var distance = heading.magnitude;
        var direction = heading / distance;

        RaycastHit2D ourRaycastHit = Physics2D.Raycast(anchorPoint, direction, distance, whatIsSwitch);
        Debug.DrawRay(anchorPoint, direction * distance, Color.green);
        return ourRaycastHit;
    }

    void PowerUpTrackDroper()
    {
        if (!droppingTrack && anchorSwitchGO != null)
        {
            ParticleSystemPlayer.PlayChildParticleSystems(trackDropParticles);
            StartCoroutine(DropSomeTrack());
        }
    }

    void ConnectAllSwitchesForTest(){
        List<Switch> ourSwitches = conduit.ourSwitches.ToList();
        foreach(Switch ourSwitche in conduit.ourSwitches){
            if(ourSwitche.GetComponent<Core>() != null){
                ourSwitches.Remove(ourSwitche);
            }
        }
        for(int i = 0; i < ourSwitches.Count; i++){
            Switch a = ourSwitches[i];
            Switch b = ourSwitches[i+1];
            a.AddSwitchConnectionAndSubscribe(b.gameObject, a.CreateNewSwitchConnection());
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.C)){
            //gameStateHandler.connectionHolder.AddSwitchConnectionAndSubscribe(SwitchHolder[0]);
            // Switch a = conduit.ourSwitches[0];
            // Switch b = conduit.ourSwitches[2];
            // a.AddSwitchConnectionAndSubscribe(b.gameObject, a.CreateNewSwitchConnection());
            ConnectAllSwitchesForTest();
           
        }
        //  anchorSwitch = playerLocationHandler.currentSwitch;
        //    //Debug.Log(pReference.playerSoulHandler.currentChargeState.ToString());
        // if (pReference.playerSoulHandler.currentChargeState == PlayerSoulHandler.ChargeStates.soulCharged && Input.GetKeyDown(KeyCode.Alpha1) && !droppingTrack && anchorSwitch != null)
        // {
        //     //Debug.Log("<color=red>THE TRACK IS BEING DROPPED!!!!!</color>");
        //     PowerUpTrackDroper();
        // }
    }

    
}
