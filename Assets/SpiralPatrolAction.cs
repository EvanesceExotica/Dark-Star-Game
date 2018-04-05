using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralPatrolAction : GoapAction
{

    float circleSpeed;
    float circleSize;
    float circleGrowSpeed;
    bool touchedSwitch;
    List<Switch> switchesTouched= new List<Switch>();
    float forwardSpeed;
    //the comet travels in spirals around the star, leaving temporary trails that are destroyed at after each phase (maybe use the "waypoint" system?)
    //the player can ride the trails?
    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            performing = true;
            StartCoroutine(Spiral());
        }
        return performing;

    }

    public IEnumerator Spiral()
    {
        circleSize = DarkStar.radius + 5.0f;
        circleGrowSpeed = 0.3f;
        circleSpeed = 1.0f;
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        while (circleSize <= GameStateHandler.voidBoundaryRadius)
        {
            if(touchedSwitch){

                
                touchedSwitch = false;
            }
            if(switchesTouched.Count == 2){
                break;
            }
            // circleSpeed = frequency, circleSize = amplitude -- the phase shift is the only thing that's "added" rather than multiplied so to speak
            xPosition = circleSize * Mathf.Sin(Time.time * circleSpeed) ;
            yPosition = circleSize * Mathf.Cos(Time.time * circleSpeed) ;
            circleSize += circleGrowSpeed;
            transform.position = new Vector2(xPosition, yPosition);
            yield return null;
        }
        hasTouchedTwoSwitches = true;

    }

 public void AddSwitchWeTouched(GameObject switchTouched, GameObject objectThatEnteredSwitch){
       if(objectThatEnteredSwitch == this.gameObject) {
           switchesTouched.Add(switchTouched);
       }
    }

    public void OnCollisionEnter2D(Collision2D hit){
      if(hit.collider.GetComponent<PlayerReferences>() != null){

      } 
    }
    public float startTime;

    public float duration = 3.0f;

    bool chargingAtPlayer;
    float speed = 10.0f;
    public SpiralPatrolAction()
    {
        AddEffect(new Condition("defend", true));
    }
    // Use this for initialization
    bool hasTouchedTwoSwitches;
    bool playerHit;


    public override void reset()
    {
        hasTouchedTwoSwitches = false;
        chargingAtPlayer = false;
    }
    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = GameStateHandler.player;
        return true;
    }
   

   
    public override bool isDone()
    {
        return hasTouchedTwoSwitches;
    }
    // Use this for initialization


   
}
