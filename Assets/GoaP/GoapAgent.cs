﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Profiling;


public class GoapAgent : MonoBehaviour, IComparable, IComparable<Goal>
{

    public event Action<GoapAction> ActionChanged;
    public GoapAction currentAction;

    public GameObject currentTarget;

    void ChangedAction(GoapAction action)
    {
        if (ActionChanged != null)
        {
            ActionChanged(action);
        }
    }

    public int CompareTo(Goal x)
    {
        int xz = 0;
        return xz;
    }
    public int CompareTo(object x)
    {
        int z = 0;
        return z;
    }

    private FSM stateMachine;

    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    private FSM.FSMState prepPerformanceState;

    private FSM.FSMState cleanupActionState;

    private FSM.FSMState stunnedState;

    private HashSet<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private GoalOAPPlanner planner_;

    private IGoap dataProvider;

    public IGoap DataProvider
    {
        get
        {
            return dataProvider;
        }
    }
    private GoapPlanner planner;

    public event Action threatInRange;

    public List<GoapAction> availableActions_;

    // Use this for initialization
    void Start()
    {

        stateMachine = new FSM();
        availableActions = new HashSet<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        planner_ = new GoalOAPPlanner();
        enemy = GetComponent<Enemy>();
        findDataProvider();
        createIdleState();
        createMoveToState();
        createPerformActionState();
        createStunnedState();
        createPrepPerformanceState();
        createCleanUpState();
        stateMachine.pushState(idleState);
        loadActions();


    }

    public Enemy enemy;

    // Update is called once per frame
    void Update()
    {

        Profiler.BeginSample("My Sample");
        //Profiler.BeginSample("My Sample");
        stateMachine.Update(this.gameObject);

        Profiler.EndSample();
    }

    public void addAction(GoapAction a)
    {
        availableActions.Add(a);
        availableActions_.Add(a);
    }

    public GoapAction getAction(Type action)
    {
        foreach (GoapAction g in availableActions)
        {
            if (g.GetType() == action)
            {
                return g;
            }
        }
        return null;

    }

    private bool hasActionPlan()
    {
        return currentActions.Count > 0;
    }

    List<Goal> OrderByPriority(List<Goal> goals)
    {
        List<Goal> orderedGoals = new List<Goal>();
        // orderedGoals.AddRange(goals);
        orderedGoals = goals.OrderByDescending(x => x.GoalWithPriority.Value).ToList();

        return orderedGoals;
    }

    private void createStunnedState()
    {
        //TODO: -- SEE IF THIS SHOULD BE AN ACTION INSTEAD
        stunnedState = (fsm, gameObj) =>
        {
            Debug.Log(gameObject.name + " is Stunned!");

            GoapAction ourCurrentAction = currentActions.Peek();
            bool incapacitated = enemy.ourMovement.incapacitated;
            if (!incapacitated)
            {
                //if no longer stunned, go back to idleState to recalculate our plan
                fsm.popState();
                //fsm.pushState(idleState);
                //TODO: MAke sure that this is working alright vvv
                fsm.pushState(cleanupActionState);
            }
        };
    }

    private void createPrepPerformanceState()
    {
        prepPerformanceState = (fsm, gameObj) =>
        {
            GoapAction ourCurrentAction = currentActions.Peek();
            ourCurrentAction.PrepareCurrentAction();
            fsm.popState();
            fsm.pushState(performActionState);
        };
    }

    private void createCleanUpState()
    {
        cleanupActionState = (fsm, gameObj) =>
        {
            GoapAction ourCurrentAction = currentActions.Peek();
            ourCurrentAction.CleanUpAction();
            currentActions.Dequeue();
            fsm.popState();
            fsm.pushState(idleState);
        };
    }

    private void createIdleState()
    {
        //this is a lambda expression nameless/anonymous delegate thingy of type FSM.FSMState 
        idleState = (fsm, gameObj) =>
        {
            Profiler.BeginSample("Gathering and ordering goals by priority  ");
            List<Condition> originalState = dataProvider.GetWorldState();

            List<Goal> goals = dataProvider.GetGoalState();
            goals = OrderByPriority(goals);

            Debug.Log(dataProvider.GetType().Name + "'s WORLD STATE " + prettyPrint(originalState));
            Debug.Log(dataProvider.GetType().Name + "'s GOAL STATE " + prettyPrint(goals));

            Profiler.EndSample();

            Profiler.BeginSample("Begin Planning -- for loop through goals and calling planner");
            Queue<GoapAction> plan = null;
            foreach (Goal ourGoal in goals)
            { //this cycles through the goals by priority until it finds one that works with a plan 
                plan = planner_.Plan(ourGoal, availableActions_, originalState, this.gameObject);
                if (plan != null)
                {
                    //if you find a plan that works, break and continue on
                    Debug.Log("<color=cyan>Plan found!:</color>" + prettyPrint(plan) + " for " + gameObj.name + " at " + Time.fixedTime);
                    break;
                }

            }
            Profiler.EndSample();


            //  planner_.Plan(goals, availableActions_, originalState, this.gameObject);
            Profiler.BeginSample("Checking to see if plan is null/failed and returning to idle if so");
            if (plan != null)
            {
                currentActions = plan;
                dataProvider.PlanFound(goals, plan);
                fsm.popState();
                fsm.pushState(prepPerformanceState); //set any preperation before performing
                //fsm.popState(); // move to PerformAction state
                //fsm.pushState(performActionState);
            }
            else
            {
                // ugh, we couldn't get a plan
                Debug.Log("<color=orange>Failed Plan:</color>" + prettyPrint(goals) + " for " + gameObj.name);
                dataProvider.PlanFailed(goals);
                fsm.popState(); // move back to IdleAction state
                fsm.pushState(idleState);
            }



            Profiler.EndSample();
        };
    }


