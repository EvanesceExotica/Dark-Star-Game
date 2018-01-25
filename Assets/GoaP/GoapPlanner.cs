using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoapPlanner
{

    public static string prettyPrint(HashSet<GoapAction> actions)
    {
        String s = "";
        foreach (GoapAction a in actions)
        {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }
//i//
    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */



    public Queue<GoapAction> Plan(GameObject agent, HashSet<GoapAction> availableActions, List<Condition> worldState, List<Goal> goal)
    {
        // reset the actions so we can start fresh with them

        foreach (GoapAction a in availableActions)
        {
            a.doReset();
        }

        // check what actions can run using their checkProceduralPrecondition

        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();
        foreach (GoapAction a in availableActions)
        {
            if (a.checkProceduralPrecondition(agent))
            {
                usableActions.Add(a);

            }
            else
            {
                Debug.Log(a.ToString() + " 's precondition check failed");
                GoapAgent.prettyPrint(a);
            }
        }


        // we now have all actions that can run, stored in usableActions
        // build up the tree and record the leaf nodes that provide a solution to the goal.

        List<Node_> leaves = new List<Node_>();

        //build graph
        Node_ start = new Node_(null, 0, worldState, null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            //Debug.log("No Plan");
            return null;
        }

        Node_ cheapest = null;
        foreach (Node_ leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.runningCost < cheapest.runningCost)
                    cheapest = leaf;
            }
        }

        //get its node and work back through the parents
        List<GoapAction> result = new List<GoapAction>();
        Node_ n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
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
        return queue;

    }


    public Queue<GoapAction> plan(GameObject agent,
        HashSet<GoapAction> availableActions,
        HashSet<KeyValuePair<string, object>> worldState,
        HashSet<KeyValuePair<string, object>> goal)
    {
        // reset the actions so we can start fresh with them

        foreach (GoapAction a in availableActions)
        {
            a.doReset();
        }

        // check what actions can run using their checkProceduralPrecondition

        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();
        foreach (GoapAction a in availableActions)
        {
            if (a.checkProceduralPrecondition(agent))
            {
                usableActions.Add(a);

            }
            else
            {
                GoapAgent.prettyPrint(a);
                //Debug.Log(a.ToString() + "'s procedrual precondition was false ");
            }
        }

        //Debug.log(prettyPrint(usableActions));

        // we now have all actions that can run, stored in usableActions
        // build up the tree and record the leaf nodes that provide a solution to the goal.

        List<Node> leaves = new List<Node>();

        //build graph
        Node start = new Node(null, 0, worldState, null);

        bool success = buildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            //Debug.log("No Plan");
            return null;
        }

        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.runningCost < cheapest.runningCost)
                    cheapest = leaf;
            }
        }

        //get its node and work back through the parents
        List<GoapAction> result = new List<GoapAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
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
        return queue;

    }

    /**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest cost will be the best action
	 * sequence.
	 */

    private bool BuildGraph(Node_ parent, List<Node_> leaves, HashSet<GoapAction> usableActions, List<Goal> goals)
    {
        bool foundOne = false;
        foreach (GoapAction action in usableActions)
        {

            //if the parent's state (world state) has the conditions for this action's preconditions, we can use it here
            if (PreconditionsInWorldState(action, action._preconditions, parent.state))
            { //parent.state is the world's state in this instance

                List<Condition> currentState = PopulateState(parent.state, action._effects);


                Node_ node = new Node_(parent, parent.runningCost + action.cost, currentState, action);


                if (GoalInState(goals, currentState))
                {
                    //if the goal aligns with the state -- 
                    //I think this is the constantly changing thing to see if each step satisfies the last 
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {


                    HashSet<GoapAction> subset = actionSubset(usableActions, action); //taking all actions and the current action in this iteration . The current action needs to be removed because it didn't match the goal

                    bool found = BuildGraph(node, leaves, subset, goals); //this time we're redoing this whole thing while taking the useless action out of the equation, but why? What makes this action useless? 
                                                                          //(The goal isn't found in this one?)
                    if (found)
                        foundOne = true;
                }


            }
            else
            {
                Debug.Log(action.ToString() + " Preconditions aren't in the world state");
                GoapAgent.prettyPrint(action);
            }

        }

        return foundOne;
    }

    private bool buildGraph(Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
    {
        //ALRIGHT, SO YOUR ISSUE HERE IS THAT YOU MIGHT NEED A WAY TO CHANGE GOALS WHEN THE THREAT IS IN RANGE. 
        //YOUR PRECONDITION -- THREAT IN RANGE - WAS NOT IN THE WORLD STATE, 
        //AND THEREFORE THE STATE "DEFENSIVE PULSE ACTION" WHICH HAD THAT PRECONDITION WAS KEEPING IT FROM BEING CONSIDERED.
        //THEREFORE THE TWO GOALS TOGETHER, STAY ALIVE AND CHARGE, COULD NOT BE COMPLETED. THE SOLUTION HERE MAY BE EITHER
        //TO HAVE ONE GOAL AND CHANGE IT WHEN THE WORLD STATE IS CHANGED WHEN A THREAT ENTERS THE AREA OR
        //MAKE IT SO THAT THE PRECONDITION FITS?
        //IN THE TEST THERE IS ONLY ONE GOAL FOR EACH ENEMY , IN THE OTHER THERE ARE NO PRECONDITIONS? 

        bool foundOne = false;

        foreach (GoapAction action in usableActions)
        {

            //if the parent's state (world state) has the conditions for this action's preconditions, we can use it here
            if (inState(action, action.Preconditions, parent.state))
            { //parent.state is the world's state in this instance

                // //Debug.log("Is this happening?");
                //here you're taking the world's current state and putting it together with the action's effects?
                HashSet<KeyValuePair<string, object>> currentState = populateState(parent.state, action.Effects);

                foreach (KeyValuePair<string, object> obj in currentState)
                {
                    //Debug.log(obj.Key + "," + " " + obj.Value);
                    //^I don't get the point of this. It just appears to be adding the effects and the world state together. why?
                }

                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);


                if (goalInState(goal, currentState))
                {
                    //Debug.log("SOLUTION FOUND: " + "Current State " + GoapAgent.prettyPrint(currentState) + "\n" + "Goal : " + GoapAgent.prettyPrint(goal));
                    //so here we're trying to see if the goal is either in the world's state or is the result of one of the action's effects? Is that why we put them together? 
                    //if all of the goals equal the worldstate + the action's effects, this triggers. 
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    //Debug.log("NO SOLUTION : " + "Current State " + GoapAgent.prettyPrint(currentState) + "\n" + "Goal : " + GoapAgent.prettyPrint(goal));

                    HashSet<GoapAction> subset = actionSubset(usableActions, action); //taking all actions and the current action in this iteration . The current action needs to be removed because it didn't match the goal

                    bool found = buildGraph(node, leaves, subset, goal); //this time we're redoing this whole thing while taking the useless action out of the equation, but why? What makes this action useless? 
                    if (found)
                        foundOne = true;
                }


            }

        }
        return foundOne;
    }


    private bool GoalInState(List<Goal> goals, List<Condition> currentState)
    {
        bool allMatch = false;
        foreach (Goal g in goals)
        {
            bool match = false;
            foreach (Condition worldcon in currentState)
            { //if any of the action's goals match with the world's current state -- since you're going backwards?
                if (worldcon.Name == g.GoalWithPriority.Key.Name && worldcon.Value == g.GoalWithPriority.Key.Value)
                {
                    match = true;
                    break;
                }

            }
            if (!match)
            {
                allMatch = false;
            }
        }
        return allMatch;
    }
    /**
     * create a subset of the actions exdluding the removeMe one. Creates a new set;
     **/
    private HashSet<GoapAction> actionSubset(HashSet<GoapAction> actions, GoapAction removeMe)
    {
        HashSet<GoapAction> subset = new HashSet<GoapAction>();

        foreach (GoapAction a in actions)
        {
            //for each usable action
            if (!a.Equals(removeMe))
                //if the current action is NOT the action we need to remove, add it to the subset, which we will retur
                //why do we need to get rid of this action, though? 
                subset.Add(a);
        }

        return subset;
    }



    private bool goalInState(HashSet<KeyValuePair<string, object>> goal, HashSet<KeyValuePair<string, object>> currentState)
    {
        bool allMatch = true;


        foreach (KeyValuePair<string, object> g in goal)
        {
            //  //Debug.log(g.Key + " , " + g.Value);
            //Here, we only have two goals, so this loop will go through twice.
            bool match = false;
            foreach (KeyValuePair<string, object> s in currentState)
            {
                if (s.Equals(g))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                allMatch = false;
                //this is testing to see if all the goals match the effects and world state? or if just one of them does? 
            }
        }
        return allMatch;


    }



    /**Check that all itmes in 'test' are in 'state'. If just one does not match or is not there, then this returns false**/

    //here we're checking if the preconditions of the action are in the world state
    private bool PreconditionsInWorldState(GoapAction action, List<Condition> actionPreconditions, List<Condition> worldState)
    {
        bool allMatch = false;
        foreach (Condition precon in actionPreconditions)
        {
            bool match = false;
            foreach (Condition worldcon in worldState)
            {
                if (worldcon.Name == precon.Name && worldcon.Value == precon.Value)
                {
                    match = true;
                    break;
                }

            }
            if (!match)
            {
                allMatch = false;
            }
        }
        return allMatch;
    }



    private bool inState(GoapAction action, HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
    {
        bool allMatch = true;



        foreach (KeyValuePair<string, object> t in test)
        {
            //  //Debug.log("Action Precondition: " + action.GetType() + " " + t.Key + "," + t.Value);
            bool match = false;
            foreach (KeyValuePair<string, object> s in state)
            {
                // //Debug.log("Does " + t + " match " + s + " " + t.Equals(s));
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                allMatch = false;
            }
        }
        return allMatch;
    }

    /**
	 * Apply the stateChange (meaning how the effects will change the world state) to the currentState
	 */

    private List<Condition> PopulateState(List<Condition> worldState, List<Condition> effects)
    {
        List<Condition> state = new List<Condition>();

        foreach (Condition con in worldState)
        {
            state.Add(con);
        }

        foreach (Condition change in effects)
        {
            bool exists = false;
            foreach (Condition con in worldState)
            {
                if (con.Name == change.Name && con.Value == change.Value)
                {
                    exists = true;
                    break;
                }
            }
            if (exists)
            {
            }
            else
            {
                state.Add(change);
            }
        }
        return state;
    }
    private HashSet<KeyValuePair<string, object>> populateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)
    {
        //stateCHANGE = how the state will change based on the effects of the actions, but WHY are we putting them together?! 

        //currentState = worldState
        //stateChange = the action's effects 
        //uuh is this method reorganizing 

        //stateChange is the effect of the current action -- this is running twice because you have two usable actions. Exists checks to see if the two are equal, of which right now only charge would be
        //but if it wasn't equal, it would add it to the end

        //currentState is the current world state
        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();

        foreach (KeyValuePair<string, object> s in currentState)
        {
            //adding the world state to this "state" Hashset 
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }


        //  string a = " ";
        //  string q = " ";
        foreach (KeyValuePair<string, object> change in stateChange)
        {
            //  a += " " + change;
            //foreach effect in this action 

            //if the key exists in the current state, update the value
            bool exists = false;
            foreach (KeyValuePair<string, object> s in state)
            {
                //  q += " " + s;
                if (s.Equals(change))
                { //if one of the world states equals the effects 
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                // //Debug.log("Exists! " + change);
                //is this reorganizing it so that it fits the same order?

                //RemoveWhere removes all elements that match the conditions defined by the predicate, so in this case where kvp's key is equal to the key, 
                //for example "charge" or "stay alive", of the effect
                //so remove from the world state populated "state" hashset 
                //remove where the key in state is equal to the effect's current key, then add it so it has the same  

                state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                state.Add(updated);
            }

            //if it doesn't exist in the current state, add it
            else
            {
                //  //Debug.log("Doesn't exist so adding " + change);
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }

        }
        ////Debug.log("Here are the effects " + a);
        ////Debug.log("Here is the world state " + q);
        //string n = " ";
        //foreach (KeyValuePair<string, object> up in state)
        //{
        //    n += " " + up;
        //}
        //    //Debug.log("Here is the updatedState ? " + n);
        return state;
    }



    /**
     * Used for building up the graph and holding the running costs of actions
     * */

    private class Node_
    {
        public Node_ parent;
        public float runningCost;
        public List<Condition> state;
        public GoapAction action;

        public Node_(Node_ parent, float runningCost, List<Condition> state, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

    private class Node
    {
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string, object>> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

}
