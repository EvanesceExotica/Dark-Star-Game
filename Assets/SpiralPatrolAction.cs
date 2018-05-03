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

    bool inSwitchTrigger;

    public RecordPointsBetweenSwitches pointRecorder;
    public List<GameObject> switchesTouched = new List<GameObject>();
    float forwardSpeed;
    //the comet travels in spirals around the star, leaving temporary trails that are destroyed at after each phase (maybe use the "waypoint" system?)
    //the player can ride the trails?

    public override void Awake()
    {
        base.Awake();
        pointRecorder = GetComponent<RecordPointsBetweenSwitches>();
        Switch.AnythingEnteredSwitch += this.AddSwitchWeTouched;
        Switch.AnythingExitedSwitch += this.SwitchExited;
        ourThreatTrigger.threatInArea += this.ImportantEventTriggered;
        canBeInterrupted = true;

    }

    public override void ImportantEventTriggered(GameObject intruder)
    {
        interrupted = true;
    }


    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            //TODO: this is where the issue is coming from. You NEED a separate method to be called once.
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
        while (circleSize <= GameStateHandler.voidBoundaryRadius )
        {
            if (interrupted || incapacitated)
            {
                if (pointRecorder != null)
                {
                    if (pointRecorder.recording)
                    {
                        pointRecorder.Cancel();
                    }
                }
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
        Debug.Log("Spiral Patrol actually ended! ");
        if (switchesTouched.Count == 2)
        {
            SetAgentTarget(switchesTouched.Last());
            ourRigidbody2D.velocity = new Vector2(0, 0);
            hasTouchedTwoSwitches = true;
        }
        else
        {
            Debug.Log("<color=cyan>We should no longer be performing</color>");
            //TODO: HAve some solution for them failing so they don't jerk all over the place
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
                inSwitchTrigger = true;
                currentSwitch = switchTouched;
                StartCoroutine(CheckIfCloseEnough(switchTouched));

            }
        }
    }

    public void SwitchExited(GameObject switchWeWereIn, GameObject objectThatExitedSwitch)
    {
        if (objectThatExitedSwitch == this.gameObject && switchWeWereIn == currentSwitch)
        {
            currentSwitch = null;
            inSwitchTrigger = false;
        }
    }

    public IEnumerator CheckIfCloseEnough(GameObject touchedSwitch)
    {
        while (inSwitchTrigger)
        {
            if (PassedCloseEnoughToSwitchCenter(touchedSwitch))
            {
                if (!switchesTouched.Contains(touchedSwitch))
                {
                    //make sure the same switch isn't being triggered
                    switchesTouched.Add(touchedSwitch);
                    if (!recording)
                    {
                        StartCoroutine(RecordPoints());
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    bool PassedCloseEnoughToSwitchCenter(GameObject touchedSwitch)
    {
        //TODO: This is the distance when the switch is entered. 
        bool passedCloseEnough = false;
        if (Vector2.Distance(this.gameObject.transform.position, touchedSwitch.transform.position) <= 1.5f)
        {
            passedCloseEnough = true;
        }
        return passedCloseEnough;

    }

    bool recording;
    GameObject currentSwitch;

    public List<Vector3> points = new List<Vector3>();
    IEnumerator RecordPoints()
    {
        recording = true;
        while (!hasTouchedTwoSwitches && !interrupted && !incapacitated)
        {
            Debug.Log("Adding points");
            points.Add(transform.position);
            yield return null;
            //yield return new WaitForSeconds(0.5f);
        }
        if (hasTouchedTwoSwitches)
        {
            Switch ourSwitch = switchesTouched[0].GetComponent<Switch>();
            //change this number back for duration
            ourSwitch.AddTemporarySwitchConnectionAndSubscribe(switchesTouched[1], ourSwitch.CreateNewSwitchConnection(), points, 150.0f);
        }
    }

    public List<Vector3> ReturnPlottedPath()
    {
        return points;
    }



    public SpiralPatrolAction()
    {
        cost = 100f;
        AddEffect(new Condition("trail", true));
    }
    // Use this for initialization
    bool hasTouchedTwoSwitches;


    public override void reset()
    {
        performing = false;
        hasTouchedTwoSwitches = false;
        recording = false;
        vectorTarget = new Vector2(0, 0);
        switchesTouched.Clear();
    }
    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        hasVectorTarget = true;
        target = GameStateHandler.DarkStarGO;
        vectorTarget = FindLocationInSafeZone.FindLocationInCircleExclusion(GameStateHandler.DarkStarGO, 3.0f);
        return true;
    }



    public override bool isDone()
    {
        return hasTouchedTwoSwitches;
    }
    // Use this for initialization



}
