using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeAction : GoapAction {

    Vector2 dashTarget;
    bool dashed;
    float dashSpeed;
    bool isDashing;


    public EvadeAction()
    {
        // addPrecondition("hookInRange", true);
        addPrecondition("charge", true);
        addEffect("findNewArea", true);
        addEffect("stayAlive", true);
        cost = 200f; 
    }


    public override void reset()
    {

    }

    public override bool isDone()
    {
        return dashed;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = GameObject.Find("Player");
        return true; 
    }

    public override bool perform(GameObject agent)
    {
        SpaceMonster currentSpaceMonster = agent.GetComponent<SpaceMonster>();
        if (currentSpaceMonster.stamina >= (500 - cost) && !isDashing)
        {
            currentSpaceMonster.stamina -= (500 - cost);
            dashTarget = Random.insideUnitCircle + (Vector2)transform.position * 3;
            isDashing = true;

            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool requiresInRange()
    {
        return false; 
    }

  
	
	// Update is called once per frame
	void Update () {
        if (isDashing)
        {
            transform.position = Vector2.MoveTowards(transform.position, dashTarget, dashSpeed);
        }
        if((Vector2)transform.position == dashTarget)
        {
            isDashing = false;
            dashed = true;
        }
		
	}
}
