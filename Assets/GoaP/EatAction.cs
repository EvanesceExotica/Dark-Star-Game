using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatAction : GoapAction
{

//Event horizons - eat player souls, grow larger, deal more damage to dark star bigger they get

// Add "Digest" action

// digesting event horizons *can* be pushed into the dark star and not steal it away.

// Add enemy with bouncy collider

// Hibernate when they're too large


    private bool hungry;
    bool hasEaten;
    private float timeWithoutFood;
    private float maxSatiationAmount;
    private float currentSatiation;

    public EdibleType targetPrey;

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
        doReset();
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
        GameObject closest = enemySpawner.GetClosestOther(ourType, this.gameObject);
        if (closest == null)
        {
            target = GameStateHandler.player;
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
        //TODO: What if this fails -- the player pulls an enemy away or the player gets away
        ourPointEffector2D.enabled = true;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            foreach(GameObject go in ourThreatTrigger.enemiesInThreatTrigger){
                //for the player, this should take a chunk out of their health,
                go.GetComponent<Health>().BeingDevoured();
            }
            if(ourThreatTrigger.enemiesInThreatTrigger.Count == 0){
                //TODO: Find a way to deal if an object is disabled while in this (meaning successfully eaten or something)
                //if there are no longer any enemies in our trigger
                interrupted = true;
                yield break;
            }
            yield return new WaitForSeconds(2.0f);
        }
        //we should only get to this if an enemy stays inside the trigger for the entire duration (default 6 seconds)
        hasEaten = true;
        ourPointEffector2D.enabled = false;
    }

    public override bool perform(GameObject agent)
    {
        if (!performing) { 
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
