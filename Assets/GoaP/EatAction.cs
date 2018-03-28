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

     void CheckWhatsIncapacitatingEnemy(GameObject source){
         ///if something incapacitates our target before we reach it
         if(source != this.gameObject && !isInRange()){
             interrupted = true;
         }
    }
    public override void ImportantEventTriggered(GameObject interruptor)
    {
        //TODO: Make SURE this can be handled before the enemy reaches its target
        //this enemy should be interrupted when it's target is being eaten by another enemy before it reaches it 

        target.GetComponent<UniversalMovement>().SomethingImpededOurMovement += this.CheckWhatsIncapacitatingEnemy;
        if (!isInRange())
        {
            interrupted = true;
        }
    }

    public override void Awake()
    {
        base.Awake();
        duration = 6.3f;
        eatingParticleSystemList = eatingParticleSystemGO.GetComponentsInChildren<ParticleSystem>().ToList();
    }
    bool devouredEnemy;
    //
    public float duration;



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
            return false;

            // //TODO: MAke a seprate "eat player" action that is a last resort 
            // GameObject player = GameStateHandler.player;
            // if (player.GetComponent<PlayerMovement>().incapacitationSources.Count > 0)
            // {
            //     //if the player is incapacitated AND there are no other enemies 
            //     return false;
            // }
            // else
            // {

            //     target = GameStateHandler.player;
            // }

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
        ourPointEffector2D.enabled = true;
        float startTime = Time.time;
        ParticleSystemPlayer.PlayChildParticleSystems(eatingParticleSystemList);
        List<GameObject> potentialTargets = null;
        ourThreatTrigger.applyingDamageThroughTrigger = true;
        ourThreatTrigger.applyingIncapacitationThroughTrigger = true;


        while (Time.time < startTime + duration)
        {
            potentialTargets = ourThreatTrigger.enemiesInThreatTrigger.ToList();
            if (ourThreatTrigger.devouredEnemy == true)
            {
                //we successfully devoured an enemy
                hasEaten = true;
                ourThreatTrigger.devouredEnemy = false;
                break;
            }
            if (potentialTargets.Count == 0 && !hasEaten)
            {
                //if there are no longer any enemies in our trigger and it's not because we ate them (they were killed by something else or pulled away)
                interrupted = true;
                break;
            }
            foreach (GameObject go in potentialTargets)
            {
                //for the player, this should take one of their souls
                Health health = go.GetComponent<Health>();
                health.BeingDevoured(this.gameObject);
                if (!health.persistentDamageSources.Contains(this.gameObject))
                {
                    //if this isn't already taking damage from the creature
                    health.AddDamageSource(this.gameObject);
                }
                UniversalMovement preyMovement = go.GetComponent<UniversalMovement>();
                if (!preyMovement.incapacitationSources.Contains(this.gameObject))
                {
                    //if this isn't already being incapacitated by this creature
                    go.GetComponent<UniversalMovement>().AddIncapacitationSource(this.gameObject);
                }
            }
            yield return new WaitForSeconds(2.0f);
        }
        if (potentialTargets != null && potentialTargets.Count > 0)
        {
            foreach (GameObject go in potentialTargets)
            {
                go.GetComponent<UniversalMovement>().RemoveIncapacitationSource(this.gameObject);
                go.GetComponent<Health>().RemovePersistentDamageSource(this.gameObject);
            }
        }

        ParticleSystemPlayer.StopChildParticleSystems(eatingParticleSystemList);
        ourPointEffector2D.enabled = false;
        ourThreatTrigger.applyingDamageThroughTrigger = false;
        ourThreatTrigger.applyingIncapacitationThroughTrigger = false;
        if (devouredEnemy == true)
        {
            hasEaten = true;
            devouredEnemy = false;
        }
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
