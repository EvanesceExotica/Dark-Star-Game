using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerMovement : UniversalMovement {

    GameObject objectWereZippingTowards;
    GameObject hook;
    public PlayerReferences pReference;
    bool zipping = false;
    public float snapSpeed;
   public bool onLocation;
    public static event Action MovementManipulated;
    public Quaternion defaultRotation;

    void MovementWasManipulated()
    {
        if(MovementManipulated != null)
        {
            MovementManipulated();
        }
    }

    public virtual void Awake()
    {
        pReference = GetComponent<PlayerReferences>();
        hook = GameObject.Find("Hook");
        defaultRotation = transform.rotation;
    //    objectWereZippingTowards = GameObject.Find("Hook");
        onLocation = false;
    }

    public virtual void Start()
    {
        pReference = GetComponent<PlayerReferences>();

        snapSpeed = 10.0f;
        StartCoroutine(ShowVelocityEveryHalfSecond());
    }

    // Use this for initialization


    // Use this for initialization


    public void IncraseDrag()
    {
        pReference.rb.drag = 0.5f;
    }

    void DecreaseDrag()
    {
        pReference.rb.drag = 0.0f;
    }

    public void SetRigidbodyDynamic()
    {
        pReference.rb.bodyType = RigidbodyType2D.Dynamic;
    }

    Vector3 hitPoint;

    private void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(hitPoint, 4.0f);
        DebugExtension.DrawPoint(pointOfHit, Color.cyan, 4.0f);
    }


    public IEnumerator ShowVelocityEveryHalfSecond()
    {
        Vector2 vel;
        float speed;
        while (true)
        {
            vel = pReference.rb.velocity;
            speed = vel.magnitude;
            // Debug.Log("Speed:" + speed);
           // Debug.Log("Force should be " + movement + " * " + pReference.speed + " = " + (movement * pReference.speed) + " or " + (movement * pReference.speed).magnitude + " Velocity is " + pReference.rb.velocity + " or " + pReference.rb.velocity.magnitude);       
            yield return new WaitForSeconds(0.5f);
      
        }
    }

    private void Update()
    {
        
        var vel = pReference.rb.velocity;
        var speed = vel.magnitude;
        //Debug.Log("Our speed is: " + snapSpeed); 
        if (onLocation)
        {
            //Debug.Log("On Location!");
        }
        //if (moving)
        //{
        //    pReference.rb.AddForce(movement * pReference.speed);
        //}

    }

    public void MoveToHookshot(Vector2 hit, GameObject hookedObject)
    {
        MovementWasManipulated();
        StartCoroutine(ZipToHookshot(hit, hookedObject));
    }

    public void MoveToObject(Vector2 target)
    {
        MovementWasManipulated();
        StartCoroutine(ZipToObject(target));
    }

    public IEnumerator ZipToObject(Vector2 target)
    {
        zipping = true;
        Vector2 zipLocation = target;
        hitPoint = zipLocation;
        Vector2 trans = zipLocation - (Vector2)transform.position;
        trans.Normalize();
        //  pReference.rb.bodyType = RigidbodyType2D.Kinematic;

        while (!onLocation)
        {

            if(Vector2.Distance(transform.position, target) <= 0.4f)
            {
                onLocation = true;
                transform.position = target;
            }
       
            Debug.DrawRay(transform.position, trans * 10f, Color.blue);
            pReference.rb.velocity = trans * snapSpeed/2;

            yield return null;
        }
        pReference.rb.velocity = new Vector2(0.0f, 0.0f);
        zipping = false;
        onLocation = false;
    }
    public IEnumerator ZipToHookshot(Vector2 hit, GameObject hookedObject)
    {
        zipping = true;
        Vector2 zipLocation = hit;
        hitPoint = zipLocation;
        Vector2 trans = zipLocation - (Vector2)transform.position;
        trans.Normalize();
        objectWereZippingTowards = hookedObject;
        //  pReference.rb.bodyType = RigidbodyType2D.Kinematic;

        while (!onLocation)
        {
           // Debug.Log("<color=orange>We're still zipping on our hookshot</color>");
            if(Vector2.Distance(pReference.locationHandler.groundCheck.transform.position, hit) <= 0.4f || Vector2.Distance(pReference.ceilingCheck.transform.position, hit) <= 0.4f)
            {
                onLocation = true;
                transform.position = hit;
            }
        
            Debug.DrawRay(transform.position, trans * 10f, Color.blue);
            pReference.rb.velocity = trans * snapSpeed;

            yield return null;
        }
        //transform.position = hit;
        //v = speed/time 
        //v * time = speed
        //time = speed/velocity
        float time = snapSpeed / pReference.rb.velocity.magnitude;
        //Debug.Log("Here is our time " +  time);
        StartCoroutine(LerpTheRestOfTheWay(hit, time));
        pReference.rb.velocity = new Vector2(0.0f, 0.0f);
        onLocation = false;
        zipping = false;
        //   transform.parent = hit.collider.transform;
    }

    Vector3 pointOfHit;

    

    private void OnTriggerEnter2D(Collider2D hit)
    {

     
        if (hit.gameObject == hook && zipping)
        {
         //   Debug.Log(gameObject.name + "  We've hit the hook");
         //   pointOfHit = hit2.point;
            onLocation = true;
        }
    }

    IEnumerator LerpTheRestOfTheWay(Vector2 pointToLerpTo, float timeToTakeToLerp)
    {
        
        float elapsedTime = 0;
        Vector2 startingPosition = transform.position;
        while(elapsedTime < timeToTakeToLerp)
        {
            //Debug.Log("We are in fact lerping toward the target");
            transform.position = Vector2.Lerp(startingPosition, pointToLerpTo, (elapsedTime / timeToTakeToLerp));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (zipping)
        {
            
            //Debug.Log(objectWereZippingTowards.name);
        }
         if(collision.gameObject == objectWereZippingTowards)
        {
            onLocation = true;
            //Debug.Log("OnLocation");
                
        }
    }





}
