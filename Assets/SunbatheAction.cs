using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunbatheAction : GoapAction {

    GameObject darkStar;

    bool charging = false;
    float radiusOfDarkStar;
    Vector2 darkStarPosition;

    ThreatTrigger threatTrigger;

    float bufferAmount = 4.0f;

    public float maxChargeAmount = 5;
    public float currentChargeAmount = 0;
    public float chargeInterval;
    

    public SunbatheAction(){
        //AddPrecondition(new Condition("charge", false));
        AddEffect(new Condition("charge", true));

        addPrecondition("charge", false);
        addEffect("charge", true);
        cost = 100f;
    }



    public bool charged;

    IEnumerator Charge()
    {
        charging = true;
        while (currentChargeAmount < maxChargeAmount)
        {
//            Debug.Log("Charging!");
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
//        currentChargeAmount = 0;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // hasVectorTarget = true;
        // target = GameObject.Find("Dark Star");
        // vectorTarget = FindSunbathingLocation(); 
        return true;
    }



    public override bool perform(GameObject agent)
    {
        if(!setPerformancePrereqs){
            //this goes before everything else to make sure targets are had and such

            hasVectorTarget = true;
            target = darkStar;
            vectorTarget = FindSunbathingLocation();

            setPerformancePrereqs = true;
        }

         performing = true;
        if (!charging)
        {
            
            StartCoroutine(Charge());
        }
        if (currentChargeAmount == maxChargeAmount)
        {
            charged = true;
        }

        if (interrupted)
        {
//            Debug.Log("We're being interrupted!" + " " + this.name);
            performing = false; 
            interrupted = false;
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

    private void Awake()
    {
        darkStar = GameObject.Find("Dark Star");
        Debug.Log(darkStar);
        radiusOfDarkStar = darkStar.GetComponent<CircleCollider2D>().bounds.extents.x;
        darkStarPosition = darkStar.transform.position;

    }

    // Use this for initialization
    void Start () {

        cost = 200.0f;
        maxChargeAmount = 10.0f;
        chargeInterval = 1.0f;
        currentChargeAmount = 5.0f;
        threatTrigger = GetComponentInChildren<ThreatTrigger>();
        threatTrigger.threatInArea += this.importantEventTriggered;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
