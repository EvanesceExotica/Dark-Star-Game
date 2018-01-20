using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
public class GoalOAPPlanner 
{

    List<Condition> satisfiedConditions = new List<Condition>();
    List<Condition> unsatisfiedConditions = new List<Condition>();

    List<Condition> goalSatisfactionState = new List<Condition>();
    List<Condition> currentState = new List<Condition>();

    GameObject ourAgent;
   
    public Queue<GoapAction> Plan(Goal goal, List<GoapAction> availableActions, List<Condition> worldState, GameObject agent)
    {
        ourAgent = agent;
        Node start = new Node(null, 0, worldState, null);
        bool foundOne = false;
        GoapAction satisfyingAction = null;
        List<Node> leaves = new List<Node>();

        ConvertGoalToCondition(goal, worldState);
        List<GoapAction> potentialActions = new List<GoapAction>();

        foreach (GoapAction action in availableActions)
        {
            foundOne = CheckIfEffectsSatisfyGoal(action._effects) && action.checkProceduralPrecondition(agent);
            if (foundOne) {
               Debug.Log("We found a potential action " + action.ToString()) ;
                potentialActions.Add(action);

                //here we want to make sure were only getting one action of the lowest cost
                if (potentialActions.Count > 0)
                {//your probelm is here!!!! TODO: FIX THIS @@@
                    Debug.Log("potential action count equals " + potentialActions.Count);
                    foreach (GoapAction potential in potentialActions)
                    {
                        if (action.Equals(potential) && potentialActions.Count != 1)
                        {
                            //understand the point of this continue pls 
                            continue;
                        }
                        else{
                            Debug.Log("Does " + action.ToString() + " equal " + potential.ToString());
                            Debug.Log("<color=red>Action does not equal potential action</color>");
                        }
                        if (action.cost < potential.cost)
                        {
                            satisfyingAction = action;
                        }
                        else{
                            Debug.Log("Satisfying action too expensive");
                        }
                    }
                }
              
            }

           
        }
        // MoveItemsFromUnsatisfiedToSatisfied(satisfyingAction);

      //  CheckIfCurrentStateMatchesGoalState();
       if(satisfyingAction != null)
        {
            Debug.Log("<color=yellow>We found a satisfying action!</color>");
            FindMatchingWorldStateValue(worldState, satisfyingAction._preconditions);
            foreach (Condition con in satisfyingAction._preconditions)
            {
                if (!goalSatisfactionState.Contains(con))
                {
                    goalSatisfactionState.Add(con);
                }
            }
            SolveConditions(satisfyingAction._effects);
            availableActions.Remove(satisfyingAction);

            //   ////Debug.Log("Here's the action that satisfied " + satisfyingAction + " " + goal.GoalWithPriority.Key.Name);

            leaves.Add(new Node(start, 0 + satisfyingAction.cost, satisfyingAction._preconditions, satisfyingAction));

            bool success = BuildGraph(leaves.Last(), leaves, availableActions, worldState);

            if (!success)
            {
                Debug.Log("WE FAILED");
                return null;
            }

            List<GoapAction> result = new List<GoapAction>();

            Node n = leaves.Last();
            while (n != null)
            {
                if (n.action != null)
                {
                    //result.Insert(0, n.action);
                    result.Add(n.action);
                }
                n = n.parent;
            }
            //we now have this action list in correct order

            Queue<GoapAction> queue = new Queue<GoapAction>();
            foreach (GoapAction a in result)
            {
                queue.Enqueue(a);
            }
            //hooray we have a plan!
            ClearCurrentStateAndGoalState();
            return queue;
        }
        else
        {
            Debug.Log("<color=cyan>Could not find an action to satisfy any of the goals</color>");
            ClearCurrentStateAndGoalState();
            return null;
        }


       

    
    }

    void ClearCurrentStateAndGoalState()
    {
        currentState.Clear();
        goalSatisfactionState.Clear();
    }

