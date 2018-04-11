﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherOnSwitchAction : GoapAction {

	float circleSpeed;
    float circleSize;
    float circleGrowSpeed;
    bool touchedSwitch;
    List<GameObject> switchesTouched= new List<GameObject>();
    float forwardSpeed;
    //the comet travels in spirals around the star, leaving temporary trails that are destroyed at after each phase (maybe use the "waypoint" system?)
    //the player can ride the trails?
    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            StartCoroutine(SleepOnSwitch());
            performing = true;
        }
        return performing;

    }

    public GatherOnSwitchAction()
    {
        cost = 200f;
        AddPrecondition(new Condition("trail", true));
        AddEffect(new Condition("hibernate", true));
    }
    // Use this for initialization
    bool hasTouchedTwoSwitches;


    public override void reset()
    {
        hasTouchedTwoSwitches = false;
    }
    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = GetCurrentTarget();
        return true;
    }
    float startTime;
    float duration = 20.0f; 

    bool sleeping;
    bool hasHibernated;
     IEnumerator SleepOnSwitch()
    {
        EnableBounceCollider();
        startTime = Time.time;
        sleeping = true;
        //setting the rigidbody to kinematic so that it's not effected by phyiscs any longer.
        //TODO: IF it's hit by a hook or the player, set it back to dynamic?
        // --- > ourGoapAgent.enemy.ourRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        while(Time.time < startTime + duration) 
        {
            if (interrupted)
            {
                sleeping = false;
                break;
            }
            yield return null;
        }
        sleeping = false;
        hasHibernated = true;
    }

    void EnableBounceCollider(){
        transform.GetChild(1).gameObject.SetActive(true);
    }
   

   
    public override bool isDone()
    {
        return hasHibernated;
    }
    // Use this for initialization

}