    private void createMoveToState()
    {
        moveToState = (fsm, gameObj) =>
        {
            Profiler.BeginSample("Move State Creation");
            // move the game object

            GoapAction action = currentActions.Peek();
            if (action.requiresInRange() && action.target == null)
            {
                Debug.Log("<color=red>Fatal error:</color> Action " + action.ToString() + " requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                fsm.popState(); // move
                fsm.popState(); // perform
                fsm.pushState(cleanupActionState);
                //fsm.pushState(idleState);
                return;
            }
            if (action.incapacitated)
            {
                fsm.popState();
                fsm.popState();
                fsm.pushState(stunnedState);
            }
            if (action.interrupted)
            {
                Debug.Log("<color=red>Fatal error:</color> Action " + action.ToString() + " of " + gameObject.name + " was interrupted by something ");
                fsm.popState();
                fsm.popState();
                fsm.pushState(cleanupActionState);
                //fsm.pushState(idleState);
                return;
            }
            // get the agent to move itself
            if (dataProvider.moveAgent(action))
            {
                fsm.popState();
            }

            /*MovableComponent movable = (MovableComponent) gameObj.GetComponent(typeof(MovableComponent));
			if (movable == null) {
				//Debug.Log("<color=red>Fatal error:</color> Trying to move an Agent that doesn't have a MovableComponent. Please give it one.");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}
			float step = movable.moveSpeed * Time.deltaTime;
			gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);
			if (gameObj.transform.position.Equals(action.target.transform.position) ) {
				// we are at the target location, we are done
				action.setInRange(true);
				fsm.popState();
			}*/
            Profiler.EndSample();
        };
    }

    private void createPerformActionState()
    {

        performActionState = (fsm, gameObj) =>
        {
            Profiler.BeginSample("Perform state");

            GoapAction action = currentActions.Peek();
            if (action.incapacitated)
            {
                Debug.Log(gameObject.name + " Gotta move to stun state now");
                fsm.popState();
                fsm.pushState(stunnedState);
                return;
            }
            // perform the action
            if (!hasActionPlan())
            {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.popState();
                fsm.pushState(cleanupActionState);
                //                fsm.pushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            if (action.isDone())
            {
                //TODO: We need to have the action clean up BEFORE we dequeue -- maybe have CleanUp do isDone();
                // the action is done. Remove the first so we can perform the next one
                ///currentActions.Dequeue();
                fsm.pushState(cleanupActionState);
            }

            if (hasActionPlan())
            {
                // perform the next action
                action = currentActions.Peek();
                ChangedAction(action);
                currentAction = action;

                //                //Debug.Log("Here's our current action " + action);
                bool inRange = action.requiresInRange() ? action.isInRange() : true;

                if (inRange)
                {
                    // we are in range, so perform the action
                    bool success = action.perform(gameObj);

                    if (!success)
                    {
                        Debug.Log("<color=red> ACTION FAILED OH NO WHY</color>");
                        // action failed, we need to plan again
                        fsm.popState();
                        fsm.pushState(cleanupActionState);
                        //fsm.pushState(idleState);
                        dataProvider.planAborted(action);
                    }
                }
                else
                {
                    // we need to move there first
                    // push moveTo state, it'll pop and return to this when we're done
                    fsm.pushState(moveToState);
                }

            }
            else
            {
                //Debug.Log("Actions completed");
                // no actions left, move to Plan state
                fsm.popState();
                // fsm.pushState(idleState);
                fsm.pushState(cleanupActionState);
                dataProvider.actionsFinished();
            }
            Profiler.EndSample();

        };
    }

    private void findDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void loadActions()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions)
        {
            availableActions.Add(a);
            availableActions_.Add(a);
        }
        //Debug.Log("Found actions: " + prettyPrint(actions));
    }
    #region prettyprint stuff
    public static string prettyPrint(HashSet<KeyValuePair<string, object>> state)
    {
        String s = "";
        foreach (KeyValuePair<string, object> kvp in state)
        {
            s += kvp.Key + ":" + kvp.Value.ToString();
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(Queue<GoapAction> actions)
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

    public static string prettyPrint(GoapAction[] actions)
    {
        String s = "";
        foreach (GoapAction a in actions)
        {
            s += a.GetType().Name;
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(GoapAction action)
    {
        String s = "" + action.GetType().Name;
        return s;
    }

    public static string prettyPrint(List<Goal> goals)
    {
        String s = "";
        foreach (Goal g in goals)
        {
            s += g.GoalWithPriority.Key.Name;
            s += ", ";
            s += g.GoalWithPriority.Value;
        }
        return s;
    }


    public static string prettyPrint(List<Condition> con)
    {
        String s = "";
        foreach (Condition c in con)
        {
            s += c.Name + "," + c.Value;
            s += ", ";
        }
        return s;
    }
    #endregion
}
