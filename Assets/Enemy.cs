using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Vector2 zipLocation = target.position;
        Vector2 trans = zipLocation - (Vector2)transform.position;
        trans.Normalize();
        rb.bodyType = RigidbodyType2D.Kinematic;
        beingPulled = true;
        BeingManipulated(ShowDarkStarPhase.DarkStarPhases.pullPhase);
        while (Mathf.Abs(Vector2.Distance(transform.position, target.position)) > stopDistance)
        {
            if (hookBroken)
            {
                 break;
            }
            rb.velocity = trans * snapSpeed;

            yield return null;
        }
        beingPulled = false;
        rb.velocity = new Vector2(0.0f, 0.0f);
       // rb.bodyType = RigidbodyType2D.Dynamic;
            
    }


    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        stopDistance = 3.0f;
        snapSpeed = 10.0f; //scale with distance? 
        ourMovement = GetComponent<UniversalMovement>();
       
        
	}

    public void Deconstruct()
    {
        health.Die(ourEnemyType); 
    }
  

    // Update is called once per frame
    void Update () {
		
	}
}
