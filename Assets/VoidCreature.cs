using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class VoidCreature : ParentTrigger {
    GameObject player;
    GameObject darkStar;
    PlayerMovement movement;

    SwapSprite ourSwapSprite;

    Vector2 darkStarPosition;

    float maxDistanceFromPlayer;
    float minDistanceToPlayer;

    float innerCircleRadius;
    float outerCircleRadius;

    public bool aggroed;
    public bool endGame;

    float speed;
    Transform mouth;
  //  public Action CurrentBehavior;
    public GameObject currentTarget;
    Rigidbody2D ourRigidbody;

    bool foundSomething;
    bool sawBurst;
    List<Vector2> burstLocations;

    public enum BehaviourStates
    {

        restrictedToVoid,
        searchingForLight
    }
    public BehaviourStates ourBehaviourState;

    private void Awake()
    {
    }

    public IEnumerator ChargeTowardBurst(Vector2 burstLocation)
    {
        sawBurst = false;
        while (!foundSomething)
        {

            if (sawBurst)
            {//another burst was sighted in the middle of looking for this one, so cancel if it's closer
                if (Vector2.Distance(transform.position, burstLocations.Last()) < Vector2.Distance(transform.position, burstLocation)){

                    StartCoroutine(ChargeTowardBurst(burstLocations.Last()));
                    yield break;
                    //basically, if this new burst is close to the last, head toward it. Else, keep looking until you reach the proximity of this one
                    //TODO: Finish this method
                }
            }
            Vector2 trans = burstLocation - (Vector2)transform.position;
            ourRigidbody.velocity = trans * speed;
            yield return null;
        }
    }
    void ReactToBurst(Vector2 burstLocation)
    {
        sawBurst = true;
        //add this to a list of bursts that the Void Monster recently saw.
        burstLocations.Add(burstLocation); 
        //a burst was sighted
        StartCoroutine(ChargeTowardBurst(burstLocation));
        

    }

    public BehaviourStates currentBehaviourState;

    //TODO: MAKE THIS THING REACT TO THE BURSTS OF LIGHT
	void Start () {

        darkStar = GameObject.Find("Dark Star");

        darkStarPosition = darkStar.transform.position;

        player = GameObject.FindWithTag("Player");

        movement = GetComponent<PlayerMovement>();
        mouth = transform.Find("Mouth");
        ourSwapSprite = GetComponentInChildren<SwapSprite>();

        innerCircleRadius = GameObject.Find("InnerCircle").GetComponent<CircleCollider2D>().bounds.extents.x;
        outerCircleRadius = GameObject.Find("Void-OuterCircle").GetComponent<CircleCollider2D>().bounds.extents.x;
      //  CurrentBehavior = LimitOutsidecircle();
	}
    public void ChangeCurrentTarget(GameObject newTarget)
    {
        currentTarget = newTarget;
    }

    void TargetPlayer()
    {
        currentTarget = player;
        AggroTowardIntruder();
    }

    void AggroTowardIntruder()
    {

        aggroed = true;
        ourSwapSprite.moving = true;
        ourSwapSprite.target = currentTarget;
        transform.position = FindTeleportLocation();
        StartCoroutine(StartAggroCharge(currentTarget));
    }

    private void OnEnable()
    {
        TheVoid.PlayerEnteredVoid += TargetPlayer;
        TheVoid.PlayerExitedVoid += LoseAggro;
        GameStateHandler.DoorOpened += BreakThroughBarrier; 
      //  DarkStar.IlluminationAtMax += BreakThroughBarrier;
    }

    private void OnDisable()
    {
        TheVoid.PlayerEnteredVoid -= TargetPlayer;
        TheVoid.PlayerExitedVoid -= LoseAggro;
        GameStateHandler.DoorOpened -= BreakThroughBarrier;
       // DarkStar.IlluminationAtMax -= BreakThroughBarrier;
        
    }

    

    void BreakThroughBarrier()
    {
        endGame = true;
        currentTarget = player;
        aggroed = true;
        ourSwapSprite.moving = true;
        ourSwapSprite.target = currentTarget;
        ourBehaviourState = BehaviourStates.searchingForLight;
        BurstBehaviour.BurstLightGoesOff += this.ReactToBurst;

       // StartCoroutine(StartAggroCharge(currentTarget));
    }

    void LoseAggro()
    {
        //Debug.Log("PlayerLostAggro");
        aggroed = false;
        currentTarget = null;
      
    }


    void RotateMe(GameObject target)
    {
        Vector2 vectorToTarget = target.transform.position - transform.position;
 
        var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        bool facingRight = transform.position.x > target.transform.position.x;

        if (ourSwapSprite.facingRight == true)
        {
            transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(180 - angle, transform.forward);
        }
        // projectileRotation = Quaternion.AngleAxis(angle, Vector3.forward);

    

}

    public IEnumerator StartAggroCharge(GameObject target)
    {
        while (Vector2.Distance(mouth.position, target.transform.position) > 1.0f)
        {
            // //Debug.Log(Vector2.Distance(mouth.position, target.transform.position));
        if (!aggroed)
        {
            yield break; 
        }
            RotateMe(target);
            float str = Mathf.Min(0.5f * Time.deltaTime, 1);
            transform.position = Vector2.Lerp(transform.position, target.transform.position, str);
            if(Vector2.Distance(mouth.position, target.transform.position) <= 1.0f)
            {
                break;
            }
            yield return null;

        }

    }


    

	// Use this for initialization


    void LimitOutsidecircle()
    {
        float distance = Vector2.Distance(darkStarPosition, transform.position);
        if ( distance <= innerCircleRadius)
        {
            Vector3 fromOriginToObject = (Vector2)((Vector2)transform.position - darkStarPosition);
            fromOriginToObject *= innerCircleRadius / distance;
            transform.position = (Vector3)darkStarPosition + fromOriginToObject; 
        }
    }

    Vector2 FindTeleportLocation()
    {
        //TODO: Fix some sort of issue here
        Vector2 potentialAttackLocation = UnityEngine.Random.insideUnitCircle.normalized * 10.0f + (Vector2)player.transform.position;

        if (Vector2.Distance(darkStarPosition, potentialAttackLocation) <= innerCircleRadius)
        {
            potentialAttackLocation = FindTeleportLocation();
        }
        return potentialAttackLocation;
    }


    Vector2 FindPatrolRoute()
    {
        Vector2 potentialLocation = UnityEngine.Random.insideUnitCircle + darkStarPosition;
        if(Vector2.Distance(darkStarPosition, potentialLocation) <= innerCircleRadius)
        {
            potentialLocation = FindPatrolRoute();
        }
        return potentialLocation;
    }

	
	// Update is called once per frame
	void Update () {

        if (!aggroed && !endGame)
        {
            LimitOutsidecircle();
        } 
		
	}

    public override void OnChildTriggerEnter2D(Collider2D hit, GameObject hitChild)
    {
       // //Debug.Log("Did this trigger? " + hit.gameObject.name);
        if(hit.gameObject == player)
        {
            player.GetComponent<PlayerHealth>().Die();
            ourSwapSprite.Swap();
            LoseAggro();
        }

    }

    public override void OnChildTriggerExit2D(Collider2D hit, GameObject hitChild)
    {


    }

}
