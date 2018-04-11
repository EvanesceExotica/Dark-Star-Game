using UnityEngine;
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
        canBeInterrupted = true;
    }

    IEnumerator Split()
    {
        float startTime = Time.time;
        currentlySplitting = true;
        while (Time.time < startTime + splitDuration)
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
        ParticleSystemPlayer.PlayChildParticleSystems(reproductionParticleEffectList);
        Vector2 birthPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f);
        BlueDwarf dataProvider = (BlueDwarf)ourGoapAgent.DataProvider; 
        enemySpawner.SpawnIndependent(dataProvider, birthPosition);
        //Instantiate(reproductionParticleEffect, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f), Quaternion.identity); 
        //BlueDwarf newBlueDwarf = dataProvider.GetPooledInstance<BlueDwarf>();
        //newBlueDwarf.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f);
    }




    public override void reset()
    {
        successfullySplit = false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

        hasVectorTarget = false;
        target = gameStateHandler.darkStar;
        return true;

    }


    void FindNewArea()
    {

    }





    public override bool perform(GameObject agent)
    {
        if (!setPerformancePrereqs)
        {

            setPerformancePrereqs = true;
        }
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

    public override void Awake()
    {
        base.Awake();
        reproductionParticleEffectGO = transform.Find("ReproductionParticleEffect").gameObject;
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
