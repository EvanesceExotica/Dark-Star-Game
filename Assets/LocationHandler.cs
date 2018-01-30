using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocationHandler : MonoBehaviour {

    public bool anchored;
    public GameObject pulseObject;
    Collider2D ourCollider;
    Collider2D pulseCollider;

    public LayerMask anchoredMask;
    public LayerMask floatingFreeMask;
    public LayerMask everythingMask;

    PointEffector2D pulsePointEffector;
    bool ignoringCollision;

    public GameObject groundCheck;
    public Hookshot hookshot;
    public bool inOpenSpace;
    public bool inOrbit;
    public bool onPlanet;

    public LayerMask whatIsPlanet;
    public LayerMask whatIsSpace;

    public int inHowManyOrbits;

    public PlanetMovement planetMovement;
    AdhereToPlanet planetAdhering;

    public GameObject currentPlanet;

    public PlayerMovement currentMovement;

    public GameObject currentSwitch;

    SpaceMovement normalMovement;

    public List<GameObject> planetsInOrbitOf = new List<GameObject>();

    PointEffector2D gravityWell;
    public GameObject closestPlanet;

    bool touchedDown;
    bool liftedUp;

    GameObject previousSwitch;

   public bool closeToPlanet;
    public bool floatingFree;

    private void Awake()
    {
        pulseObject = GameObject.Find("PulsePrefab");
        pulseCollider = pulseObject.GetComponent<Collider2D>();
        pulsePointEffector = pulseObject.GetComponent<PointEffector2D>();
        ourCollider = GetComponent<Collider2D>();

        TheVoid.PlayerEnteredVoid += InVoid;
        TheVoid.PlayerExitedVoid += InSafeZone;
        Switch.SwitchEntered += SetCurrentSwitch;
        Switch.SwitchExited += RemoveCurrentSwitch;
    }

    private void OnDisable()
    {
        TheVoid.PlayerEnteredVoid -= InVoid;
        TheVoid.PlayerExitedVoid -= InSafeZone;
    }

    public bool inVoid;

    public void AddOrbitedPlanet(GameObject orbitedPlanet)
    {

        if (!planetsInOrbitOf.Contains(orbitedPlanet))
        {

            planetsInOrbitOf.Add(orbitedPlanet);
        }
    }
    void InVoid()
    {//testing to see if this works
        inVoid = true;
    }

    void InSafeZone()
    {
        inVoid = false;
    }

    public  event Action<GameObject> TouchedOnPlanet;

    public  event Action<GameObject> LiftedFromPlanet;

    public void PlanetTouchedDownOn(GameObject planet)
    {
        touchedDown = true;
       // planet.GetComponentInChildren<PointEffector2D>().enabled = false;
        if (TouchedOnPlanet != null)
        {
            TouchedOnPlanet(planet);
        }
    }

    public void PlanetLiftedOffFrom(GameObject planet)
    {

        if(LiftedFromPlanet != null)
        {
            LiftedFromPlanet(planet);
        }
    }

    public static event Action<PlayerMovement> MovementTypeSwitched;

    void SwitchMovementType(PlayerMovement ourTypeSwitchedTo)
    {
        currentMovement = ourTypeSwitchedTo;
        if(MovementTypeSwitched != null)
        {
            MovementTypeSwitched(currentMovement);
        }
        
    }
    void SetCurrentSwitch(GameObject newSwitch)
    {
        currentSwitch = newSwitch;
    }

    void RemoveCurrentSwitch(GameObject switchExitedFrom)
    {
        previousSwitch = switchExitedFrom;
        currentSwitch = null;
    }

	// Use this for initialization
	void Start () {
        groundCheck = transform.Find("GroundCheck").gameObject;
        planetAdhering = gameObject.GetComponent<AdhereToPlanet>();
        normalMovement = gameObject.GetComponent<SpaceMovement>();
        planetMovement = gameObject.GetComponent<PlanetMovement>();
        hookshot = GameObject.Find("Hook").GetComponent<Hookshot>();
	}

    void IgnoreCollisionWithPulseObject()
    {
        ////Debug.Log("Ignoring collision so not pulled b pulse");
        ignoringCollision = true;
        pulsePointEffector.colliderMask = anchoredMask;
      //  Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pulse"), LayerMask.NameToLayer("Player"), true);
       // Physics2D.IgnoreCollision(pulseCollider, ourCollider, true);
    }

    void RestoreCollisionWithPulseObject()
    {
      //  //Debug.Log("Restoring collision so pulled b pulse");
        ignoringCollision = false;
        pulsePointEffector.colliderMask = everythingMask; 
        //pulsePointEffector.colliderMask = floatingFreeMask; 
    //    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pulse"), LayerMask.NameToLayer("Player"), false);
    }
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            DebugExtension.DrawCircle(groundCheck.transform.position, Vector3.forward, Color.magenta, 0.2f);
        }
    }

    public static event Action<bool> AnchorStatusChanged;

    public void AnchorStatusWasChanged(bool isAnchored){
        if(AnchorStatusChanged != null){
            AnchorStatusChanged(isAnchored);
        }
    }

    // Update is called once per frame
    void Update () {
        
        if(currentSwitch != null || onPlanet || hookshot.hookedOn)
        {
            anchored = true;
            AnchorStatusWasChanged(anchored);
            floatingFree = false;
            if (!ignoringCollision)
            {
              //  IgnoreCollisionWithPulseObject();
            }
        }
        else
        {
            floatingFree = true;
            anchored = false;
            AnchorStatusWasChanged(anchored);
            //this is part of something to make sure that you're not pulled into the star when you're hooked on to something
            if (ignoringCollision)
            {
               // RestoreCollisionWithPulseObject();

            }
        }
        //if(onPlanet | hookshot.hookedOn)
        //{
        //    anchored = true;
        //}

        inOpenSpace = Physics2D.OverlapCircle(groundCheck.transform.position, 0.2f, whatIsSpace);

        inHowManyOrbits = planetsInOrbitOf.Count;

        if (inHowManyOrbits == 0)
        {
            inOrbit = false;
        }
        else if (inHowManyOrbits > 0)
        {
            inOrbit = true;

        }


        onPlanet = Physics2D.OverlapCircle(groundCheck.transform.position, 0.2f, whatIsPlanet);


        if (inOrbit)
        {
            closestPlanet = planetsInOrbitOf[0];
            if(gravityWell != null && gravityWell != closestPlanet.GetComponentInChildren<PointEffector2D>())
            {
                gravityWell = closestPlanet.GetComponentInChildren<PointEffector2D>();
            }
       
        }

        


        if (inOpenSpace && !onPlanet && !inOrbit && !planetMovement.jumping && !planetMovement.fallingToPlanet)
        {
            //if in space, not on planet or in orbit of planet
            touchedDown = false;
            normalMovement.enabled = true;
            currentMovement = normalMovement;
            planetMovement.enabled = false;
            SwitchMovementType(normalMovement);
            gameObject.transform.parent = null;
            
                 
        }
        else if(inOpenSpace && !onPlanet && inOrbit && !planetMovement.jumping && !planetMovement.fallingToPlanet)
        {
            touchedDown = false;
           // planetMovement.enabled = true;
           // normalMovement.enabled = false;
           // planetMovement.cantOrient = true;
            planetMovement.enabled = false;
            normalMovement.enabled = true;

        }
        else if(inOpenSpace && onPlanet)
        {
            normalMovement.enabled = false;
            planetMovement.enabled = true;
            planetMovement.cantOrient = false;
            SwitchMovementType(planetMovement);
            currentPlanet = Physics2D.OverlapCircle(groundCheck.transform.position, 0.5f, whatIsPlanet).gameObject;
            currentMovement = planetMovement;
            if (!touchedDown)
            {
                PlanetTouchedDownOn(currentPlanet);
            }


        }


    }

    
}
