﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class EventHorizon : SpaceMonster {

    public override void Awake(){
        base.Awake();
        id = 1;
    }
    public override void CreateGoalState()
    {
        List<Goal> goal = new List<Goal>();

        ourGoals = goal;
    }
 
    
}
