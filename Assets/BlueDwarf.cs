using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDwarf : SpaceMonster {



    private void Start()
    {
        maxStamina = 800f;
        stamina = maxStamina;
        damage = 0;
        speed = 10.0f;
        CreateGoalState();


    }



    public override void CreateGoalState()
    {
        List<Goal> goal = new List<Goal>();
        Condition con1 = new Condition("reproduce", true);
        goal.Add(new Goal(con1, 50));

          goal.Add(new Goal(new Condition("stayAlive", true), 20));

        ourGoals = goal;
    }

    
    //public override Hashset<KeyValuePair<string, object>> changeGoalState(HashSet<KeyValuePair<string, object>> newGoal)
    //{
    //    HashSet<KeyValuePair<string, object>> modifiedGoal = new HashSet<KeyValuePair<string, object>>();
    //    modifiedGoal
    //}
   

}
