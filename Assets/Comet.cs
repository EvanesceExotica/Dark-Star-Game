using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : SpaceMonster
{
    //TODO: Comets need to be born INSIDE of the dark star to avoid the jumping. Maybe have them spiral inward back toward it rather than outward?
    //TODO: When they spiral inward, have them be destroyed by the star, which might be an extra failure for the player

    
    bool spiraledOutward;


  private void Start()
    {
        maxStamina = 800f;
        stamina = maxStamina;
        damage = 0;
        speed = 10.0f;
        CreateGoalState();
    }

    public override void OnEnable(){

    }

    public override void OnDisable(){

    }

     public override void ReactToInterruption(GameObject interruptor)
    {
        Debug.Log(gameObject.name + " is reacting to " + interruptor);
        base.ReactToInterruption(interruptor);
        Goal priority = new Goal(new Condition("defend", true), 90);
        ChangeGoalPriority(priority);
    }


    public override void ReturnToNormalFunction()
    {
        base.ReturnToNormalFunction();
        //Debug.Log("Threat gone. Returning to normal");
        Goal priority = new Goal(new Condition("defend", true), 10);
        ChangeGoalPriority(priority);
    }

      public Vector2 DetermineSpiralVectorTarget()
    {
        float circleSize = DarkStar.radius - 4.0f;
        float circleSpeed = 1.0f;
        float xPosition = circleSize * Mathf.Sin(Time.time * circleSpeed);
        float yPosition = circleSize * Mathf.Cos(Time.time * circleSpeed);
        Vector2 vecTarget = new Vector2(xPosition, yPosition);
        return vecTarget;
    }

   
    public override List<Condition> GetWorldState()
    {
        List<Condition> worldData = new List<Condition>();
       // worldData.Add(new Condition("threatInRange", false));
        worldData.AddRange(base.GetWorldState());
        worldData.Add(new Condition("trail", false));
        //We want the SpiralPatrolAction's effect to be spiral inward
        worldData.Add(new Condition("spiraledOutward", false));
        worldData.Add(new Condition("spiralInward", false));
        worldData.Add(new Condition("hibernate", false));
        worldData.Add(new Condition("defend", false));
        //worldData.Add(new Condition(""), true);
        return worldData;
    }

    TrailRenderer ourTrailRenderer;
    float trailRendererDefaultTime;

    public override void Awake()
    {
        base.Awake();
        id = 2;
        ourSpawnBehaviour = SpaceMonster.SpawnBehaviour.CenterOfStar;
        enemy.ourMovement.BornFromStar = true;
        ourTrailRenderer = GetComponent<TrailRenderer>();
        trailRendererDefaultTime = ourTrailRenderer.time;
        
    }
    public override void CreateGoalState()
    {
        List<Goal> goal = new List<Goal>();
        goal.Add(new Goal(new Condition("hibernate", true), 20));
        goal.Add(new Goal(new Condition("defend", true), 10));

        ourGoals = goal;
    }

	
	
	
}
