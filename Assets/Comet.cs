using System.Collections;
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

     public override void ReactToInterruption(GameObject interruptor)
    {
        base.ReactToInterruption(interruptor);
        Goal priority = new Goal(new Condition("defend", true), 90);
        ChangeGoalPriority(priority);
    }


    public override void ReturnToNormalFunction()
    {
        base.ReturnToNormalFunction();
        //Debug.Log("Threat gone. Returning to normal");
        Goal priority = new Goal(new Condition("defend", true), 10);
        ChangeGoalPriority(priority);
    }

   
    public override List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
       // worldData.Add(new Condition("threatInRange", false));
        worldData.AddRange(base.GetWorldState());
        worldData.Add(new Condition("trail", false));
        worldData.Add(new Condition("hibernate", false));
        worldData.Add(new Condition("defend", false));
        return worldData;
    }

    public override void Awake()
    {
        base.Awake();
        id = 2;
    }
    public override void CreateGoalState()
    {
        List<Goal> goal = new List<Goal>();
        //the comet travels in spirals around the star, leaving temporary trails that are destroyed at the end (maybe use the "waypoint" system?)
        //the player can ride the trails?
        goal.Add(new Goal(new Condition("hibernate", true), 20));
        goal.Add(new Goal(new Condition("defend", true), 10));

        ourGoals = goal;
    }
	
	
	// Update is called once per frame
	
}
