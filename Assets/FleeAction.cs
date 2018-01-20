using UnityEngine;
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

    public bool freeFromEnemy;

    UniversalMovement movement;
    ThreatTrigger ourThreatTrigger;
    public FleeAction()
    {
        //if the creature hasn't charged, it will default to this 
        //maybe some shenanigans with the player pulling a mate to the one to make more -- perhaps put a range on the mate call?  
        AddEffect(new Condition("stayAlive", true));
        cost = 500f;
    }

    public void WereSafe(){
        freeFromEnemy = true;
    }

    public override void importantEventTriggered(GameObject intruder)
    {
        target = intruder;
        if(target == gameStateHandler.darkStar){
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
            freeFromEnemy = false;
            isDashing = true;
        }
        if (interrupted)
        {
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
        isDashing = false;
        freeFromEnemy = true;
    }
    // Use this for initialization
    void Awake(){
        ourThreatTrigger = gameObject.GetComponentInChildren<ThreatTrigger>();
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        ourThreatTrigger.SetAllClear += this.WereSafe;
    }
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
          if(target = gameStateHandler.darkStar){
              targetRangeBuffer = gameStateHandler.darkStar.GetComponent<CircleCollider2D>().bounds.extents.x + 10.0f;
            
          }
            if(Vector2.Distance(transform.position, target.transform.position) > targetRangeBuffer && freeFromEnemy)
            {
              
              
                isDashing = false;
                dashed = true;
                Debug.Log("<color=yellow>PHEW I GOT AWAY </color> " + gameObject.name);
                // TODO: After this, make sure you fix that they stop moving, perhaps reset the "interrupted" variables in the other classes
            }
            movement.rb.AddForce(-GetDirection(this.gameObject) * dashSpeed);// Vector2.MoveTowards(transform.position, dashTarget, dashSpeed);
        }

    }
}
