
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarBash : MonoBehaviour
{
    Vector3 ourUp;
    public int planetsBashed;
    //add screensshake to this
    bool canBash;
    public bool outOfPlanet;

    public float bashStartTime;
    public float bashDuration;

    public bool bashing;
    public bool flying;


    public Vector2 bashNormal;
    public Vector2 bashSpot;
    public PlanetMovement planetMovement;
    //    public Transform core;
    public GameObject bashablePlanet;
    public Rigidbody2D ourRigidbody;
    public LayerMask whatIsPlanet;
    public LayerMask newPlanetsOnly;

    public float holdX;
    public Animator anim;
    public float eruptionTime;
    public Transform core;
    public Transform coreLocation;
    public bool nesting;

    public int incoporealLayer = 27;
    public int playerLayer = 13;
    public int oldPlanetLayer = 24;
    public int planetLayer = 23;

    public float nestingStartTime;
    public float nestingDuration;
    public GameObject lastPlanet;
    public LayerMask oldPlanetOnly;
    public LocationHandler ourLocationHandler;

    //  public Vector2 outPosition;
   // PointEffector2D ourPlanetPointEffector;

    void CancelMovement()
    {
        bashing = false;
    }




    void SetPlanet(GameObject currentPlanet)
    {
        if (bashablePlanet != currentPlanet)
        {
            bashablePlanet = currentPlanet;
            core = bashablePlanet.transform.Find("Core");
        }
    }

    void RemovePlanet(GameObject currentPlanet)
    {
        if (bashablePlanet == currentPlanet)
        {
            bashablePlanet = null;
        }
    }


    private void OnEnable()
    {
        ourLocationHandler.LiftedFromPlanet += this.RemovePlanet;
        ourLocationHandler.TouchedOnPlanet += this.SetPlanet;
    }

    private void OnDisable()
    {
        ourLocationHandler.LiftedFromPlanet -= this.RemovePlanet;
        ourLocationHandler.TouchedOnPlanet -= this.SetPlanet;
    }

    void Awake()
    {
        bashDuration = 4.0f;
        nestingDuration = 2.0f;
        ourLocationHandler = GetComponent<LocationHandler>();
        ourRigidbody = GetComponent<Rigidbody2D>();
        ourUp = -Vector3.up;
        outOfPlanet = true;
        pReference = GetComponent<PlayerReferences>();
        planetMovement = GetComponent<PlanetMovement>();
        PlayerMovement.MovementManipulated += this.CancelMovement;
        pReference = GetComponent<PlayerReferences>();

    }
    void Start()
    {



    }




    void Update()
    {




        if (!bashing && !nesting)
        {
            if (ourLocationHandler.onPlanet)
            {
                if (ourLocationHandler.currentPlanet != null)
                {
                    //change this to be event activated
                    bashablePlanet = ourLocationHandler.currentPlanet;
                   // ourPlanetPointEffector = bashablePlanet.GetComponentInChildren<PointEffector2D>();
                    core = bashablePlanet.transform.Find("Core");
                    if (core != null)
                    {
                        coreLocation = core.gameObject.transform;
                    }

                    if (core != null)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow) && !bashing)
                        {
                            //Debug.Log("Starting bash");
                            StartCoroutine(BashIntoCore());
                        }
                    }

                }
            }
        }

        else if (bashing)
        {
            if (ourLocationHandler.inOrbit)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //StartCoroutine(ContBash());
                }
            }

            if (!nesting)
            {
                ourRigidbody.bodyType = RigidbodyType2D.Dynamic;
                ourRigidbody.AddForce(50 * bashNormal, ForceMode2D.Force);
            }

            if (!Physics2D.OverlapCircle(transform.position, 0.5f, pReference.locationHandler.whatIsPlanet))
            {
                //Debug.Log("Bash info being reset");
                ResetBashInformation();
            }
            else
            {
                //Debug.Log("Still in planet");
            }

            if ((Time.time - bashStartTime) > bashDuration)
            {
                bashing = false;
                core.GetComponent<SpriteRenderer>().enabled = false;

            }

        }
    }

    void ResetBashInformation()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        //  ourRigidbody.gravityScale = 1;
        lastPlanet.layer = LayerMask.NameToLayer("Planet");
      //lastPlanet.GetComponentInChildren<PointEffector2D>().enabled = true;
        outOfPlanet = true;
    }

    //void BashIntoGround()
    //{
    //    RaycastHit2D rayCastHit = Physics2D.Raycast(transform.position, -transform.up, Mathf.Infinity, whatIsPlanet);
    //    Debug.DrawRay(transform.position, -transform.up * 10f, Color.red);
    //    if (rayCastHit)
    //    {
    //        bashNormal = rayCastHit.normal;
    //        bashSpot = rayCastHit.point;
    //    }

    //}

    //IEnumerator ContBash()
    //{

    //    ourRigidbody.velocity = Vector2.zero;
    //    if (ourLocationHandler.inHowManyOrbits == 1)
    //    {
    //        if (ourLocationHandler.planetsInOrbitOf[0] != lastPlanet)
    //        {
    //            bashablePlanet = ourLocationHandler.planetsInOrbitOf[0];
    //            core = bashablePlanet.GetComponentInChildren<Core>();
    //        }
    //        else
    //        {
    //            yield break;
    //        }

    //    }
    //    else if (ourLocationHandler.inHowManyOrbits >= 2)
    //    {
    //        List<GameObject> temporaryList = new List<GameObject>();
    //        foreach (GameObject go in ourLocationHandler.planetsInOrbitOf)
    //        {
    //            if (go != lastPlanet)
    //            {
    //                temporaryList.Add(go);
    //            }
    //        }
    //        bashablePlanet = FindClosest.FindClosestObject(temporaryList, this.gameObject);
    //        core = bashablePlanet.GetComponentInChildren<Core>();


    //    }
    //    else
    //    {
    //        //Debug.Log("This is wrong!");
    //        yield break;
    //    }
    //    Vector3 direction = bashablePlanet.transform.position - transform.position;
    //    direction.Normalize();
    //    RaycastHit2D ourRayCast = Physics2D.Raycast(transform.position, direction, 50.0f, newPlanetsOnly);
    //    Debug.DrawRay(transform.position, direction * 30f, Color.black, 30.0f);

    //    if (ourRayCast.collider.gameObject == bashablePlanet)
    //    {
    //        core.GetComponent<SpriteRenderer>().enabled = true;

    //        //Debug.Log("WE HIT OUR NEW PLANET!");
    //        bashNormal = ourRayCast.normal;
    //        bashSpot = ourRayCast.point;



    //        RaycastHit2D newRayCast = Physics2D.Raycast(bashSpot - (bashNormal * 1000), bashNormal, Mathf.Infinity, newPlanetsOnly);
    //        Debug.DrawRay(bashSpot - bashNormal, bashNormal * 30f, Color.blue, 30.0f);

    //        canBash = true;
    //        if (newRayCast.collider.gameObject == bashablePlanet)
    //        {
    //            gameObject.layer = incoporealLayer; //incoporeal layer
    //            planetMovement.enabled = false;
    //            gameObject.transform.position = core.transform.position;
    //            nestingStartTime = Time.time;

    //            if (anim != null)
    //            {
    //                anim.SetBool("Nesting", true);
    //            }

    //            nesting = true;
    //            ourRigidbody.gravityScale = 0;
    //            lastPlanet = bashablePlanet;
    //            lastPlanet.layer = oldPlanetLayer;
    //            ourRigidbody.isKinematic = true;
    //            bashSpot = newRayCast.point;
    //            bashNormal = newRayCast.normal;
    //            yield return StartCoroutine(RotateHeadFirst());
    //            nesting = false;
    //            bashing = true;
    //            bashStartTime = Time.time;

    //            planetMovement.enabled = false;
    //            ourRigidbody.isKinematic = false;
    //        }
    //        else
    //        {
    //            //Debug.Log("Something went wrong");
    //        }
    //    }
    //    else
    //    {
    //        //Debug.Log("Didn't hit anything");
    //    }
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DebugExtension.DrawPoint(bashSpot, 1.0f);
        //if (pReference.locationHandler.groundCheck != null)
        //{
        //    DebugExtension.DrawCircle(pReference.locationHandler.groundCheck.transform.position, Vector3.forward, Color.magenta, 0.5f);
        //}
    }

    PlayerReferences pReference;

    IEnumerator BashIntoCore()
    {
        outOfPlanet = false;
     //   ourPlanetPointEffector.enabled = false;
        planetMovement.enabled = false;
        // pReference.rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("Incoporeal");
        core.GetComponent<SpriteRenderer>().enabled = true;

        //cast a ray from the players position down to the planet they're standing on. Since it didn't hit at an angle, the normal should be straight down in the opposite direction of the first ray 
        RaycastHit2D rayCastHit = Physics2D.Raycast(transform.position, -transform.up, Mathf.Infinity, ourLocationHandler.whatIsPlanet);
       Debug.DrawRay(transform.position, -transform.up * 30, Color.green, 30.0f);

        if (rayCastHit && rayCastHit.collider.gameObject == ourLocationHandler.currentPlanet)
        {
            //if this ray hits, we hit the planet (which we should since we're standing on it)
            //Debug.Log("We hit a planet!");
            bashablePlanet = ourLocationHandler.currentPlanet;
           // bashablePlanet.GetComponentInChildren<PointEffector2D>().enabled = false;
            // This should cast a ray, find the other side of the ray, blast character out of other side of object
            bashNormal = rayCastHit.normal;
            bashSpot = rayCastHit.point; //this point should be where the ray hit the planet, and we want to shoot another ray from it to hit the other side
            lastPlanet = bashablePlanet;
            lastPlanet.layer = LayerMask.NameToLayer("AlreadyBashed");

            //this second raycast is shooting from where the first ray hit and through the planet from the other side. It has the bashnormal subtracted and multiplied in order to not hit the edge that the first raycast hit.
            Debug.Log("Here is the point we hit " + bashSpot);
            Debug.Log("Here is the normal of the point we hit " + bashNormal);
            Debug.DrawRay(bashSpot, bashNormal, Color.white, 30.0f);
            Debug.Log("Here is the normal * 1000 "  + bashNormal * 1000);
            Debug.DrawRay(bashNormal, bashNormal * 1000, Color.red, 30.0f);

            //the 1000 is affecting both the x and y components -- multiplying by the scalar 1000 does not affect the direction, but the magnitude -- the scalar scaled up the vector. It's longer now without changing its direction.

            //multiplying by a negative number sends the vector in the opposite direction

            RaycastHit2D newRayCast = Physics2D.Raycast(bashSpot - bashNormal * 1000, bashNormal, Mathf.Infinity, oldPlanetOnly);
            bashNormal = newRayCast.normal;
            bashSpot = newRayCast.point;
            Debug.DrawRay(bashSpot - bashNormal, bashNormal * 30f, Color.yellow, 30.0f);

            canBash = true;

            gameObject.layer = LayerMask.NameToLayer("Incoporeal"); //incoporeal layer

            planetMovement.enabled = false;
            gameObject.transform.position = core.transform.position;
            // gameObject.transform.position = GameObject.Find("Dark Star").transform.position;
            if (anim != null)
            {
                anim.SetBool("Nesting", true);
            }

            nesting = true;
            //ourRigidbody.gravityScale = 0;


            lastPlanet = bashablePlanet;
            lastPlanet.layer = LayerMask.NameToLayer("AlreadyBashed");

            yield return StartCoroutine(RotateHeadFirst());
            bashNormal = newRayCast.normal;
            nesting = false;
            bashing = true;
            bashStartTime = Time.time;

            planetMovement.enabled = false;
            ourRigidbody.bodyType = RigidbodyType2D.Dynamic;
         //  BashOut_();

        }
        else
        {
            yield break;
        }
        // Time.timeScale = 0;

        if (anim != null)
        {
            anim.SetBool("Nesting", true);

        }
    }

    //public IEnumerator ContinueBash()
    //{
    //    //RaycastHit2D rayCastHit = Physics2D.Raycast(transform.position, -transform.up, Mathf.Infinity, newPlanetsOnly);
    //    //RaycastHit2D rayCastHit2 = Physics2D.Raycast(transform.position, transform.up, Mathf.Infinity, newPlanetsOnly);

    //    //if (rayCastHit || rayCastHit2 || (rayCastHit && rayCastHit2))
    //    //{

    //    //    //Debug.Log("HIT PLANET! GOING TO BASH ONW!");

    //    //    if (rayCastHit)
    //    //    {
    //    //        //This should cast a ray, find the other side of the ray, blast character out of other side of object
    //    //        GameObject planet = rayCastHit.collider.gameObject;
    //    //        bashablePlanet = planet;
    //    //        core = bashablePlanet.GetComponentInChildren<Core>();
    //    //        bashNormal = rayCastHit.normal;
    //    //        bashSpot = rayCastHit.point;
    //    //    }
    //    //    else if (rayCastHit2)
    //    //    {

    //    //        GameObject planet = rayCastHit2.collider.gameObject;
    //    //        bashablePlanet = planet;
    //    //        core = bashablePlanet.GetComponentInChildren<Core>();
    //    //        bashNormal = rayCastHit2.normal;
    //    //        bashSpot = rayCastHit.point;
    //    //    }
    //    //    else if (rayCastHit && rayCastHit2)
    //    //    {
    //    //        GameObject planet1 = rayCastHit.collider.gameObject;
    //    //        GameObject planet2 = rayCastHit2.collider.gameObject;
    //    //        if (Vector2.Distance(transform.position, planet1.transform.position) < Vector2.Distance(transform.position, planet2.transform.position))
    //    //        {
    //    //            //ifplanet 1 is lcoser than planet 2
    //    //            bashablePlanet = planet1;
    //    //            core = bashablePlanet.GetComponentInChildren<Core>();
    //    //            bashNormal = rayCastHit.normal;
    //    //            bashSpot = rayCastHit.point;
    //    //        }
    //    //        else
    //    //        {
    //    //            bashablePlanet = planet2;
    //    //            core = bashablePlanet.GetComponentInChildren<Core>();
    //    //            bashNormal = rayCastHit2.normal;
    //    //            bashSpot = rayCastHit2.point;
    //    //        }

    //    //    }

    //    //    RaycastHit2D newRayCast = Physics2D.Raycast(bashSpot - bashNormal * 1000, bashNormal, Mathf.Infinity, whatIsPlanet);
    //    //    Debug.DrawRay(bashSpot - bashNormal * 1000, bashNormal * 30f, Color.blue);

    //    //    canBash = true;

    //    //    gameObject.layer = incoporealLayer; //incoporeal layer
    //    //    planetMovement.enabled = false;
    //    //    gameObject.transform.position = coreLocation.position;
    //    //    nestingStartTime = Time.time;

    //    //    if (anim != null)
    //    //    {
    //    //        anim.SetBool("Nesting", true);
    //    //    }

    //    //    nesting = true;
    //    //    ourRigidbody.gravityScale = 0;
    //    //    lastPlanet = bashablePlanet;
    //    //    lastPlanet.layer = oldPlanetLayer;
    //    //    yield return StartCoroutine(RotateHeadFirst());
    //    //    // yield return new WaitForSeconds(3.0f);
    //    //    bashNormal = newRayCast.normal;
    //    //    nesting = false;
    //    //    bashing = true;
    //    //    bashStartTime = Time.time;
    //    //    // ourRigidbody.gravityScale = 1;

    //    //    planetMovement.enabled = false;
    //    //    ourRigidbody.isKinematic = false;


    //    //}
    //    //else if (rayCastHit && rayCastHit2)
    //    //{
    //    //    GameObject planet1 = rayCastHit.collider.gameObject;
    //    //    GameObject planet2 = rayCastHit2.collider.gameObject;
    //    //    if (Vector2.Distance(transform.position, planet1.transform.position) < Vector2.Distance(transform.position, planet2.transform.position))
    //    //    {
    //    //        //ifplanet 1 is lcoser than planet 2
    //    //        bashablePlanet = planet1;
    //    //        core = bashablePlanet.GetComponentInChildren<Core>();
    //    //        bashNormal = rayCastHit.normal;
    //    //        bashSpot = rayCastHit.point;
    //    //    }
    //    //    else
    //    //    {
    //    //        bashablePlanet = planet2;
    //    //        core = bashablePlanet.GetComponentInChildren<Core>();
    //    //        bashNormal = rayCastHit2.normal;
    //    //        bashSpot = rayCastHit2.point;
    //    //    }

    //    //}
    //}




    public IEnumerator RotateHeadFirst()
    {

        ourUp *= -1;

        nestingStartTime = Time.time;
        while (Time.time < nestingStartTime + nestingDuration)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {

                yield break;
            }
            Quaternion newTargetRotation = Quaternion.FromToRotation(ourUp, bashNormal);
            Quaternion newFinalRotation = Quaternion.RotateTowards(transform.rotation, newTargetRotation, 15f * ((Time.time - nestingStartTime) / nestingDuration));
            transform.rotation = Quaternion.Euler(0, 0, newFinalRotation.eulerAngles.z);
            yield return null;
        }



    }

    public IEnumerator BashOut()
    {

        ourRigidbody.bodyType = RigidbodyType2D.Dynamic;
        //use ForceMode2D.Impulse here. The other one is better for forces over time, while this one is for instant forces like explosions <3
        //still having issues with this, try setting the velocity instead

        pReference.rb.velocity = bashNormal * 20f;
       // ourRigidbody.AddForce(70f * bashNormal, ForceMode2D.Impulse);
        yield return new WaitForSeconds(2.0f);


    }

    void BashOut_()
    {
      //  StartCoroutine(BashOut());
        ourRigidbody.bodyType = RigidbodyType2D.Dynamic;
            
        pReference.rb.velocity = bashNormal * 20f;

    }
}
