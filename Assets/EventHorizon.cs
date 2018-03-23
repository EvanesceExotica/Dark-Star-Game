using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventHorizon : SpaceMonster
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

        SoulBehavior.SoulSpawned += this.ReactToInterruption;
        EnemySpawner.NewEnemySpawned+= this.ReactToInterruption;
        //
    }

    public override void OnDisable(){

    }

    public override void ReactToInterruption(GameObject interruptor){
        //TODO: The event horizon needs to find a new target if a new enemy is spawned
        
    }

    public override void ReturnToNormalFunction(){

    }



    public override List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
        worldData.Add(new Condition("eat", false));
        worldData.Add(new Condition("growLarger", false));
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
        Condition con1 = new Condition("growLarger", true);
        goal.Add(new Goal(con1, 50));


        ourGoals = goal;
    }


}
