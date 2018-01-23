﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AsexuallyReproduceAction : GoapAction
{

    float splitDuration = 10.0f;
    bool currentlySplitting;
    bool successfullySplit = false;

    public GameObject offspringPrefab;
    public GameObject reproductionParticleEffectGO;
    public List<ParticleSystem> reproductionParticleEffectList; 

    public AsexuallyReproduceAction()
    {
        AddPrecondition(new Condition("charge", true));
        AddEffect(new Condition("reproduce", true));
        cost = 400f;
    }

    IEnumerator Split()
    {
        float startTime = Time.time;
        currentlySplitting = true;
        while(Time.time < startTime + splitDuration)
        {
            if (interrupted)
            {
                currentlySplitting = false;
                yield break;
            }
            yield return null;
        }
        currentlySplitting = false;
        successfullySplit = true;
        InstantiateOffspring(); 
    }


    void InstantiateOffspring()
    {
        //Instantiate(reproductionParticleEffect, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f), Quaternion.identity); 
        ParticleSystemPlayer.PlayChildParticleSystems(reproductionParticleEffectList);
        Instantiate(offspringPrefab, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f), Quaternion.identity);
    }
   



    public override void reset()
    {
        successfullySplit = false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //the blue dwarf must find a mate here 

        hasVectorTarget = false;


        target = GameObject.Find("Dark Star");

        if (target != null)
        {
            return true;
        }
        else
        {

            return false;
        }
    }


    void FindNewArea()
    {

    }

  



    public override bool perform(GameObject agent)
    {
        performing = true;

        if (!currentlySplitting)
        {
            StartCoroutine(Split());
        }
        if (interrupted)
        {
            ////Debug.Log("We're being interrupted!" + " " + this.name);
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
        return successfullySplit;

    }

void Awake(){
    reproductionParticleEffectGO = transform.Find("ReproductionParticleEffect").gameObject ;
    reproductionParticleEffectList = reproductionParticleEffectGO.GetComponentsInChildren<ParticleSystem>().ToList();
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
