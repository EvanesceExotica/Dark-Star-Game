using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UniversalMovement : MonoBehaviour
{

    bool bornFromStar;

    public bool BornFromStar
    {
        get
        {
            return bornFromStar;
        }
        set
        {
            bornFromStar = value;
        }
    }
    GameStateHandler ourGameStateHandler;
    public float moveSpeed;
    public Rigidbody2D rb;
    public float horizontalSpeed;
    public float verticalSpeed;
    public bool jumpInitiated;
    public bool incapacitated;

    public bool moving;
    public Vector3 movement;

    DarkStar darkStar;


    public event Action<GameObject> SomethingImpededOurMovement;

    public event Action<GameObject> SomethingStoppedImpedingOurMovement;

    public event Action NothingImpedingOurMovement;

    public List<GameObject> incapacitationSources = new List<GameObject>();
    public List<IncapacitationType> ourTypesOfIncapacitation = new List<IncapacitationType>();
    public enum IncapacitationType
    {
        Frozen,
        Pulled,
        BeingDevoured
    }
    public void RemoveIncapacitationSource(GameObject incapacitator, IncapacitationType incapacitationType)
    {
        incapacitationSources.Remove(incapacitator);
        ourTypesOfIncapacitation.Remove(incapacitationType);
        //TODO: Not sure if it's okay to put the Event Action inside this if statement -- might want to have an event trigger regardless of whether or not nothing's impeding
        //TODO: Should I add a second event to capture that behaviour?
        if (incapacitationSources.Count == 0)
        {
            incapacitated = false;
            //TODO: "I changed this from "Something stopped impeding our movement" to "nothing's impeding our movement". May need a second method to account for any time something stops.
            if (SomethingStoppedImpedingOurMovement != null)
            {
                SomethingStoppedImpedingOurMovement(incapacitator);
            }
            if (NothingImpedingOurMovement != null)
            {
                NothingImpedingOurMovement();
            }
        }
    }



    public void AddIncapacitationSource(GameObject incapacitator, IncapacitationType incapacitationType)
    {
        incapacitationSources.Add(incapacitator);
        ourTypesOfIncapacitation.Add(incapacitationType);
        incapacitated = true;
        if (SomethingImpededOurMovement != null)
        {
            SomethingImpededOurMovement(incapacitator);
        }
    }


    private void Awake()


    {
        moveSpeed = 5.0f;
        stunDuration = 4.0f;
        ourGameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        darkStar = GameObject.Find("Dark Star").GetComponent<DarkStar>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }


    // Use this for initialization


    // Use this for initialization
    void Start()
    {
    }


    public virtual void Move(float moveH, float moveV, bool jump)
    {

        if (moveH != 0 || moveV != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        movement = new Vector3(moveH, moveV, 0.0f);
        //  pReference.rb.AddForce(movement * pReference.speed);




    }

    Vector2 LimitPosition_()
    {
        Vector2 ourPosition = transform.position;
        //TODO: The radius is only changed after the star finishes growing, you might want to change this later. 

        Vector2 center = ourGameStateHandler.darkStar.transform.position;
        Vector2 offset = (Vector2)transform.position - center;
        float distance = offset.magnitude;
        if (distance < DarkStar.radius + 1.0f)
        {
            Vector2 direction = offset / distance;
            ourPosition = DarkStar.position + direction * (DarkStar.radius + 3.0f);
        }
        else if (distance > GameStateHandler.voidBoundaryRadius - 3)
        {
            Vector2 direction = offset / distance;
            ourPosition = DarkStar.position + direction * GameStateHandler.voidBoundaryRadius;
        }
        else
        {
            ourPosition = transform.position;
        }

        return ourPosition;
    }

    public void Stun(GameObject incapacitator)
    {

        StartCoroutine(StartStun(incapacitator));
    }

    float stunDuration;
    IEnumerator StartStun(GameObject incapacitator)
    {
        AddIncapacitationSource(incapacitator, IncapacitationType.Frozen);
        rb.velocity = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(stunDuration);
        RemoveIncapacitationSource(incapacitator, IncapacitationType.Frozen);
    }
    public void HitBySomething()
    {
        //TODO: MAke sure this is differentiating between the hook and the players frozenness
        if (incapacitationSources.Contains(GameStateHandler.player))
        {
            //if frozen by the player
            BurstAndDropSoul();
        }
    }

    public void BurstAndDropSoul()
    {
        //TODO: Make the  

    }

    public void KnockBack(Collision2D col, float force, Vector2 direction)
    {
        Debug.Log("Blue Dwarf's velocity! " + rb.velocity);
        //TODO: Figure out why the player isn't being knocked back
        Vector2 impactPoint = col.contacts[0].point;
        //TODO: Play some sort of particle effect;
        //rb.AddForce(direction * 500f);
        rb.velocity = direction * force;
        Debug.Log("Direction " + direction + " Force " + force);
        Debug.Log("Blue dwarf's velocity now! " + rb.velocity);
        //direction = -direction.normalized;
        //rb.velocity = direction * force;
    }

    public bool Vector2Equal(Vector2 a, Vector2 b)
    {
        return Vector2.SqrMagnitude(a - b) < 0.1;
    }

    public void MoveToTarget(GameObject targetGO)
    {
        Vector2 target = targetGO.transform.position;
        if (!moving)
        {
            moving = true;
        }
        if (Vector2Equal(transform.position, target))
        {
            Debug.Log("WE'VE REACHED OUR TARGET");
            moving = false;
            rb.velocity = new Vector2(0, 0);
        }


        Vector2 trans = GetTransition.GetTransitionDirection(transform.position, target);
        if (!incapacitated)
        {
            rb.AddForce(trans * moveSpeed);
        }
        //  float step = moveSpeed * Time.deltaTime;
        //  gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, target, step);
        // Debug.Log(gameObject.name + " should be moving!");


    }

    public void MoveToVectorTarget(Vector2 target)
    {
        if (!moving)
        {
            moving = true;
        }
        if (Vector2Equal(transform.position, target))
        {
            Debug.Log("WE'VE REACHED OUR TARGET");
            moving = false;
            rb.velocity = new Vector2(0, 0);
        }
       

        Vector2 trans = GetTransition.GetTransitionDirection(transform.position, target);

        if (!incapacitated)
        {
            rb.AddForce(trans * moveSpeed);
        }
        //  float step = moveSpeed * Time.deltaTime;
        //  gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, target, step);
    }

    private void Update()
    {
        if (moving && !bornFromStar)
        {

            transform.position = LimitPosition_();
        }
        //if (moving)
        //{
        //    pReference.rb.AddForce(movement * pReference.speed);
        //}

    }


}
