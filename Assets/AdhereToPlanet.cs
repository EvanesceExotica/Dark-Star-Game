using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AdhereToPlanet : MonoBehaviour {
    

    //public List<GameObject> interPlanetaryLines = new List<GameObject>();

    //public Vector3 locationPoint;

    //public float duration;

    //public float stickStartTime;
    //public float stickDuration;
    //public bool sticking;

    //public bool SwitchingPlanets;
    //public GameObject groundCheck;
    //public LayerMask ignoreCurrent;

    //public Transform ourTransform;
    //public Vector3 myNormal;
    //public LayerMask whatIsGround;

    //public Transform LeftAlign;
    //public Transform RightAlign;

    //public bool flying;
    //Vector3 currentUp;
    ////  Vector3 right;

    //public GameObject oldPlanet;
    //public GameObject newPlanet;
    //public GameObject currentPlanet;
    //public Rigidbody2D currentPlanetRigidbody;

    //public Vector3 stickPoint;
    //public Vector3 stickNormal;

    //public RaycastHit2D left;
    //public RaycastHit2D right;

    //public float gravityConstant = 9.8f;

    //public PlanetMovement planetMovement;

    //public Vector2 averagePlanetNormal;

    //public GameObject linePrefab;

    //LocationHandler lh;


    //// Use this for initialization
    //void Start()
    //{
    //    currentPlanet = lh.currentPlanet;

    //}
    //void Awake()
    //{

    //    myNormal = Vector3.up;
    //    ourTransform = gameObject.GetComponent<Transform>();
    //    currentUp = myNormal;
    //    //right = transform.right;
    //    planetMovement = lh.planetMovement;
    //    duration = 2.0f;
    //}


  

    //void Update()
    //{
    //    if (gameObject == GameStateHandler.player)
    //    {
    //        DrawPlanetaryLines();
    //    }
        

        

    //}

    //void FixedUpdate()
    //{


    //    if (Time.time > stickStartTime + stickDuration)
    //    {
    //        sticking = false;
    //    }
    //    currentPlanet = lh.currentPlanet;
    //    planetMovement.currentPlanet = currentPlanet;

    //    Debug.DrawRay(transform.position, transform.right * 50, Color.white);
    //    Debug.DrawRay(transform.position, transform.up * 50, Color.yellow);


    //    OrientationTest();

    //    if (planetMovement.jumping && unit.lh.inHowManyOrbits > 1)
    //    {
    //        if (!SwitchingPlanets)
    //        {
    //            StartCoroutine(SwitchBodies());
    //        }

    //    }
    //    else
    //    {
    //    }



    //    if (currentPlanet.GetComponent<PlanetaryBody>().isSpinning && !planetMovement.jumping)
    //    {
    //        GameStateHandler.instance.playerRigidbody.MovePosition(transform.position - transform.right * (planetMovement.maxSpeed / 2) * Time.deltaTime);
    //    }
    //    if (lh.inHowManyOrbits > 1)
    //    {
    //        foreach (GameObject go in lh.planetsInOrbitOf)
    //        {
    //            if (go != lh.currentPlanet)
    //            {
    //                go.GetComponentInChildren<PointEffector2D>().enabled = false;
    //            }
    //        }
    //    }


    //}


    //void DrawPlanetaryLines()
    //{
    //    if (lh.inHowManyOrbits > 1)
    //    {

    //        foreach (GameObject planet in lh.planetsInOrbitOf)
    //        {
    //            if (planet != currentPlanet && !SwitchingPlanets)
    //            {
    //                Vector3 direction = planet.transform.position - this.transform.position;
    //                direction.Normalize();
    //                RaycastHit2D newHit = Physics2D.Raycast(transform.position, direction, 50.0f, ignoreCurrent);
    //                //Cast a ray between every planet and the player that the player is in orbit of

    //                LineRenderer holdLineRenderer = planet.GetComponent<PlanetaryBody>().ourLinerenderer;
    //                holdLineRenderer.enabled = true;
    //                holdLineRenderer.SetPosition(0, transform.position);
    //                holdLineRenderer.SetPosition(1, newHit.point);
    //                holdLineRenderer.startWidth = 0.5f;
    //                holdLineRenderer.endWidth = 0.5f;

    //                if (lh.inHowManyOrbits > 2)
    //                {
    //                    GameObject nearestPlanet = FindClosest.FindClosestObject(lh.planetsInOrbitOf, this.gameObject);
    //                    if (nearestPlanet == planet)
    //                    {
    //                        holdLineRenderer.startColor = Color.green;
    //                        holdLineRenderer.endColor = Color.blue;
    //                        // holdLineRenderer.SetColors(Color.green, Color.blue);
    //                        stickPoint = newHit.point;
    //                        stickNormal = newHit.normal;
    //                        Debug.DrawRay(transform.position, direction * 10, Color.blue);

    //                    }
    //                    else
    //                    {
    //                        holdLineRenderer.startColor = Color.gray;
    //                        holdLineRenderer.endColor = Color.gray;
    //                    }
    //                }
    //                else
    //                {
    //                    holdLineRenderer.startColor = Color.green;
    //                    holdLineRenderer.endColor = Color.blue;
    //                    stickPoint = newHit.point;
    //                    stickNormal = newHit.normal;
    //                    Debug.DrawRay(transform.position, direction * 10, Color.magenta);


    //                }




    //            }
    //            else if (SwitchingPlanets)
    //            {
    //                LineRenderer holdLineRenderer = planet.GetComponent<PlanetaryBody>().ourLinerenderer;
    //                holdLineRenderer.enabled = false;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}





    //void OrientationTest()
    //{
    //    Vector3 transformUp = transform.up;
    //    Vector3 transformRight = transform.right;
    //    float distanceToCenter = Vector3.Distance(LeftAlign.position, transform.position);


    //    RaycastHit2D leftHit = Physics2D.Raycast(transform.position + 2.0f * transformUp + distanceToCenter * transformRight, -transformUp, 5.0f, whatIsGround);
    //    left = leftHit;
    //    Debug.DrawRay(LeftAlign.position, -transform.up * 10, Color.green);
    //    Debug.DrawRay(transform.position, leftHit.normal * 10, Color.magenta);

    //    RaycastHit2D rightHit = Physics2D.Raycast(transform.position + 2.0f * transformUp - distanceToCenter * transformRight, -transformUp, 5.0f, whatIsGround);
    //    right = rightHit;
    //    Debug.DrawRay(transform.position, rightHit.normal * 10, Color.yellow);
    //    Debug.DrawRay(RightAlign.position, -transform.up * 10, Color.red);

    //    if (leftHit && rightHit)
    //    {

    //        if (GameStateHandler.instance.playerRigidbody.velocity.sqrMagnitude > 0)
    //        {
    //            if (GameStateHandler.instance.playerRigidbody.velocity.x < 0)
    //            {
    //                RaycastHit2D overrideLeftHit = Physics2D.Raycast(transform.position + 2.0f * transformUp, -transformRight, 1.0f, lh.whatIsPlanet);

    //                if (overrideLeftHit)
    //                {
    //                    leftHit = overrideLeftHit;
    //                }
    //            }
    //            else
    //            {
    //                RaycastHit2D overrideRightHit = Physics2D.Raycast(transform.position + 2.0f * transformUp, -transformRight, 1.0f, lh.whatIsPlanet);

    //                if (overrideRightHit)
    //                {
    //                    rightHit = overrideRightHit;
    //                }
    //            }
    //        }


          


    //        Vector2 preciseOrientation = (leftHit.normal + rightHit.normal) / 2;
    //        averagePlanetNormal = preciseOrientation;

    //        Debug.DrawRay(transform.position, preciseOrientation * 10, Color.cyan);
    //        Vector2 precisePoint = (leftHit.point + rightHit.point) / 2;

    //        locationPoint = precisePoint;

    //        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, preciseOrientation);
    //        Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 50.0f * Time.deltaTime);
    //        transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);
    //        if (!planetMovement.jumping)
    //        {
    //            transform.position = (Vector3)precisePoint + transform.up * 0.2f;
    //        }

          

    //    }



    //}


    //public IEnumerator SwitchBodies()
    //{
    //    SwitchingPlanets = true;
    //    float startTime = Time.time;

    //    oldPlanet = unit.lh.currentPlanet;
    //    oldPlanet.layer = 24;
    //    oldPlanet.GetComponentInChildren<PointEffector2D>().enabled = false;

    //    unit.rb.gravityScale = 0;

    //    //float distance = Vector2.Distance(transform.position, stickPoint);
    //    //float time = distance/unit.rb.velocity.y;
    //    //planetMovement.jumpDuration = time; 

    //    while (Time.time < startTime + duration)
    //    {
    //        //if (planetMovement.onPlanet && currentPlanet != oldPlanet)
    //        //{
    //        //    yield break;
    //        //}
    //        Quaternion newTargetRotation = Quaternion.FromToRotation(Vector3.up, stickNormal);
    //        Quaternion newFinalRotation = Quaternion.RotateTowards(transform.rotation, newTargetRotation, 200f * (Time.time - startTime) / duration);
    //        transform.rotation = Quaternion.Euler(0, 0, newFinalRotation.eulerAngles.z);
    //        transform.position = Vector2.Lerp(transform.position, stickPoint, (Time.time - startTime) / duration);
    //        yield return null;

    //    }
    //    unit.rb.velocity = new Vector2(0, 0);
    //    SwitchingPlanets = false;
    //    planetMovement.jumping = false;
    //    unit.rb.isKinematic = true;
    //    //currentPlanet = newPlanet;
    //    currentPlanet.GetComponentInChildren<PointEffector2D>().enabled = true;
    //    unit.rb.gravityScale = 1;
    //    stickStartTime = Time.time;
    //    sticking = true;
    //    oldPlanet.layer = 23;
    //    oldPlanet = null;
    //}




}