    bool CheckIfCurrentStateMatchesGoalState()
    {
        bool allMatch = true;
        //Condition[] current = new Condition[currentState.Count];
        //Condition[] goal = new Condition[goalSatisfactionState.Count];
        //for(int i = 0; i < currentState.Count; i++)
        //{
        //    current[i] = new Condition(currentState[i].Name, currentState[i].Value);
        //}
        //for(int i = 0; i < goalSatisfactionState.Count; i++)
        //{
        //    goal[i] = new Condition(currentState[i].Name, currentState[i].Value);
        //}

        for (int i = 0; i < goalSatisfactionState.Count; i++)
        {
            if (!(currentState[i].Name.Equals(goalSatisfactionState[i].Name) && currentState[i].Value.Equals(goalSatisfactionState[i].Value))){
                ////Debug.Log(currentState[i].Name + "," + currentState[i].Value + " does not equal " + goalSatisfactionState[i].Name + ", " + goalSatisfactionState[i].Value);
                allMatch = false;
                Debug.Log("<color=orange>The current state did not match the goal state");
            }
        }




      //  bool allMatch = currentState.SequenceEqual(goalSatisfactionState);
        ////Debug.Log("Do they all match? " + allMatch);
        ////Debug.Log("Current State: " + GoapAgent.prettyPrint(currentState));
        ////Debug.Log("Goal State: " + GoapAgent.prettyPrint(goalSatisfactionState));
 
        return allMatch;
    }

    void ConvertGoalToCondition(Goal goal, List<Condition> worldState)
    {
        Condition convertedGoal = new Condition(goal.GoalWithPriority.Key.Name, goal.GoalWithPriority.Key.Value);

        if (!goalSatisfactionState.Contains(convertedGoal))
        {
            goalSatisfactionState.Add(convertedGoal);

        }
        foreach (Condition condition in worldState)
        {
            if (convertedGoal.Name.Equals(condition.Name) && !currentState.Contains(condition))
            {
                currentState.Add(condition);
            }
        }
        

    }

    
    void FindWorldState(List<Condition> conditions, List<Condition> worldState)
    {
        foreach (Condition worldStateCondition in worldState)
        {
            foreach (Condition condition in conditions)
            {
                if (condition.Name.Equals(worldStateCondition.Name) && !currentState.Contains(worldStateCondition))
                {
                    currentState.Add(worldStateCondition);
                }
            }
        }
    }


    bool CheckIfEffectsSatisfyGoal(List<Condition> effects)
    {
        //here we're trying to find a thing with effects that satisfy the goal 
        bool match = false;
        foreach(Condition con in effects)
        {

            //Debug.Log("Goal: " + goal.GoalWithPriority.Key.Name + ": " + goal.GoalWithPriority.Key.Value + " This Action's Effects : " + con.Name + ": " + con.Value);

            if (goalSatisfactionState.Last().Name == con.Name && goalSatisfactionState.Last().Value.Equals(con.Value)) 
            {
                Debug.Log("SOMETHING MATCHED ====> Effects are " + con.Name + "," + con.Value + " and DO match " + goalSatisfactionState.Last().Name + "," + goalSatisfactionState.Last().Value);
                match = true;
            }
            else{
                // Debug.Log("Does this goal satisfaction state's name " + goalSatisfactionState.Last().Name.ToString() + " equals this effect's name" + con.Name.ToString() + "?" );
                // Debug.Log( goalSatisfactionState.Last().Name == con.Name);
                // Debug.Log("Does this goal satisfaction state's name " + goalSatisfactionState.Last().Value.ToString() + " equals this effect's value" + con.Value.ToString() + "?" );
                // Debug.Log( goalSatisfactionState.Last().Value.Equals( con.Value));

                // Debug.Log("<color=red>Effects don't satisfy the goal</color>");
            }
          
                           
        }
        if(match == false){
            foreach(Condition con in effects){
                Debug.Log("NOTHING MATCHED =====> Effects are " + con.Name + "," + con.Value + " and don't match " + goalSatisfactionState.Last().Name + "," + goalSatisfactionState.Last().Value);
            }
        }
        return match;
    }

   

    bool TestIfEqual(GoapAction action, GoapAction parent)
    {
        if(parent._preconditions.Count == 0)
        {
            ///Debug.Log("Parent " + parent.ToString() + " and Child " + action.ToString() + " NO PRECONDITIONS");
        }
        bool match = false;
        foreach(Condition effect in action._effects)
        {
            
            foreach(Condition precondition in parent._preconditions)
            {
                bool busty = precondition.Name == effect.Name && precondition.Value.Equals(effect.Value);
             //   Debug.Log("Does " + precondition.Name + " equal " + effect.Name + " and does " + precondition.Value + " equal " + effect.Value + "?" + " The answer is " + busty);
                if (precondition.Name == effect.Name && precondition.Value.Equals(effect.Value))
                {
                    ////Debug.Log("Here are the matching ones: " + "Action : " + action.ToString() + ", " + "Parent Action" + parent.ToString() + precondition.Name + ", " + precondition.Value);
                    if (action.checkProceduralPrecondition(ourAgent))
                    {
                        match = true;
                    }
                    else
                    {
                        Debug.Log("ProceduralPrecondition of " + action.ToString() + " failed");
                    }
                
               }
               else{
               }
               
            }
        }
        if(match == false){
            Debug.Log("No parent precondition met the action's effects");
        }
        return match;
    }

  
    void SolveConditions(List<Condition> satisfiedConditions)
    {
        //this is pulling the unsatisfied conditions out of the current state and making them satisfied
        List<Condition> currState = new List<Condition>(currentState);

        foreach (Condition condition in currState)
        {
            foreach (Condition satisfiedCondition in satisfiedConditions) {

                if (condition.Name.Equals(satisfiedCondition.Name))
                {
                    int index = currentState.IndexOf(condition);
                    currentState.Remove(condition);
                    currentState.Insert(index, satisfiedCondition);
                }
            }
        }
    }

