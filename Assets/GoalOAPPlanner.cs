using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Profiling;
public class GoalOAPPlanner
{
    //
    List<Condition> satisfiedConditions = new List<Condition>();
    List<Condition> unsatisfiedConditions = new List<Condition>();

    List<Condition> goalSatisfactionState = new List<Condition>();
    List<Condition> currentState = new List<Condition>();

    GameObject ourAgent;

    public Queue<GoapAction> Plan(Goal goal, List<GoapAction> availableActions_, List<Condition> worldState, GameObject agent)
    {
        Profiler.BeginSample("Starting planning by initializing variables");
        List<GoapAction> availableActions = availableActions_.ToList();
        ourAgent = agent;
        Node start = new Node(null, 0, worldState, null);
        bool foundOne = false;
        GoapAction satisfyingAction = null;
        List<Node> leaves = new List<Node>();

        ConvertGoalToCondition(goal, worldState);
        List<GoapAction> potentialActions = new List<GoapAction>();

        Profiler.EndSample();

        Profiler.BeginSample("Plan - sifting through actions to find one whose effects satisfy the goal");
        // //Debug.Log("Here are our available actions " + GoapAgent.prettyPrint(availableActions.ToArray()));
        foreach (GoapAction action in availableActions_)
        {
           //Get rid of the actions with no precondition here 
            if (!action.checkProceduralPrecondition(agent))
            {
                availableActions.Remove(action);

            }
        }
        foreach (GoapAction action in availableActions)
        {
            //Here we're checking if the effects of the last action satisfy the goals this agent is trying to reach, if they do, we add them to the potentialActions list;
            foundOne = CheckIfEffectsSatisfyGoal(action._effects) && action.checkProceduralPrecondition(agent);
            if (foundOne)
            {
                //  //Debug.Log("We found a potential action " + action.ToString());
                potentialActions.Add(action);
            }
        }

        //here we want to make sure were only getting one action of the lowest cost
        if (potentialActions.Count > 1)
        {
            GoapAction lowestCostAction = potentialActions.Last();
            foreach (GoapAction potential in potentialActions)
            {
                if (potential.cost < lowestCostAction.cost)
                {
                    lowestCostAction = potential;
                }

            }
            satisfyingAction = lowestCostAction;
        }
        else if (potentialActions.Count == 1)
        {

            satisfyingAction = potentialActions[0];
        }
        Profiler.EndSample();



        // MoveItemsFromUnsatisfiedToSatisfied(satisfyingAction);

        //  CheckIfCurrentStateMatchesGoalState();
        Profiler.BeginSample("Plan - If satisfying action isn't null, solve this satisfying action to have a prepped node");
        if (satisfyingAction != null)
        {
            // //Debug.Log("<color=yellow>We found a satisfying action!</color>" + satisfyingAction.ToString());
            FindMatchingWorldStateValue(worldState, satisfyingAction._preconditions);
            foreach (Condition con in satisfyingAction._preconditions)
            {
                if (!goalSatisfactionState.Contains(con))
                { //add the conditions needed to satisfy the goal (a precondition is added for every new potential action discovered)
                    goalSatisfactionState.Add(con);
                }
            }
            SolveConditions(satisfyingAction._effects);
            availableActions.Remove(satisfyingAction);

            //   //////Debug.Log("Here's the action that satisfied " + satisfyingAction + " " + goal.GoalWithPriority.Key.Name);

            leaves.Add(new Node(start, 0 + satisfyingAction.cost, satisfyingAction._preconditions, satisfyingAction));

            Profiler.EndSample();
            Profiler.BeginSample("Plan - Building graph");
            bool success = BuildGraph(leaves.Last(), leaves, availableActions, worldState);

            if (!success)
            {
                // //Debug.Log("WE FAILED");
                ClearCurrentStateAndGoalState();
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
            Profiler.EndSample();
            return queue;
        }
        else
        {
            // //Debug.Log("<color=cyan>Could not find an action to satisfy any of the goals</color>");
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
        Profiler.BeginSample("Check if current state matches goal state");
        //this method is to check if while you're going through th eplan, if the current state matches the goal state, which means you can stop planning
        bool allMatch = true;
        foreach (Condition con in goalSatisfactionState)
        {

            //Debug.Log("Goal satisfaciton state " + GoapAgent.prettyPrint(goalSatisfactionState));
        }

        foreach (Condition con in currentState)
        {
            //Debug.Log("Current State:  " + GoapAgent.prettyPrint(currentState));
        }
        for (int i = 0; i < goalSatisfactionState.Count; i++)
        {
            if (!(currentState[i].Name.Equals(goalSatisfactionState[i].Name) && currentState[i].Value.Equals(goalSatisfactionState[i].Value)))
            {
                allMatch = false;
                //Debug.Log("<color=orange>The current state did not match the goal state");
            }
        }




        //  bool allMatch = currentState.SequenceEqual(goalSatisfactionState);
        ///Debug.Log("Do they all match? " + allMatch);
        ///Debug.Log("Current State: " + GoapAgent.prettyPrint(currentState));
        ///Debug.Log("Goal State: " + GoapAgent.prettyPrint(goalSatisfactionState));

        Profiler.EndSample();
        return allMatch;
    }

    void ConvertGoalToCondition(Goal goal, List<Condition> worldState)
    {
        Profiler.BeginSample("Convert Goal to Condition");
        //This converts a goal (which has a priority attached) to a condition (which does not)
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

        Profiler.EndSample();
    }


    // void FindWorldState(List<Condition> conditions, List<Condition> worldState)
    // {
    //     foreach (Condition worldStateCondition in worldState)
    //     {
    //         foreach (Condition condition in conditions)
    //         {
    //             if (condition.Name.Equals(worldStateCondition.Name) && !currentState.Contains(worldStateCondition))
    //             {
    //                 currentState.Add(worldStateCondition);
    //             }
    //         }
    //     }
    // }


    bool CheckIfEffectsSatisfyGoal(List<Condition> effects)
    {
        Profiler.BeginSample("Check If Effects Satisfy Goal");
        //here we're trying to find a thing with effects that satisfy the goal 
        bool match = false;
        foreach (Condition con in effects)
        {

            //Debug.Log("Goal: " + goal.GoalWithPriority.Key.Name + ": " + goal.GoalWithPriority.Key.Value + " This Action's Effects : " + con.Name + ": " + con.Value);

            if (goalSatisfactionState.Last().Name == con.Name && goalSatisfactionState.Last().Value.Equals(con.Value))
            {
                //Debug.Log("SOMETHING MATCHED ====> Effects are " + con.Name + "," + con.Value + " and DO match " + goalSatisfactionState.Last().Name + "," + goalSatisfactionState.Last().Value);
                match = true;
            }
            else
            {
                // //Debug.Log("Does this goal satisfaction state's name " + goalSatisfactionState.Last().Name.ToString() + " equals this effect's name" + con.Name.ToString() + "?" );
                // //Debug.Log( goalSatisfactionState.Last().Name == con.Name);
                // //Debug.Log("Does this goal satisfaction state's name " + goalSatisfactionState.Last().Value.ToString() + " equals this effect's value" + con.Value.ToString() + "?" );
                // //Debug.Log( goalSatisfactionState.Last().Value.Equals( con.Value));

                // //Debug.Log("<color=red>Effects don't satisfy the goal</color>");
            }


        }
        if (match == false)
        {
            foreach (Condition con in effects)
            {
                //Debug.Log("NOTHING MATCHED =====> Effects are " + con.Name + "," + con.Value + " and don't match " + goalSatisfactionState.Last().Name + "," + goalSatisfactionState.Last().Value);
            }
        }
        Profiler.EndSample();
        return match;
    }



    bool TestIfEqual(GoapAction action, GoapAction parent)
    {
        Profiler.BeginSample("Test if parent preconditions are equal to child effects");
        bool match = false;
        //Condition[] potentialMatches = new Condition[parent._preconditions.Count + action._effects.Count]{ };
        List<GoapAction> potentialMatches = new List<GoapAction>();
        // Condition[] potentialMatches = Condition[action._effects.Count]{};
        foreach (Condition effect in action._effects)
        {

            foreach (Condition precondition in parent._preconditions)
            {
                bool busty = precondition.Name == effect.Name && precondition.Value.Equals(effect.Value);
                //   //Debug.Log("Does " + precondition.Name + " equal " + effect.Name + " and does " + precondition.Value + " equal " + effect.Value + "?" + " The answer is " + busty);
                if (precondition.Name == effect.Name && precondition.Value.Equals(effect.Value))
                {
                    potentialMatches.Add(action);

                }


            }
        }
        Profiler.EndSample();

        // // Profiler.BeginSample("TEST");
        // // bool test = potentialMatches[0].checkProceduralPrecondition(ourAgent);
        // // Profiler.EndSample();

        // Profiler.BeginSample("Checking procedural preconditions");
        // if (potentialMatches.Count > 1)
        // {
        //     Profiler.BeginSample("Is this the issue?");
        //     foreach (GoapAction potentialAction in potentialMatches)
        //     {
        //         Profiler.BeginSample("Or is it the precondition check itself");
        //         if (potentialAction.checkProceduralPrecondition(ourAgent))
        //         {
        //             match = true;
        //         }
        //         Profiler.EndSample();

        //     }
        //     Profiler.EndSample();


        // }
        // else if (potentialMatches.Count == 1)
        // {
        //     Profiler.BeginSample("Single procedural condition check");
        //     if (potentialMatches[0].checkProceduralPrecondition(ourAgent))
        //     {
        //         match = true;

        //     }
        //     Profiler.EndSample();

        // }
        // Profiler.EndSample();
        return match;
    }

    //
    void SolveConditions(List<Condition> satisfiedConditions)
    {
        Profiler.BeginSample("Solving Conditions in Current State");
        //due to the effects of each action, this is changing the unsatisfied conditions out of the current state and making them satisfied
        List<Condition> currState = new List<Condition>(currentState);

        foreach (Condition condition in currState)
        {
            foreach (Condition satisfiedCondition in satisfiedConditions)
            {

                if (condition.Name.Equals(satisfiedCondition.Name))
                {
                    int index = currentState.IndexOf(condition);
                    if (index != -1)
                    {
                        currentState[index] = satisfiedCondition;

                    }
                    // currentState.Remove(condition);
                    // currentState.Insert(index, satisfiedCondition);
                }
            }
        }
        Profiler.EndSample();
    }

    void FindMatchingWorldStateValue(List<Condition> worldState, List<Condition> preconditions)
    {

        Profiler.BeginSample("Finding matching world state value in preconditions");
        //this method is finding which preconditions match the current world state

        foreach (Condition worldStateCondition in worldState)
        {
            foreach (Condition precondition in preconditions)
            {
                if (precondition.Name.Equals(worldStateCondition.Name) && !currentState.Contains(worldStateCondition))
                {
                    //if a precondition is found that matches the world state's name (Not value), and the current state doesn't already have it 
                    //    //////Debug.Log("PRECONDITION = WORLDSTATE" + precondition.Name + " and " + worldStateCondition.Name);
                    currentState.Add(worldStateCondition);
                }
                else
                {

                }
            }
        }
        Profiler.EndSample();
    }


    bool BuildGraph(Node parent, List<Node> leaves, List<GoapAction> availableActions, List<Condition> worldState)
    {
        Profiler.BeginSample("Building graph");
        bool worldAndCurrentMatch = false;
        bool foundMatch = false;
        List<GoapAction> potentialMatches = new List<GoapAction>();
        GoapAction satisfyingAction = null;

        foreach (GoapAction action in availableActions)
        {
            Profiler.BeginSample("Testing if equal might be the culprit");
            //    //////Debug.Log("The parents'" + newParent.action.ToString() + " preconditions " + GoapAgent.prettyPrint(newParent.action._preconditions) + "This action: " + action.ToString() + "  effects " + GoapAgent.prettyPrint(action._effects));
            if (TestIfEqual(action, parent.action))
            {
                potentialMatches.Add(action);
            }
            Profiler.EndSample();
        }

        if (potentialMatches.Count > 1)
        {
            GoapAction lowestCostAction = potentialMatches.Last();
            foreach (GoapAction potential in potentialMatches)
            {
                if (potential.cost < lowestCostAction.cost)
                {
                    lowestCostAction = potential;
                }

            }
            satisfyingAction = lowestCostAction;
        }
        else if (potentialMatches.Count == 1)
        {
            satisfyingAction = potentialMatches[0];
        }



        if (satisfyingAction != null)
        {
            foundMatch = true;
        }
        ///Debug.Log("Satisfying action " + satisfyingAction);
        if (foundMatch)
        {
            //////Debug.Log(foundMatch);
        }
        if (foundMatch == false)
        {
            ///Debug.Log("Found match is false");
        }
        worldAndCurrentMatch = CheckIfCurrentStateMatchesGoalState();

        Node next = null;

        if (foundMatch)
        {
            //////Debug.Log("Satisfying action was " + satisfyingAction);
            next = new Node(parent, parent.runningCost + satisfyingAction.cost, worldState, satisfyingAction);
            // //////Debug.Log("<color=green>Node next = </color>" + next.parent.action.ToString() + next.action.ToString());
            leaves.Add(next);

            if (!worldAndCurrentMatch)
            {

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
                    ///Debug.Log("They're finally equal!");
                    worldAndCurrentMatch = true;
                }
            }

        }






        Profiler.EndSample();

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

