using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoap  {

    List<Goal> GetGoalState();

    List<Condition> GetWorldState();

    void CreateGoalState();

    void PlanFailed(List<Goal> failedGoals);

    void PlanFound(List<Goal> goals, Queue<GoapAction> actions);

    void ActionsFinished();

    void PlanAborted(GoapAction aborter);

    bool MoveAgent(GoapAction nextAction);

    HashSet<KeyValuePair<string, object>> getWorldState();



   // HashSet<KeyValuePair<string, object>> changeGoalState();

    void planFailed(HashSet<KeyValuePair<string, object>> failedGoal);

    void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions);


    void actionsFinished();

    void planAborted(GoapAction aborter);

    bool moveAgent(GoapAction nextAction);

	
}
