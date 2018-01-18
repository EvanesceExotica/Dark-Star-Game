using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatAction : GoapAction {

    private bool hungry;
    bool hasEaten; 
    private float timeWithoutFood;
    private float maxSatiationAmount;
    private float currentSatiation;

    public EdibleType targetPrey;

    private float startTime = 0;


    public EatAction()
    {
        addPrecondition("isHungry", true);
        addEffect("eat", true);
        cost = 100f;
    }

  
    EdibleType.FoodTypes ourEdibleType;

    public override void reset()
    {
       
    }

    public override bool isDone()
    {
        return hasEaten;
    }

    public override bool requiresInRange()
    {
        return true; //yes we need to be near food
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

       return FindClosestLowHealthEnemy(agent);       

    }

    public override bool perform(GameObject agent)
    {
        
        return true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
