using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SpiralPatrolAction : GoapAction
{

    float circleSpeed;
    float circleSize;
    float circleGrowSpeed;
    bool touchedSwitch;
    public List<GameObject> switchesTouched = new List<GameObject>();
    float forwardSpeed;
    //the comet travels in spirals around the star, leaving temporary trails that are destroyed at after each phase (maybe use the "waypoint" system?)
    //the player can ride the trails?

    public override void Awake()
    {
        base.Awake();
        Switch.AnythingEnteredSwitch += this.AddSwitchWeTouched;
        ourThreatTrigger.threatInArea += this.ImportantEventTriggered;

    }

    public override void ImportantEventTriggered(GameObject intruder){
        interrupted = true;
    }


    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            performing = true;
            StartCoroutine(Spiral());
        }
        performing = base.perform(agent);
        return performing;

    }

    public IEnumerator Spiral()
    {
        circleSize = DarkStar.radius + 5.0f;
        circleGrowSpeed = 0.3f;
        circleSpeed = 1.0f;
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        while (circleSize <= GameStateHandler.voidBoundaryRadius)
        {
            if (interrupted)
            {
                yield break;
            }
            if (touchedSwitch)
            {
                //we touched one switch

                touchedSwitch = false;
            }
            if (switchesTouched.Count == 2)
            {
                break;
            }
            // circleSpeed = frequency, circleSize = amplitude -- the phase shift is the only thing that's "added" rather than multiplied so to speak
            xPosition = circleSize * Mathf.Sin(Time.time * circleSpeed);
            yPosition = circleSize * Mathf.Cos(Time.time * circleSpeed);
            circleSize += circleGrowSpeed;
            transform.position = new Vector2(xPosition, yPosition);
            yield return null;
        }
        if (switchesTouched.Count == 2)
        {
            SetAgentTarget(switchesTouched.Last()); 
             ourRigidbody2D.velocity = new Vector2(0, 0);
            hasTouchedTwoSwitches = true;
        }
        else{
            performing = false;
        }
      

    }

    public void AddSwitchWeTouched(GameObject objectThatEnteredSwitch, GameObject switchTouched)
    {
        Debug.Log("We're adding switches we touched");
        Debug.Log("This is the object that entered the switch " + objectThatEnteredSwitch.name);

        Debug.Log("This is the switch we entered " + switchTouched.name);
        if (switchTouched.GetComponent<Switch>().GetType() != typeof(Core))
        {
            //we don't want the core of a planet to count here
            if (objectThatEnteredSwitch == this.gameObject)
            {
                switchesTouched.Add(switchTouched);
            }
        }
    }



    public SpiralPatrolAction()
    {
        cost = 100f;
        AddEffect(new Condition("trail", true));
    }
    // Use this for initialization
    bool hasTouchedTwoSwitches;
    bool playerHit;


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
        return true;
    }



    public override bool isDone()
    {
        return hasTouchedTwoSwitches;
    }
    // Use this for initialization



}
