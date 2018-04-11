using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometAction : GoapAction
{
    //
    public float startTime;
    public float duration = 3.0f;

    bool chargingAtPlayer;
    float speed = 10.0f;
    public CometAction()
    {
        cost = 100f;
        AddEffect(new Condition("defend", true));
    }
    // Use this for initialization
    bool hasFired;
    bool playerHit;

    public override void ImportantEventTriggered(GameObject intruder)
    {

    }

    public override void reset()
    {
        hasFired = false;
        chargingAtPlayer = false;
        interrupted = false;
        incapacitated = false;
    }
    public override bool requiresInRange()
    {
        return false;
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
            performing = true;
            StartCoroutine(ChargingAtPlayer());
        }
        performing = base.perform(agent);
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
        Debug.Log(ourRigidbody2D.velocity);
        ourRigidbody2D.velocity = direction * 50;
        //ourRigidbody2D.AddForce(direction * speed);
    }





    void OnCollisionEnter2D(Collision2D hit)
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

    public IEnumerator ChargingAtPlayer()
    {
        startTime = Time.time;
        ourGoapAgent.enemy.SetToCollideWithPlayerLayer();
        ChargeAtPlayer();
        while (Time.time < startTime + duration)
        {

            if (playerHit)
            {
                //if we collide with the player
                break;
            }
            yield return null;
        }
        ourRigidbody2D.velocity = new Vector2(0, 0);
        ourGoapAgent.enemy.SetToNotCollideWithPlayer();
        if (playerHit)
        {
            chargingAtPlayer = false;
            hasFired = true;
        }
        else{
            performing  = false;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
}
