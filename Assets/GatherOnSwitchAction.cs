using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherOnSwitchAction : GoapAction
{

    float circleSpeed;
    float circleSize;
    float circleGrowSpeed;
    bool touchedSwitch;
    List<GameObject> switchesTouched = new List<GameObject>();
    float forwardSpeed;
    //the comet travels in spirals around the star, leaving temporary trails that are destroyed at after each phase (maybe use the "waypoint" system?)
    //the player can ride the trails?

    public override void PrepareCurrentAction()
    {
        StartCoroutine(SleepOnSwitch());
        performing = true;
    }
    public override bool perform(GameObject agent)
    {
        performing = base.perform(agent);
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
        Debug.Log("Sleep on switch IS being triggered");
        EnableBounceCollider();
        startTime = Time.time;
        sleeping = true;
        //setting the rigidbody to kinematic so that it's not effected by phyiscs any longer.
        //TODO: IF it's hit by a hook or the player, set it back to dynamic?
        // --- > ourGoapAgent.enemy.ourRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        while (Time.time < startTime + duration)
        {
            Debug.Log("Sleep on switch IS being triggered IN WHILE LOOP");
            if (incapacitated)
            {
                sleeping = false;
                break;
            }
            yield return null;
        }
        sleeping = false;
        hasHibernated = true;
        //TODO:  HAVE THE COMET TRANSFORM INTO A PLANET
        //TODO: Maybe have the comet become a planet if another comet collides with the switch?
    }

    void EnableBounceCollider()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }



    public override bool isDone()
    {
        return hasHibernated;
    }
    // Use this for initialization

}
