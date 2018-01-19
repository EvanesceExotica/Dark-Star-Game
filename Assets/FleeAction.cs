﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FleeAction : GoapAction
{
    bool playerInRange = false;

    Vector2 dashTarget;
    public bool dashed;
    float dashSpeed = 5.0f;
    public bool isDashing;
    public Vector2 fleeDirection;
    public float targetRangeBuffer; 

    UniversalMovement movement;

    public FleeAction()
    {
        //if the creature hasn't charged, it will default to this 
        //maybe some shenanigans with the player pulling a mate to the one to make more -- perhaps put a range on the mate call?  
        AddEffect(new Condition("stayAlive", true));
        cost = 500f;
    }

    public override void importantEventTriggered(GameObject intruder)
    {
        target = intruder;
        if(target == gameStateHandler.darkStar){
            Debug.Log("Increasing the distance we must flee due to Dark Star");
            targetRangeBuffer = DarkStar.radius + 5.0f;
        }
        else{
            targetRangeBuffer = 5.0f;
        }
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = GameObject.Find("Dark Star");
        fleeDirection = GetDirection(this.gameObject);
        return true;
    }

    public override bool isDone()
    {
        return dashed;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override bool perform(GameObject agent)
    {
        performing = true;
        SpaceMonster currentSpaceMonster = agent.GetComponent<SpaceMonster>();
        if (!isDashing)
        {
          //  dashTarget = UnityEngine.Random.insideUnitCircle + (Vector2)transform.position * 3;
            isDashing = true;

        }
        if (interrupted)
        {
            Debug.Log("We're being interrupted!" + this.name);
            performing = false;
        }

        return performing;
    }

    Vector2 GetDirection(GameObject agent)
    {
        Vector2 direciton = new Vector2(0, 0);
        var heading = target.transform.position - agent.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;

        return direction; 

    }

    public override void reset()
    {
        dashed = false;
    }
    // Use this for initialization
    void Start()
    {
        dashSpeed = 4.0f;
        movement = GetComponent<UniversalMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
          //  Debug.Log("I'm dashing");
          //TODO: Put some sort of buffer here for when the star is growing
            if(Vector2.Distance(transform.position, target.transform.position) > targetRangeBuffer)
            {
                isDashing = false;
                dashed = true;
              //  Debug.Log("PHEW I GOT AWAY " + gameObject.name);
            }
            movement.rb.AddForce(fleeDirection * dashSpeed);// Vector2.MoveTowards(transform.position, dashTarget, dashSpeed);
        }

    }
}