    void FindMatchingWorldStateValue(List<Condition> worldState, List<Condition> preconditions)
    {
       
        
        foreach(Condition worldStateCondition in worldState)
        {
            foreach(Condition precondition in preconditions)
            {
                if (precondition.Name.Equals(worldStateCondition.Name) && !currentState.Contains(worldStateCondition))
                {
                //    ////Debug.Log("PRECONDITION = WORLDSTATE" + precondition.Name + " and " + worldStateCondition.Name);
                    currentState.Add(worldStateCondition);
                }
                else{

                }
            }
        }
    }

   
    bool BuildGraph(Node parent, List<Node> leaves, List<GoapAction> availableActions, List<Condition> worldState)
    {
        bool worldAndCurrentMatch = false;
        bool foundMatch = false;
        List<GoapAction> potentialMatches = new List<GoapAction>();
        GoapAction satisfyingAction = null;

        foreach (GoapAction action in availableActions)
        {

            //    ////Debug.Log("The parents'" + newParent.action.ToString() + " preconditions " + GoapAgent.prettyPrint(newParent.action._preconditions) + "This action: " + action.ToString() + "  effects " + GoapAgent.prettyPrint(action._effects));
          
          //  foundMatch = TestIfEqual(action, parent.action);
            if (TestIfEqual(action, parent.action))
            {
                potentialMatches.Add(action);
                if (potentialMatches.Count > 1)
                {
                    foreach (GoapAction potential in potentialMatches)
                    {
                        if (action.Equals(potential))
                        {
                            continue;
                        }
                        if (action.cost < potential.cost)
                        {
                            satisfyingAction = action;
                        }
                    }
                }
                else
                {
                    satisfyingAction = action;
                }

                //Get world value of this state here. 
                //  currentState.Add(action.precondit)
               
                
              //  ////Debug.Log("HERE's OUR NEW ACTION  " + newParent.action);
            }
           
           
        }
        if(satisfyingAction != null)
        {
            foundMatch = true;
        }
        ////Debug.Log("Satisfying action " + satisfyingAction);
        if (foundMatch) {
            ////Debug.Log(foundMatch);
        }
        if(foundMatch == false)
        {
            ////Debug.Log("Found match is false");
        }
        worldAndCurrentMatch = CheckIfCurrentStateMatchesGoalState();

        Node next = null;
       
        if (foundMatch)
        {
            ////Debug.Log("Satisfying action was " + satisfyingAction);
            next = new Node(parent, parent.runningCost + satisfyingAction.cost, worldState, satisfyingAction);
            // ////Debug.Log("<color=green>Node next = </color>" + next.parent.action.ToString() + next.action.ToString());
            leaves.Add(next);

            if (!worldAndCurrentMatch) {
            
                FindMatchingWorldStateValue(worldState, satisfyingAction._preconditions);

                foreach (Condition con in satisfyingAction._preconditions)
                {
                    if (!goalSatisfactionState.Contains(con))
                    {
                        goalSatisfactionState.Add(con);
                    }
                }

                SolveConditions(satisfyingAction._effects);

                availableActions.Remove(satisfyingAction);
                //might want to make the world state variable just in case

                bool match = BuildGraph(next, leaves, availableActions, worldState);
                    if (match)
                    {
                        ////Debug.Log("They're finally equal!");
                        worldAndCurrentMatch = true;
                    }
            }

        }
       

       




        return worldAndCurrentMatch;
       
    }
    

   
    

    private class Node
    {
        public Node parent;
        public float runningCost;
        List<Condition> state;
        //public Dictionary<string, object> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, List<Condition> state, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
}

