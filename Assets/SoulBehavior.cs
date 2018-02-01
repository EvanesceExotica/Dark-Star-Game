using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoulBehavior : PooledObject
{

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

    public void Attached()
    {
        attachmentState = Attachments.AttatchedToPlayer;
        ChangeScaleOfObject();
        if (AttachToPlayer != null)
        {
            AttachToPlayer(this.gameObject);
        }
    }


    public void Detatched()
    {
        if (DetatchFromPlayer != null)
        {
            DetatchFromPlayer(this.gameObject);
        }
    }

    public static event Action<GameObject> AttachToPlayer;
    public static event Action<GameObject> DetatchFromPlayer;


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
    }


    public enum Attachments
    {
        AttatchedToPlayer,
        DetatchedFromPlayer
    }

    public Attachments attachmentState;


    // Use this for initialization
    void Start()
    {

        attachmentState = Attachments.DetatchedFromPlayer;
        followSpeed = 5.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (attachmentState == Attachments.AttatchedToPlayer)
        {
            //Debug.Log("Following!");
            followAlong(player);
        }

    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
    }

    public void followAlong(GameObject ourTarget)
    {
        if (beingPrimed &&  !launching)
        {
            //return;
           transform.position = ourTarget.transform.position;
        }
        else if(launching){
            return;
        }

        else
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
        }

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
