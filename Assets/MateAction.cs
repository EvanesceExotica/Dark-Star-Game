using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MateAction : GoapAction
{

    bool currentlyMating = false;
    float mateDuration = 10.0f;
    bool reproduced = false;

    public GameObject offspringPrefab;
    public GameObject reproductionParticleEffectGO;

    public List<ParticleSystem> reproductionParticleEffectList;
    public MateAction()
    {

        AddPrecondition(new Condition("foundMate", true));
        AddEffect(new Condition("reproduce", true));
        cost = 200f;
        canBeInterrupted = true;

    }
    public override void Awake()
    {
        base.Awake();
        reproductionParticleEffectGO = transform.Find("ReproductionParticleEffect").gameObject;
        reproductionParticleEffectList = reproductionParticleEffectGO.GetComponentsInChildren<ParticleSystem>().ToList();
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
        if (blueDwarfGOs.Count == 1 && blueDwarfGOs.Contains(this.gameObject))
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


        //take out the null check, as it /should/ have a target from the "Calling for Mate" action
        hasVectorTarget = false;
        // target = enemySpawner.GetClosestAlly(ourType, this.gameObject);

        target = ourGoapAgent.currentTarget;
        return true;
        // if (target != null)
        // {
        //     //Debug.Log(target.name);
        //     return true;
        // }
        // else
        // {

        //     return false;
        // }
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
        ParticleSystemPlayer.PlayChildParticleSystems(reproductionParticleEffectList);
        Vector2 birthPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f);
        BlueDwarf dataProvider = (BlueDwarf)ourGoapAgent.DataProvider; 
        enemySpawner.SpawnIndependent(dataProvider, birthPosition);
        // ParticleSystemPlayer.PlayChildParticleSystems(reproductionParticleEffectList);
        // BlueDwarf dataProvider = (BlueDwarf)ourGoapAgent.DataProvider; 
        // BlueDwarf newBlueDwarf = dataProvider.GetPooledInstance<BlueDwarf>();
        // newBlueDwarf.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f);
    }

   





    public override bool perform(GameObject agent)
    {
        if (!setPerformancePrereqs)
        {

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
