using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalMovement : MonoBehaviour
{


    GameStateHandler ourGameStateHandler;
    public float moveSpeed;
    public Rigidbody2D rb;
    public float horizontalSpeed;
    public float verticalSpeed;
    public bool jumpInitiated;
    public bool cantMove;

    public bool moving;
    public Vector3 movement;

    DarkStar darkStar;

    private void Awake()


    {
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
        ////Debug.Log(moveH);
        ////Debug.Log(moveV);

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
        if (distance < DarkStar.radius + 3.0f)
        {
            Vector2 direction = offset / distance;
            ourPosition = DarkStar.position + direction * (DarkStar.radius + 3.0f);
        }
        else
        {
            ourPosition = transform.position;
        }

        return ourPosition;
    }



    public void MoveToTarget(GameObject targetGO)
    {       
        Vector2 target = targetGO.transform.position;
         if(Vector2.Distance(transform.position, target) <= 5){
            rb.velocity = new Vector2(0, 0);
        }
        transform.position = LimitPosition_();
        Vector2 trans = GetTransition.GetTransitionDirection(transform.position, target);
        rb.AddForce(trans * moveSpeed);
      //  float step = moveSpeed * Time.deltaTime;
       //  gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, target, step);
       // Debug.Log(gameObject.name + " should be moving!");


    }

    public void MoveToVectorTarget(Vector2 target)
    {
        if(Vector2.Distance(transform.position, target) <= 5){
            rb.velocity = new Vector2(0, 0);
        }
        transform.position = LimitPosition_();
        Vector2 trans = GetTransition.GetTransitionDirection(transform.position, target);
        rb.AddForce(trans * moveSpeed);
      //  float step = moveSpeed * Time.deltaTime;
       //  gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, target, step);
    }

    private void Update()
    {
        transform.position = LimitPosition_();
        //if (moving)
        //{
        //    pReference.rb.AddForce(movement * pReference.speed);
        //}

    }


}
