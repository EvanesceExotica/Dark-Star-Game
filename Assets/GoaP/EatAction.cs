using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class EatAction : GoapAction
{

    //Event horizons - eat player souls, grow larger, deal more damage to dark star bigger they get

    // Add "Digest" action

    // digesting event horizons *can* be pushed into the dark star and not steal it away.

    // Add enemy with bouncy collider

    // Hibernate when they're too large

    EatenBurst eatenBurstPrefab;
    private bool hungry;
    bool hasEaten;
     private float timeWithoutFood;
    private float maxSatiationAmount;
    private float currentSatiation;

    public EdibleType targetPrey;

    public GameObject eatingParticleSystemGO;

    List<ParticleSystem> eatingParticleSystemList = new List<ParticleSystem>();

    public override void Awake()
    {
        base.Awake();
        duration = 6.3f;
        eatingParticleSystemList = eatingParticleSystemGO.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    //
    public float duration;

    // public void EnemiesBeingDevoured(){
    //     if(DevouringEnemies != null){
    //         DevouringEnemies();
    //     }
    // }

    public EatAction()
    {
        AddEffect(new Condition("eat", true));
        cost = 50;
    }


    EdibleType.FoodTypes ourEdibleType;

    public override void reset()
    {
        target = null;
        hasEaten = false;

    }

    public override bool isDone()
    {
        return hasEaten;
    }

    public override bool requiresInRange()
    {
        return true; //yes we need to be near food
    }

    bool FindClosestEnemy()
    {
        GameObject closest = enemySpawner.GetClosestOther(ourType, this.gameObject, EnemySpawner.FilterSpecific.incapacitated);

        if (closest == null)
        {
           GameObject player = GameStateHandler.player;
            if(player.GetComponent<PlayerMovement>().incapacitationSources.Count > 0){
                //if the player is incapacitated AND there are no other enemies 
                return false;
            }
            else{

                target = GameStateHandler.player;
            }
        
        }
        else
        {
            target = closest;
        }
        return true;
    }


    bool FindClosestLowHealthEnemy(GameObject agent)
    {

        EdibleType[] potentialEdibleObjects = GameObject.FindObjectsOfType<EdibleType>();
        List<EdibleType> edibleObjectsBelow50PercentHP = new List<EdibleType>();

        EdibleType closest = null;
        float closestDistance = 0;

        foreach (EdibleType potentialEdible in potentialEdibleObjects)
        {
            //find enemies below half health
            Health health = potentialEdible.gameObject.GetComponent<Health>();
            if (health.currentHealth <= health.maxHealth * 0.5f && potentialEdible.goodFoods.Contains(ourEdibleType))
            {
                edibleObjectsBelow50PercentHP.Add(potentialEdible);
            }
        }
        foreach (EdibleType edible in edibleObjectsBelow50PercentHP)
        {
            if (closest == null)
            {
                closest = edible;
                closestDistance = Vector2.Distance(edible.gameObject.transform.position, agent.transform.position);
            }
            else
            {
                float distance = Vector2.Distance(edible.gameObject.transform.position, agent.transform.position);
                if (distance < closestDistance)
                {
                    closest = edible;
                    closestDistance = distance;
                }
            }

        }

        if (closest == null)
            return false;
        targetPrey = closest;
        target = targetPrey.gameObject;

        return closest != null;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

        return FindClosestEnemy();

    }

    public IEnumerator Devour()
    {
        //TODO: The point effector ALSO needs to not absorb other SpaceMonster enemies
        //TODO: What if this fails -- the player pulls an enemy away or the player gets away -- find a way to deal
        //TODO: With failure AND remove the incapacitation sources
        ourPointEffector2D.enabled = true;
        float startTime = Time.time;
        ParticleSystemPlayer.PlayChildParticleSystems(eatingParticleSystemList);

        while (Time.time < startTime + duration)
        {
            foreach (GameObject go in ourThreatTrigger.enemiesInThreatTrigger)
            {
                //WARNING: Collection modified during enumeration, fix
                //TODO: This needs to be fixed as the collection ends up being modified during inemeration, which is a nono
                //for the player, this should take a chunk out of their health,
                go.GetComponent<Health>().BeingDevoured(this.gameObject);
                UniversalMovement preyMovement = go.GetComponent<UniversalMovement>();
                if (!preyMovement.incapacitationSources.Contains(this.gameObject))
                {
                    //if this isn't already being incapacitated by this creature
                    go.GetComponent<UniversalMovement>().AddIncapacitationSource(this.gameObject);
                }
            }
            if (ourThreatTrigger.enemiesInThreatTrigger.Count == 0)
            {
                //TODO: Find a way to deal if an object is disabled while in this (meaning successfully eaten or something)
                //if there are no longer any enemies in our trigger
                interrupted = true;
                yield break;
            }
            yield return new WaitForSeconds(2.0f);
        }
        //we should only get to this if an enemy stays inside the trigger for the entire duration (default 6 seconds)
        ParticleSystemPlayer.StopChildParticleSystems(eatingParticleSystemList);
        hasEaten = true;
        ourPointEffector2D.enabled = false;
    }

    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            StartCoroutine(Devour());
        }
        performing = true;
        if (interrupted)
        {
            //TODO: MAke it so a new enemy spawning interrupts IF it's chasing the player
            performing = false;
        }
        return performing;
    }

}
