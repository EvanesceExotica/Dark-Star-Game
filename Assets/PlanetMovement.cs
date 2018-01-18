using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlanetMovement : PlayerMovement
{

    // bool moving;
    public float movementRight;
    public float movementLeft;
    public LayerMask WhatIsGround;
    public bool grounded;
    public bool jumping;
    public bool falling;

    bool inputJump;
    float maxJumpHeight;
    float jumpDuration;

    float gravityConstant = 9.8f;


    public bool fallingToPlanet;

    //public float area;

    public List<GameObject> inOrbitOfThesePlanets;

   


    public Vector3 stickPoint;
    public Vector3 stickNormal;

    public RaycastHit2D left;
    public RaycastHit2D right;

    AdhereToPlanet planetAdhering;

    public Transform LeftAlign;
    public Transform RightAlign;

    public LayerMask whatIsGround;

    Vector2 averagePlanetNormal;
    Vector2 locationPoint;

    [SerializeField]
    public bool cantOrient;

    float jumpStartTime;

    float maxSpeed;

    void SetVelocityToZero(GameObject go)
    {
        //Debug.Log("Now that we're landing on planet, velocity set to zero");
        pReference.rb.velocity = Vector3.zero;
        pReference.rb.angularVelocity = 0.0f;
    }

    public override void Awake()
    {
        pReference = GetComponent<PlayerReferences>();
        pReference.locationHandler.TouchedOnPlanet += this.SetVelocityToZero;
    }

    public override void Start()
    {
        maxSpeed = 5.0f;
        maxJumpHeight = 5.0f;
        jumpDuration = 3.0f;
        snapSpeed = 10.0f;
      //  pReference.locationHandler = GetComponent<LocationHandler>(); 

     //   area = GetComponent<SpriteRenderer>().bounds.size.x * GetComponent<SpriteRenderer>().bounds.size.y;
    }
    void OnEnable()
    {
       // transform.parent = pReference.locationHandler.closestPlanet.transform;
        //pReference.rb.velocity = new Vector2(0, 0);
        //pReference.rb.isKinematic = true;
        pReference.rb.bodyType = RigidbodyType2D.Kinematic;
      //  pReference.rb.gravityScale = 1;
       // pReference.trailRenderer.enabled = false;
     //   pReference.rb.drag = 1.0f;
        Hookshot.ObjectHooked += this.DisableOrientation;

  
    }




    private void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(locationPoint);

        DebugExtension.DrawPoint(testPoint, Color.red, 1.0f);
      //  DebugExtension.DrawPoint(higherTestPoint, Color.green, 1.0f);
        DebugExtension.DrawCircle(center, Vector3.forward, Color.yellow, radius_);
    }


    void DisableOrientation(GameObject hookedObj)
    {
        if(hookedObj.GetComponent<IHookable>() != null)
        {
           // //Debug.Log("This is happening");

            cantOrient = true;
            transform.parent = null;
        }
    }

    Vector2 center;
    float radius_;

    Vector2 rightPosition;
    Vector2 leftPosition;


    bool doubleRaycastDown()
    {
        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;
        float distanceToCenter = 0.53f; // Vector2.Distance(LeftAlign.position, transform.position);

        leftPosition = transform.position + 0.5f * transform.up + distanceToCenter * transform.right;

        rightPosition = transform.position + 0.5f * transform.up - distanceToCenter * transform.right;

        RaycastHit2D leftHit = Physics2D.Raycast(leftPosition, -transformUp, 5.0f, pReference.locationHandler.whatIsPlanet);
        left = leftHit;
        Debug.DrawRay(leftPosition, -transform.up * 10, Color.green);
        Debug.DrawRay(transform.position, leftHit.normal * 10, Color.magenta);

        RaycastHit2D rightHit = Physics2D.Raycast(rightPosition, -transformUp, 5.0f, pReference.locationHandler.whatIsPlanet);
        right = rightHit;
        Debug.DrawRay(transform.position, rightHit.normal * 10, Color.yellow);
        Debug.DrawRay(rightPosition, -transform.up * 10, Color.red);

        return right && left;

    }

    void PositionOnTerrain()
    {

    }

    void Orientatate()
    {
        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;
        float distanceToCenter = 0.53f; // Vector2.Distance(LeftAlign.position, transform.position);


        leftPosition = transform.position + 0.5f * transform.up + distanceToCenter * transform.right;

        rightPosition = transform.position + 0.5f * transform.up - distanceToCenter * transform.right;

        RaycastHit2D leftHit = Physics2D.Raycast(leftPosition, -transformUp, 5.0f, pReference.locationHandler.whatIsPlanet);
        left = leftHit;
        Debug.DrawRay(leftPosition, -transform.up * 10, Color.green);
        Debug.DrawRay(transform.position, leftHit.normal * 10, Color.magenta);

        RaycastHit2D rightHit = Physics2D.Raycast(rightPosition, -transformUp, 5.0f, pReference.locationHandler.whatIsPlanet);
        right = rightHit;
        Debug.DrawRay(transform.position, rightHit.normal * 10, Color.yellow);
        Debug.DrawRay(rightPosition, -transform.up * 10, Color.red);

     

        if (leftHit && rightHit)
        {
            Debug.Log("LEFT AND RIGHT HIT DOING STUFF CHECK");
            if (pReference.rb.velocity.sqrMagnitude > 0)
            {
                if (pReference.rb.velocity.x < 0)
                {
                    RaycastHit2D overrideLeftHit = Physics2D.Raycast(transform.position + 2.0f * transformUp, -transformRight, 5.0f, pReference.locationHandler.whatIsPlanet);

                    if (overrideLeftHit)
                    {
                        leftHit = overrideLeftHit;
                    }
                }
                else
                {
                    RaycastHit2D overrideRightHit = Physics2D.Raycast(transform.position + 2.0f * transformUp, -transformRight, 5.0f, pReference.locationHandler.whatIsPlanet);

                    if (overrideRightHit)
                    {
                        rightHit = overrideRightHit;
                    }
                }
            }





            Vector2 preciseOrientation = (leftHit.normal + rightHit.normal) / 2;
            averagePlanetNormal = preciseOrientation;
            Debug.DrawRay(transform.position, averagePlanetNormal * 30f, Color.magenta);
            Debug.DrawRay(transform.position, preciseOrientation * 10, Color.cyan);
            Vector2 precisePoint = (leftHit.point + rightHit.point) / 2;

            locationPoint = precisePoint;

            //TODO: Change this back to Vector3.up instead of transform.up
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, preciseOrientation);

            float speed = 200.0f; 
            Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);
            float offset = Vector2.Distance(locationPoint, transform.position);

            testPoint = locationPoint;
           // testPoint = (Vector3)locationPoint + transformUp * offset;

            if (!jumping && !cantOrient )
            {
                Debug.Log("We're orientating");
                testPoint = (Vector3)locationPoint + transformUp * 0.9f;
                transform.position = (Vector3)locationPoint + transformUp * 0.9f;
             //   //Debug.Log("We're orientating!" + locationPoint);
            }
           


        }



    }


    void OrientationTest()
    {
        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;
        float distanceToCenter = Vector3.Distance(LeftAlign.position, transform.position);


        RaycastHit2D leftHit = Physics2D.Raycast(transform.position + 2.0f * transformUp + distanceToCenter * transformRight, -transformUp, 5.0f, whatIsGround);
        left = leftHit;
        Debug.DrawRay(LeftAlign.position, -transform.up * 10, Color.green);
        Debug.DrawRay(transform.position, leftHit.normal * 10, Color.magenta);

        RaycastHit2D rightHit = Physics2D.Raycast(transform.position + 2.0f * transformUp - distanceToCenter * transformRight, -transformUp, 5.0f, whatIsGround);
        right = rightHit;
        Debug.DrawRay(transform.position, rightHit.normal * 10, Color.yellow);
        Debug.DrawRay(RightAlign.position, -transform.up * 10, Color.red);

        if (leftHit && rightHit)
        {

            if (pReference.rb.velocity.sqrMagnitude > 0)
            {
                if (pReference.rb.velocity.x < 0)
                {
                    RaycastHit2D overrideLeftHit = Physics2D.Raycast(transform.position + 2.0f * transformUp, -transformRight, 1.0f, pReference.locationHandler.whatIsPlanet);

                    if (overrideLeftHit)
                    {
                        leftHit = overrideLeftHit;
                    }
                }
                else
                {
                    RaycastHit2D overrideRightHit = Physics2D.Raycast(transform.position + 2.0f * transformUp, -transformRight, 1.0f, pReference.locationHandler.whatIsPlanet);

                    if (overrideRightHit)
                    {
                        rightHit = overrideRightHit;
                    }
                }
            }


            // currentPlanet = leftHit.collider.gameObject;
            //  planet = leftHit.collider.gameObject.transform;


            Vector2 preciseOrientation = (leftHit.normal + rightHit.normal) / 2;
            averagePlanetNormal = preciseOrientation;

            Debug.DrawRay(transform.position, preciseOrientation * 10, Color.cyan);
            Vector2 precisePoint = (leftHit.point + rightHit.point) / 2;

            locationPoint = precisePoint;

            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, preciseOrientation);
            Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200.0f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);
            if (!cantOrient && !jumping)
            {
                transform.position = (Vector3)precisePoint + transform.up * 0.2f;
            }

            //...
            //            if (currentPlanet.GetComponent<PlanetaryBody>().isSpinning) { 

            //}

        }



    }

    public void Update()
    {
        if (moving)
        {
            if(movementRight != 0)
            {

                transform.Translate(Vector3.right * maxSpeed * Time.deltaTime);
            }
            if(movementLeft != 0)
            {
     
                transform.Translate(-Vector3.right * maxSpeed * Time.deltaTime);
            }
        }
        //OrientationTest();
            Orientatate();
        

        Debug.DrawRay(transform.position, transform.right * 50, Color.white);
        Debug.DrawRay(transform.position, transform.up * 10, Color.yellow);
        Debug.DrawRay(transform.position, Vector3.up * 50, Color.cyan);
        //base.Update();

        if (Time.time > jumpStartTime + jumpDuration)
        {
            jumping = false;
        }



    }

    

    public IEnumerator WaitForFall()
    {
        //Debug.Log("Waiting for fall");
        yield return new WaitForSeconds(1.0f);
        fallingToPlanet = true;
    }



  

    public override void Move(float moveH, float moveV, bool jump)
    {
        if(moveH == 0 && moveV == 0 && !jump)
        {
            movementLeft = 0;
            movementRight = 0;
            moving = false;
        }
        if (moveH > 0)
        {
            moving = true;
            movementRight = moveH;
            pReference.rb.AddForce(transform.right * maxSpeed);
            transform.Translate(Vector3.right * maxSpeed * Time.deltaTime);
            pReference.ourSpriteRenderer.flipX = false;
     
        }
        else
        {
            movementRight = 0;
        }
        if (moveH < 0)
        {
            moving = true;
            transform.Translate(-Vector3.right * maxSpeed * Time.deltaTime);
            movementLeft = moveH;
            pReference.rb.AddForce(-transform.right * maxSpeed);
            pReference.ourSpriteRenderer.flipX = true; 
        }
        else
        {
            movementLeft = 0;
        }
        if (pReference.locationHandler.onPlanet && jump)
        {
            fallingToPlanet = false;
            jumpStartTime = Time.time;
      
            jumping = true;
            pReference.locationHandler.PlanetLiftedOffFrom(pReference.locationHandler.closestPlanet);
            StartCoroutine(WaitForFall());
            pReference.rb.isKinematic = false;

            pReference.rb.AddForce(transform.up * 200f);
        }


        if (pReference.locationHandler.onPlanet && fallingToPlanet)
        {
           // pReference.rb.isKinematic = true;
            fallingToPlanet = false;
            jumping = false;

        }

      


        else if (pReference.locationHandler.onPlanet && !jump && !jumping /* && !pReference.starBash.nesting && !pReference.starBash.bashing*/)
        {
     

        }
        else if (!pReference.locationHandler.onPlanet /*&& !planetAdhering.sticking*/)
        {
           // pReference.rb.isKinematic = false;
        }
        else if (!pReference.locationHandler.onPlanet && pReference.locationHandler.inOrbit)
        {

        }


    }

    public Vector2 testPoint;
    public Vector2 higherTestPoint;

   

 
    private void FixedUpdate()
    {

    }


    void OnDisable()
    {
        pReference.rb.bodyType = RigidbodyType2D.Dynamic;
      //  pReference.rb.gravityScale = 0;
   //     pReference.trailRenderer.enabled = false;
        pReference.rb.drag = 0.0f;
       
        cantOrient = false;
    }
    

}
