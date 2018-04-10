using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class SpaceMonster : PooledObject, IGoap
{
    public enum AggressionType{
        passive,
        defensive,
        aggressive
    }

    public AggressionType ourAggressionType;

   
    protected int id;

    public int ID
    {
        get
        {
            return id;
        }
    }
    public GameObject player;
    public UniversalMovement movement;

    public int powerLevel;
    public float speed;
    public float stamina;
    public float maxStamina;
    public int hitPoints;
    public int damage;
    public bool playerInRange;
    int maxPriority;//i

    GoapAgent ourAgent;

    ThreatTrigger ourThreatTrigger;

    public List<Goal> ourGoals = new List<Goal>();

    public List<Goal> GetGoalState()
    {
        return ourGoals;
    }

    public virtual void OnEnable(){

        SetMoveSpeed();
    }

    void SetMoveSpeed(){
        movement.moveSpeed = speed;
    }

   
    public virtual void ReactToInterruption(GameObject interruptor)
    {
        ourAgent.currentAction.ImportantEventTriggered(interruptor);
      ourAgent.currentAction.interrupted = true;  
    }
    public virtual void ReactToIncapacitation(GameObject incapacitator){
        ourAgent.currentAction.incapacitated = true;

    }

    public void DestroyMe()
    {
        ReturnToPool();
    }

    public virtual void ReturnToNormalFunction()
    {

        //Debug.Log("Threat gone. Returning to normal");
    }
    public void ChangeGoalPriority(Goal changedGoal)
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

    List<List<Condition>> plansAlreadyTried = new List<List<Condition>>();

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();
        worldData.Add(new KeyValuePair<string, object>("eat", false));
        worldData.Add(new KeyValuePair<string, object>("stayAlive", false));
        worldData.Add(new KeyValuePair<string, object>("charge", false));
        worldData.Add(new KeyValuePair<string, object>("threatInRange", false));


        return worldData;
    }

    public virtual List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
        worldData.Add(new Condition("darkStarBurstingSoon", false));
        
        return worldData;
    }

    public abstract void CreateGoalState();

    bool TestIfListsAreEquivalent(List<Goal> list)
    {

        return true;
    }

    public void PlanFailed(List<Goal> failedGoal)
    {

        //if (TestIfListsAreEquivalent(failedGoal))
        //{

        //}
    }

    public void PlanFound(List<Goal> goals, Queue<GoapAction> actions) { }

    public void ActionsFinished() { }

    public void PlanAborted(GoapAction aborter) { }

    public bool MoveAgent(GoapAction nextAction)
    {

        Debug.Log(this.gameObject.name + " is moving toward " + nextAction.target.name);
        Vector2 targetPosition = new Vector2(0, 0);

        if (nextAction.hasVectorTarget)
        {
            movement.MoveToVectorTarget(nextAction.vectorTarget);
            targetPosition = nextAction.vectorTarget;
        }
        else
        {
            movement.MoveToTarget(nextAction.target);
            targetPosition = nextAction.target.transform.position;

        }

        if(Vector2.Distance(transform.position, targetPosition) <= 4.0f){
            nextAction.setInRange(true);
            return true;
        }
        else{
            return false;
        }
    }


    //   public abstract HashSet<KeyValuePair<string, object>> changeGoalState();

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {

    }



    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        //Yay we found a plan for our goal
    }

    public void actionsFinished()
    {
        //Everything is done, we completed our actions for this goal. Hooray!
    }

    public void planAborted(GoapAction aborter)
    {

        //An action bailed out of the plan. State has been reset to plan again.
        //Take note of what happened and make sure if you run the same goal again
        //that it can succeed;

    }

    public bool moveAgent(GoapAction nextAction)
    {
        //        //Debug.Log(gameObject.name + " is headed toward this target  " + nextAction.target.name);
        Vector2 targetPosition = new Vector2(0, 0);

        //TODO: Cancel movement somehow

        if (nextAction.interrupted || nextAction.incapacitated)
        {
            //Debug.Log("<color=red> " + gameObject.name + "'s next action : " + nextAction.name + " was interrupted </color>" );
            nextAction.performing = false;
        }

        if (nextAction.hasVectorTarget)
        {
            movement.MoveToVectorTarget(nextAction.vectorTarget);
            targetPosition = nextAction.vectorTarget;
        }
        else
        {
            movement.MoveToTarget(nextAction.target);
            targetPosition = nextAction.target.transform.position;

        }

        if (Vector2.Distance(gameObject.transform.position, (Vector3)targetPosition) <= 5.0f)
        {
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }

    public Enemy enemy;
    public virtual void Awake()
    {

        ourAgent = GetComponent<GoapAgent>();
       enemy = GetComponent<Enemy>(); 
        ourThreatTrigger = gameObject.GetComponentInChildren<ThreatTrigger>();
        enemy.ourMovement.SomethingImpededOurMovement += this.ReactToIncapacitation;
        //enemy.ourMovement.NothingImpedingOurMovement += this.ReturnToNormalFunction;

        if (ourThreatTrigger != null)
        {
            ourThreatTrigger.threatInArea += this.ReactToInterruption;
            ourThreatTrigger.SetAllClear += this.ReturnToNormalFunction;
        }

        movement = GetComponent<UniversalMovement>();

    }

    public virtual void OnDisable()
    {
        ourThreatTrigger.threatInArea -= this.ReactToInterruption;
    }
    // Use this for initialization
    void Start()
    {
        stamina = maxStamina;

    }

    // Update is called once per frame
    void Update()
    {

    }




}
