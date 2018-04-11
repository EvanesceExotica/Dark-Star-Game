using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunbatheAction : GoapAction
{

    GameObject darkStar;

    bool charging = false;
    float radiusOfDarkStar;
    Vector2 darkStarPosition;

    ThreatTrigger threatTrigger;

    float bufferAmount = 4.0f;

    public float maxChargeAmount = 5;
    public float currentChargeAmount = 0;
    public float chargeInterval;


    public SunbatheAction()
    {
        //AddPrecondition(new Condition("charge", false));
        AddEffect(new Condition("charge", true));

        addPrecondition("charge", false);
        addEffect("charge", true);
        cost = 100f;
        canBeInterrupted = true;
    }



    public bool charged;

    IEnumerator Charge()
    {
        charging = true;
        while (currentChargeAmount < maxChargeAmount)
        {
            Debug.Log("Charging!");
            if (interrupted)
            {
                charging = false;
                break;
            }
            currentChargeAmount += chargeInterval;
            yield return new WaitForSeconds(1.0f);
        }
        charging = false;
    }



    public override void reset()
    {
        charged = false;
        currentChargeAmount = 0;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        hasVectorTarget = true;
        target = darkStar;
        //you can change this
        vectorTarget = FindLocationInSafeZone.FindLocationInCircleExclusion(darkStar, bufferAmount);
        // vectorTarget = FindSunbathingLocation(); 
        return true;
    }



    public override bool perform(GameObject agent)
    {
        if (!charging)
        {
            //TODO: This is the first go around and won't be called again, but I'd prefer having a separate method that's called once

            StartCoroutine(Charge());
        }
        performing = base.perform(agent);
        
        if (performing)
        {
            //if performing has been set to false, we want to skip all of this and return performing

            //base.perform(agent) will return false if the monster is interrupted or incapacitated
            if (currentChargeAmount == maxChargeAmount)
            {
                charged = true;
            }
        }

        return performing;

    }

    public override bool requiresInRange()
    {

        return true;
    }

    public override bool isDone()
    {
        return charged;

    }

    Vector2 FindSunbathingLocation()
    {
        Vector2 primeLocation = new Vector2(0, 0);
        do
        {
            primeLocation = UnityEngine.Random.insideUnitCircle * radiusOfDarkStar + darkStarPosition;
        }
        while (Vector2.Distance(primeLocation, darkStarPosition) < radiusOfDarkStar);

        primeLocation = new Vector2(primeLocation.x + bufferAmount, primeLocation.y + bufferAmount);

        return primeLocation;
    }

    public override void Awake()
    {
        base.Awake();
        darkStar = gameStateHandler.darkStar;
        // Debug.Log(darkStar);
        radiusOfDarkStar = darkStar.GetComponent<CircleCollider2D>().bounds.extents.x;
        darkStarPosition = darkStar.transform.position;

    }

    // Use this for initialization
    void Start()
    {

        cost = 200.0f;
        maxChargeAmount = 10.0f;
        chargeInterval = 1.0f;
        currentChargeAmount = 5.0f;
        threatTrigger = GetComponentInChildren<ThreatTrigger>();
        threatTrigger.threatInArea += this.ImportantEventTriggered;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
