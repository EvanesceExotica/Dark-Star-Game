using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoulBehavior : PooledObject
{

    public static event Action<GameObject> SoulSpawned;
    SoulRotateScript ourSoulRotateScript;
    float timeOutInTheOpen;

    float timeAtWhichWeWereCreated;

    float maximumTimeWeCanFloat;
    Rigidbody2D rb;
    float followSpeed;
    SpriteRenderer ourSpriteRenderer;
    GameObject player;
    float smallScaleX;
    float smallScaleY;
    bool scaling;
    Vector2 currentScale;

    public bool beingPrimed;
    public bool launching;

    public static event Action MissedPowerUp;
    public void MissedPowerUpWrapper(){

        if(MissedPowerUp != null){
            MissedPowerUp();
        }
    }
    public void Attached()
    {
        attachmentState = Attachments.AttachedToPlayer;
        ChangeScaleOfObject();
        if (AttachToPlayer != null)
        {
            AttachToPlayer(this.gameObject);
        }
    }


    public void Detached()
    {
        if (DetachFromPlayer != null)
        {
            DetachFromPlayer(this.gameObject);
        }
    }

    public static event Action<GameObject> AttachToPlayer;
    public static event Action<GameObject> DetachFromPlayer;


    void ChangeScaleOfObject()
    {
        ScaleObject.AdjustScale(this, this.gameObject, -3.0f, 1.0f, 1.0f, false);
    }

    PlayerReferences playerReferences;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerReferences = player.GetComponent<PlayerReferences>();
        maximumTimeWeCanFloat = 30.0f;
    }

    void WeWereSpawned(){
        if(SoulSpawned != null){
            SoulSpawned(this.gameObject);
        }
    }
    void OnEnable()
    {

        timeAtWhichWeWereCreated = Time.time;
        attachmentState = Attachments.DetachedFromPlayer;
        WeWereSpawned();
    }


    public enum Attachments
    {
        AttachedToPlayer,
        DetachedFromPlayer
    }

    public Attachments attachmentState;


    // Use this for initialization
    void Start()
    {

        attachmentState = Attachments.DetachedFromPlayer;
        followSpeed = 5.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (attachmentState == Attachments.AttachedToPlayer)
        {
            //Debug.Log("Following!");
            followAlong(player);
        }
        if (launching)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > 6.0f)
            {
                //logic here is that we want it to attach back to the player if it flies off without hitting a powerup
                rb.velocity = new Vector2( 0, 0);
                MissedPowerUpWrapper();
                Attached();
                //TODO: WANT TO TREAT AS IF ADDING A NEW SOUL

               // ReturnToPool();
            }
        }
        if (attachmentState == Attachments.DetachedFromPlayer)
        {

            if (Time.time >= timeAtWhichWeWereCreated + maximumTimeWeCanFloat)
            {
                //if we've been out here for at or longer than our maximum time, which is 30 seconds at default
                ReturnToPool();
            }

        }

    }



    public void followAlong(GameObject ourTarget)
    {
        if (beingPrimed && !launching)
        {
            //return;
            transform.position = ourTarget.transform.position;
        }
        else if (launching)
        {
            return;
        }
        if (transform.parent != null)
        {
            //transform.position = transform.parent.positi:on;
        }

        /*   else
           {
               Vector3 ourPosition = transform.position;

               Vector3 ourTargetsPosition = ourTarget.transform.position;
               float ourTargetsPositionX = ourTargetsPosition.x;
               float ourPositionX = ourPosition.x;
               if (ourTargetsPositionX < ourPositionX)
               {
                   if (Vector3.Distance(ourTargetsPosition, ourPosition) < 2.0f)
                   {
                       rb.velocity = Vector2.zero;
                   }
                   else
                   {
                       Vector2 targetDirection = (Vector2)Vector3.Normalize(ourTargetsPosition - ourPosition);
                       rb.velocity = new Vector2(targetDirection.x * followSpeed, targetDirection.y * followSpeed);
                   }
                   //FlipLeft();
               }
               else if (ourTargetsPositionX > ourPositionX)
               {


                   if (Vector3.Distance(ourTargetsPosition, ourPosition) < 2.0f)
                   {
                       rb.velocity = Vector2.zero;
                   }
                   else
                   {
                       Vector2 targetDirection = (Vector2)Vector3.Normalize(ourTargetsPosition - ourPosition);
                       rb.velocity = new Vector2(targetDirection.x * followSpeed, targetDirection.y * followSpeed);
                   }
                   //FlipRight();
               }
           }*/

    }


    void FlipRight()
    {
        ourSpriteRenderer.flipX = false;
    }

    void FlipLeft()
    {
        ourSpriteRenderer.flipX = true;
    }
}
