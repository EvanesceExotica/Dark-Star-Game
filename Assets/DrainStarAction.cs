using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainStarAction : GoapAction
{

public DrainStarAction(){

    cost = 300;
	AddEffect(new Condition("eat", true));
}
    bool hasDrained;
    float startTime;

	float bufferAmount = 5.0f;

    float drainDuration = 10.0f;
    public override void reset()
    {
        hasDrained = false;
    }
    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
         hasVectorTarget = true;
         target = GameStateHandler.DarkStarGO;
         //you can change this
         vectorTarget = FindLocationInSafeZone.FindLocationInCircleExclusion(GameStateHandler.DarkStarGO, bufferAmount);
        return true;
    }
    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            startTime = Time.time;
			PlayDrainLaserEffect();
        }
        performing = true;
        return performing;
    }

	void PlayDrainLaserEffect(){

		//this method will play the drain effect 
	}

    public override bool isDone()
    {
        return hasDrained;
    }


    // Update is called once per frame
    void Update()
    {
        if (performing)
        {
            if (Time.time < startTime + drainDuration)
            {
                hasDrained = true;
            }
        }

    }
}
