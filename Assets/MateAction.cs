using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MateAction : GoapAction
{

    bool currentlyMating = false;
    float mateDuration = 10.0f;
    bool reproduced = false;

    public GameObject offspringPrefab;
    public GameObject reproductionParticleEffect;

    public MateAction(){

        AddPrecondition(new Condition("foundMate", true));
        AddEffect(new Condition("reproduce", true));
        cost = 200f;

    }

    GameObject FindMate()
    {
        GameObject potentialMate = null;
        //change this
        BlueDwarf[] potentialBlueDwarfMates = GameObject.FindObjectsOfType<BlueDwarf>();
        List<GameObject> blueDwarfGOs = new List<GameObject>();
        if (potentialBlueDwarfMates.Length > 0)
        {
            //Debug.Log("Found some mates");
        }
        foreach (BlueDwarf bd in potentialBlueDwarfMates)
        {
            blueDwarfGOs.Add(bd.gameObject);
        }
        if(blueDwarfGOs.Count == 1 && blueDwarfGOs.Contains(this.gameObject))
        {
            return null;
        }
        potentialMate = FindClosest.FindClosestObject(blueDwarfGOs, this.gameObject);

        return potentialMate;
    }


    public override void reset()
    {
        reproduced = false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {



        target = enemySpawner.GetClosestAlly(ourType, this.gameObject);

        if (target != null)
        {
            //Debug.Log(target.name);
            return true;
        }
        else
        {

            return false;
        }
    }

    IEnumerator Mate()
    {
        float startTime = Time.time;
        currentlyMating = true;
        while (Time.time < startTime + mateDuration)
        {
            if (interrupted)
            {
                currentlyMating = false;
                yield break;
            }
            //Debug.Log("Mating");
            yield return null;
        }
        //Debug.Log("Action ended at " + (int)Time.time);
        currentlyMating = false;
        reproduced = true;
        InstantiateOffspring(); 
    }



    void InstantiateOffspring()
    {
        if (offspringPrefab != null)
        {
            Instantiate(reproductionParticleEffect, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f), Quaternion.identity);
            Instantiate(offspringPrefab, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f), Quaternion.identity);
        }
    }





    public override bool perform(GameObject agent)
    {
        if(!setPerformancePrereqs){

            hasVectorTarget = false;
            setPerformancePrereqs = true;
        }
        performing = true;

        if (!currentlyMating)
        {
            StartCoroutine(Mate());
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

        return true;
    }

    public override bool isDone()
    {
        return reproduced;

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
