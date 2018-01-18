using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class Goal : IComparable<Goal>, IComparable 
{
    public int CompareTo(Goal other)
    {
        return 1;
    }

    public int CompareTo(object other)
    {
        return 1;
    }

    public Goal(Condition desiredCondition, float priority)
    {
        goalWithPriority = new KeyValuePair<Condition, float>(desiredCondition, priority);
    }

    //keep this value between 0.0 and 1.0
    KeyValuePair<Condition, float> goalWithPriority;

    public KeyValuePair<Condition, float> GoalWithPriority
    {
        get
        {
            return goalWithPriority;
        }

    }
}
