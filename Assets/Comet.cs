﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : SpaceMonster
{
  private void Start()
    {
        maxStamina = 800f;
        stamina = maxStamina;
        damage = 0;
        speed = 10.0f;
        CreateGoalState();
    }

    public override void OnEnable(){

    }

    public override void OnDisable(){

    }

    public override void ReactToInterruption(GameObject interruptor){
        //TODO: The comet will react if the player comes within a certain range.
    }

    public override void ReturnToNormalFunction(){

    }



    public override List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
        worldData.Add(new Condition("threatInRange", false));
        worldData.Add(new Condition("foundMate", false));
        worldData.Add(new Condition("charge", false));
        worldData.Add(new Condition("reproduce", false));
        worldData.Add(new Condition("defend", false));
        return worldData;
    }

    public override void Awake()
    {
        base.Awake();
        id = 1;
    }
    public override void CreateGoalState()
    {
        List<Goal> goal = new List<Goal>();
        Condition con1 = new Condition("reproduce", true);
        goal.Add(new Goal(con1, 20));

        goal.Add(new Goal(new Condition("defend", true), 50));

        ourGoals = goal;
    }
	
	
	// Update is called once per frame
	void Update () {
		
	}
}