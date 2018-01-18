using UnityEngine;
using System.Collections;

public class MigrateAction : GoapAction
{


  

    float bufferAmount = 4.0f;

    public float maxChargeAmount = 5;
    public float currentChargeAmount = 0;
    public float chargeInterval;

    public MigrateAction()
    {
        AddPrecondition(new Condition("charge", true));


     //   AddPrecondition(new Condition("threatInRange", false));
     //I think that the "ThreatInRange" should be a procedural precondition rather than one put here since it's not something that can be satisfied by the agent itself
     //then again, won't it be an issue when we want to change the world State so that the agent goes after the player instead? Or would we just change the priority?

//This "FindNewArea" might also be a procedural precondition
        addEffect("findNewArea", true);


        //addPrecondition("charge", true);
        //addEffect("charge", true);

    }


    bool foundNewLocation = false;

 

    public override void reset()
    {
        foundNewLocation = false;
        //        currentChargeAmount = 0;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        hasVectorTarget = true;


        target = GameObject.Find("Dark Star");
        return true;
    }


    void FindNewArea()
    {

    }

    public override bool perform(GameObject agent)
    {

      

        return true;

    }

    public override bool requiresInRange()
    {

        return true;
    }

    public override bool isDone()
    {
        return foundNewLocation;

    }

   
    // Use this for initialization
    void Start()
    {

        cost = 200.0f;
        maxChargeAmount = 10.0f;
        chargeInterval = 1.0f;
        currentChargeAmount = 5.0f;

    }

    // Update is called once per frame
    void Update()
    {

    }
}


