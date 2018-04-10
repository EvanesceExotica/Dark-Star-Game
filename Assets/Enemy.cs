using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpawnSoul))]
[RequireComponent(typeof(Rigidbody2D))]





public class Enemy : MonoBehaviour, IPullable, IDigestible, IBashable {

    public SpaceMonster ourEnemyType;
    public bool hookBroken;
    public bool beingPulled;
    public bool beingPushed;

   public UniversalMovement ourMovement;


   public Rigidbody2D ourRigidbody2D;

   public SpaceMonster ourSpaceMonster;


    public event Action<ShowDarkStarPhase.DarkStarPhases> WereBeingManipulated;
  //  public event Action Died;

   public void BeingManipulated(ShowDarkStarPhase.DarkStarPhases typeOfManipulation)
    {
        if(WereBeingManipulated != null)
        {
            WereBeingManipulated(typeOfManipulation);
        }
    }

    public ShowDarkStarPhase.DarkStarPhases phaseWeNeedToMatch;


    public void SetNeededMatchPhase(ShowDarkStarPhase.DarkStarPhases typeOfManipulation)
    {
        phaseWeNeedToMatch = typeOfManipulation;
    }
    private void Awake()
    {
        ourEnemyType = GetComponent<SpaceMonster>();
        WereBeingManipulated += this.SetNeededMatchPhase;
        health = GetComponent<Health>();
         agent = GetComponent<GoapAgent>();
        ourSpaceMonster = GetComponent<SpaceMonster>();
        rb = GetComponent<Rigidbody2D>();
        stopDistance = 3.0f;
        snapSpeed = 10.0f; //scale with distance? 
        ourMovement = GetComponent<UniversalMovement>();

    }

    Rigidbody2D rb;
    float snapSpeed;
    float stopDistance;
    Health health;

    public DigestibleEntityType entityType { get ; set; }

    private int illuminationValue = 2;

    public int illuminationAdjustmentValue { get { return this.illuminationValue; } set { this.illuminationValue = value;  } }
    

    public void CancelPull()
    {
        hookBroken = true;
    }


    public void BeginPull(Transform target)
    {
        StartCoroutine(PullMeForward(target));
    }

    public IEnumerator PullMeForward(Transform target)
    {
        Debug.Log("The player is pulling us!");
        Vector2 zipLocation = target.position;
        Vector2 trans = zipLocation - (Vector2)transform.position;
        trans.Normalize();
        rb.bodyType = RigidbodyType2D.Kinematic;
        beingPulled = true;
        ourMovement.AddIncapacitationSource(target.gameObject);
        BeingManipulated(ShowDarkStarPhase.DarkStarPhases.pullPhase);
        //TODO: Take this out
        while (Mathf.Abs(Vector2.Distance(transform.position, target.position)) > stopDistance)
        {
            if (hookBroken)
            {
                 break;
            }
            rb.velocity = trans * snapSpeed;

            yield return null;
        }
        ourMovement.RemoveIncapacitationSource(target.gameObject);
        beingPulled = false;
        rb.velocity = new Vector2(0.0f, 0.0f);
       // rb.bodyType = RigidbodyType2D.Dynamic;
            
    }

    


   GoapAgent agent; 
	// Use this for initialization
	
    public void Deconstruct(GameObject source)
    {
        health.Die(ourEnemyType, source); 
    }
  

    // Update is called once per frame

}
