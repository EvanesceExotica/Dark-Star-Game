using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallForMateAction : GoapAction
{

    bool foundMate = false;

    bool callingForMate = false;

    float callDuration = 5.0f;

    public CallForMateAction()
    {
        AddPrecondition(new Condition("charge", true));
        AddEffect(new Condition("foundMate", true));
        cost = 100f;
    }







    public override void reset()
    {
        foundMate = false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //the blue dwarf must find a mate here 


       // target = enemySpawner.GetClosestAlly(ourType, this.gameObject);
        target = enemySpawner.GetClosestAlly(ourType, this.gameObject);
        ourGoapAgent.currentTarget = target;

        if (target != null)
        {
            return true;
        }
        else
        {

            return false;
        }
    }

    IEnumerator CallForMate()
    {
        hasVectorTarget = false;
        float startTime = Time.time;
        callingForMate = true;
        while (Time.time < startTime + callDuration)
        {
            if (interrupted)
            {
                callingForMate = false;
                yield break;
            }
            //Debug.Log("Calling");
            yield return null;
        }
        //Debug.Log("Action ended at " +  (int)Time.time);
        callingForMate = false;

        foundMate = true;

    }



    // GameObject FindMate()
    // {
    //     GameObject potentialMate = null;
    //     //change this
    //     BlueDwarf[] potentialBlueDwarfMates = GameObject.FindObjectsOfType<BlueDwarf>();
    //     List<GameObject> blueDwarfGOs = new List<GameObject>();
    //     if(potentialBlueDwarfMates.Length > 0)
    //     {
    //     }
    //     foreach(BlueDwarf bd in potentialBlueDwarfMates)
    //     {
    //         blueDwarfGOs.Add(bd.gameObject);
    //     }
    //     potentialMate = FindClosest.FindClosestObject(blueDwarfGOs, this.gameObject);

    //     return potentialMate;
    // }



    public override bool perform(GameObject agent)
    {

        if (!setPerformancePrereqs)
        {
//

            setPerformancePrereqs = true;
        }
        performing = true;

        if (!callingForMate)
        {
            StartCoroutine(CallForMate());
        }

        if (interrupted)
        {
            //Debug.Log("We're being interrupted! " + this.name);
            performing = false;
        }

        return performing;

    }

    public override bool requiresInRange()
    {

        return false;
    }

    public override bool isDone()
    {
        return foundMate;

    }


    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }
}
