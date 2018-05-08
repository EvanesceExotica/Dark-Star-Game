﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
public class SpiralPatrolAction : GoapAction
{

    bool spiraledOutwardAlready; //this should NOT be changed
    float circleSpeed;
    float circleSize;
    float circleGrowSpeed;
    bool touchedSwitch;

    bool inSwitchTrigger;

    bool startedSpiraling;

    public RecordPointsBetweenSwitches pointRecorder;
    public List<GameObject> switchesTouched = new List<GameObject>();
    float forwardSpeed;

    TrailRenderer ourTrailRenderer;
    //the comet travels in spirals around the star, leaving temporary trails that are destroyed at after each phase (maybe use the "waypoint" system?)
    //the player can ride the trails?


    float timeWePausedAndRecalculated;
    public override void Awake()
    {
        base.Awake();
        pointRecorder = GetComponent<RecordPointsBetweenSwitches>();
        Switch.AnythingEnteredSwitch += this.AddSwitchWeTouched;
        Switch.AnythingExitedSwitch += this.SwitchExited;
        ourThreatTrigger.threatInArea += this.ImportantEventTriggered;
        canBeInterrupted = true;
        ourTrailRenderer = GetComponent<TrailRenderer>();
    }

    void DisableTrailRenderer()
    {
        ourTrailRenderer.time = 0.5f;
        //ourTrailRenderer.enabled = false;
    }

    void SetTrailRendererLifeTime()
    {
        ourTrailRenderer.enabled = true;
    }


    public override void ImportantEventTriggered(GameObject intruder)
    {
        interrupted = true;
    }

    public override void PrepareCurrentAction()
    {
        Debug.Log(this.ToString() + "<color=green> is preparing to perform </color>");
        performing = true;
    }


    public override bool perform(GameObject agent)
    {
        if (!startedSpiraling)
        {
            StartCoroutine(Spiral());
        }

        performing = base.perform(agent);
        return performing;

    }

    public override void CleanUpAction()
    {
        base.CleanUpAction();
        Debug.Log(this.ToString() + " <color=green>is cleaning up!</color>");
    }

    public Vector2 DetermineSpiralVectorTarget()
    {

        // Debug.Log("Time when first calculated " + Time.time);
        float xPosition = circleSize * Mathf.Sin(Time.time * circleSpeed);
        float yPosition = circleSize * Mathf.Cos(Time.time * circleSpeed);
        Vector2 vecTarget = new Vector2(xPosition, yPosition);
        return vecTarget;
    }

    public IEnumerator Spiral()
    {
        startedSpiraling = true;
        if (!spiraledOutwardAlready)
        {
            circleSize = DarkStar.radius - 4.0f;
            circleGrowSpeed = 0.05f;
            circleSpeed = 1.0f;
        }
        else{
            //You want to reverse the grow speed of the circle so it will start getting smaller, but not the frequency of the circle (which is what "CircleSpeed" is. Better naming maybe?)
            //changing the frequency (which I shouldn't have done) just caused it to reverse the side of the circle it was on
            circleGrowSpeed = -0.05f;
            circleSpeed = 1.0f;
        }
        // circleSize = Vector2.Distance(vectorTarget, GameStateHandler.DarkStarGO.transform.position);
        // //circleSize = DarkStar.radius + 3.0f;
        // circleGrowSpeed = 0.1f;
        // circleSpeed = 1.0f;
        //Right here, the x and y position should already be within the circle
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        float timeAugment=timeWePausedAndRecalculated;
        Debug.Log("Time we paused " + timeAugment);
        Debug.Log("Time now " + Time.time);
        while (true)
        {
            if(!spiraledOutwardAlready){
                timeAugment = Time.time;
                if(circleSize >= GameStateHandler.voidBoundaryRadius -3){
                    break;
                }
            }
            else{
                Debug.Log("<color=cyan>We should be spiraling inward</color>");
                //timeAugment = timeWePausedAndRecalculated + Time.deltaTime;
                //timeAugment = Time.time - (Time.time - timeWePausedAndRecalculated);
                //TODO: The GO should self destruct at some point by running into the star
            }
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
            //TODO: We need to change it so that the vector target is the below value
            // Debug.Log("Time now "+ Time.time);
            //change this bck to TimeAugment potentially
            xPosition = circleSize * Mathf.Sin(Time.time * circleSpeed);
            yPosition = circleSize * Mathf.Cos(Time.time * circleSpeed);
            circleSize += circleGrowSpeed;
            transform.position = new Vector2(xPosition, yPosition);
            if(spiraledOutwardAlready){
                timeAugment += Time.deltaTime;
            }
            //TODO: Change this
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
            if (!spiraledOutwardAlready)
            {
                timeWePausedAndRecalculated = Time.time;
                ChangeToSpiralInward();
            }
            performing = false;
        }


    }


    void ChangeToSpiralInward()
    {
        Debug.Log("Changing to spiral inward!");
        spiraledOutwardAlready = true;
        RemoveEffect("spiralOutward");
        AddEffect(new Condition("spiralInward", true));
        //AddPrecondition(new Condition("spiralOutward", true));
        //TODO: MAybe have it somehow add the "SpiralOutward" as a precondition? But then  you'd have to double up on code

        foreach (Condition con in _effects)
        {
            Debug.Log("Spiral patrol effect " + con.Name);
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
        AddEffect(new Condition("spiralOutward", true));
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
        startedSpiraling = false;
        switchesTouched.Clear();
    }
    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        hasVectorTarget = true;
        target = GameStateHandler.DarkStarGO;
        //vectorTarget = FindLocationInSafeZone.FindLocationInCircleExclusion(GameStateHandler.DarkStarGO, 1.0f);
        vectorTarget = DetermineSpiralVectorTarget();
        return true;
    }


    void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(vectorTarget, Color.cyan, 2);
    }

    public override bool isDone()
    {
        return hasTouchedTwoSwitches;
    }
    // Use this for initialization



}
