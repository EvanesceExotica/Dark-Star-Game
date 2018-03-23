using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : SpaceMonster {


    

    public override void OnEnable(){

    }

   
    public override void ReactToInterruption(GameObject interruptor)
    {//TODO: Add an if statement so it only reacts this way to other enemies
        Goal priority = new Goal(new Condition("stayAlive", true), 90);
        ChangeGoalPriority(priority);
    }

  

    public override void ReturnToNormalFunction()
    {

        //Debug.Log("Threat gone. Returning to normal");
        Goal priority = new Goal(new Condition("stayAlive", true), 20);
        ChangeGoalPriority(priority);
    }
    void ChangeGoalPriority(Goal changedGoal)
    {
        Goal goalToChange = ourGoals.Find(goal => goal.GoalWithPriority.Key.Name.Equals(changedGoal.GoalWithPriority.Key.Name));

        int index = ourGoals.IndexOf(goalToChange);
        if (index != -1)
        {
            ourGoals[index] = changedGoal;
        }
        // ourGoals.Remove(goalToChange);
        // ourGoals.Insert(index, changedGoal);

        //Debug.Log("<color=green> Goal Priority changed </color");

    }


public override List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
		worldData.Add(new Condition("foundSwitch", true));
		worldData.Add(new Condition("attachedToSwitch", true));
        worldData.Add(new Condition("gather", false));

        return worldData;
    }
    public override void CreateGoalState(){


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
