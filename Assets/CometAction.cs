using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometAction : GoapAction
{

    public float startTime;
    public float duration = 3.0f;

    bool chargingAtPlayer;
    float speed = 10.0f;
    public CometAction()
    {
        AddPrecondition(new Condition("threatInRange", true));
        AddEffect(new Condition("defend", true));
    }
    // Use this for initialization
    bool hasFired;
    bool playerHit;


    public override void reset()
    {
        hasFired = false;
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
    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            startTime = Time.time;
        }
        performing = true;
        if (interrupted)
        {
			//want to make sure this isn't necessarily interrupted by the player constantly triggering
			//on trigger enter should only trigger once, but you never know.

            performing = false;
        }
        return performing;
    }

    void ChargeAtPlayer()
    {
        startTime = Time.time;
        chargingAtPlayer = true;

        float distance = Vector2.Distance(gameObject.transform.position, target.transform.position);
        //the opposite direction from mousepositionworld (destination ) - transform.position

        Vector2 direction = (Vector2)(target.transform.position - transform.position);
        direction.Normalize();
        ourRigidbody2D.velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collider2D hit)
    {

        //TODO: May want to move this method somewhere else for clarity
        if (hit.gameObject == GameStateHandler.player)
        {
            playerHit = true;
        }
    }

    public override bool isDone()
    {
        return hasFired;
    }
    // Use this for initialization

	public IEnumerator ChargingAtPlayer(){
		startTime = Time.time;
		ChargeAtPlayer();
		while(Time.time < startTime + duration){

			if(playerHit){
				break;
			}
			yield return null;
		}
		ourRigidbody2D.velocity = new Vector2(0, 0);
		hasFired = true;
	}
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
