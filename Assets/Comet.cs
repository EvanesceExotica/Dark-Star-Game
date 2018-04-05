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

    public override void ReactToInterruption(GameObject interruptor){
        //TODO: The comet will react if the player comes within a certain range.
    }

    public override void ReturnToNormalFunction(){

    }

   
    public override List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
        worldData.Add(new Condition("threatInRange", false));
        worldData.Add(new Condition("trail", false));
        worldData.Add(new Condition("reachedSwitch", true));
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
        //the comet travels in spirals around the star, leaving temporary trails that are destroyed at the end (maybe use the "waypoint" system?)
        //the player can ride the trails?
        Condition con1 = new Condition("trail", true);
        goal.Add(new Goal(con1, 20));

        goal.Add(new Goal(new Condition("defend", true), 50));

        ourGoals = goal;
    }
	
	
	// Update is called once per frame
	
}
