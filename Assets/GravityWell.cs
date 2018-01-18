using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : ParentTrigger {

    public List<GameObject> objectsInWell = new List<GameObject>();
    GameObject player;
    LocationHandler locationHandler;
    GameObject parent;
    PointEffector2D ourPointEffector;
    PlayerReferences pReference;

    GameObject gravityWellObject;
    GameObject proximityTriggerObject;

   public bool onPlanet;

   public bool gravityWellOn;


    public event Action<GameObject> playerEnteredOrbit;
    public event Action<GameObject> playerLeftOrbit;
    // TODO: Work on this. Make sure the methods to turn off the point effector are working. And zoom out.
    private void OnEnable()
    {
        pReference.locationHandler.TouchedOnPlanet += this.SetPlanetTouched;
        pReference.locationHandler.LiftedFromPlanet += this.RemovePlanetTouched;
       // prefere
       // locationHandler.TouchedOnPlanet += this.TurnPointEffectorOff;
        //locationHandler.LiftedFromPlanet += this.TurnPointEffectorOn;
    }

    private void OnDisable()
    {
        //locationHandler.TouchedOnPlanet -= this.TurnPointEffectorOff;
       // locationHandler.LiftedFromPlanet -= this.TurnPointEffectorOn;
        pReference.locationHandler.TouchedOnPlanet -= this.SetPlanetTouched;
        pReference.locationHandler.LiftedFromPlanet -= this.RemovePlanetTouched;
    }



    // Use this for initialization
    void Awake() {
        player = GameObject.FindWithTag("Player");
        pReference = player.GetComponent<PlayerReferences>();
        locationHandler = pReference.locationHandler;
        ourPointEffector = GetComponentInChildren<PointEffector2D>();
        proximityTriggerObject = GameObject.Find("PlanetProximityHandler");
        gravityWellObject = GameObject.Find("GravityWell");
       // parent = transform.parent.gameObject;
            
    }

    public void SetPlanetTouched(GameObject us)
    {
        if (us != this.gameObject)
        {
            return;
        }

        onPlanet = true;
    }

    public void RemovePlanetTouched(GameObject us)
    {
        if (us != this.gameObject)
        {
            return;
        }

        onPlanet = false;
    }
   

   public  void TurnPointEffectorOff()
    {
        //Debug.Log("<color=orange>Turning OFF</color>");
        gravityWellOn = false;
        ourPointEffector.enabled = false;
    }


    public void TurnPointEffectorOn()
    {
        gravityWellOn = true;

        //Debug.Log("<color=green>Turning ON</color>");
        ourPointEffector.enabled = true;
    }


    void SwitchPointEffector(PlayerMovement ourMovementTypeToSwitchTo)
    {
        //if(ourMovementTypeToSwitchTo.GetType() == typeof(PlanetMovement))
        //{
        //    ourPointEffector.enabled = false;
        //}
        //else
        //{
        //    ourPointEffector.enabled = true;
        //}
    }

    // Update is called once per frame
    void Update() {

        //if the player is on the planet, the gravity well turns off.
        //if the player is in the orbit of the planet but not on the plaet, it turns on.
        //if the player is neither on the planet nor in its orbit, the gravity well turns off. 
        if(onPlanet && gravityWellOn)
        {
            //Debug.Log("On planet and gravity well is on");
            TurnPointEffectorOff();
        }
        if(!onPlanet && objectsInWell.Contains(player) && !gravityWellOn  && !pReference.starBash.nesting && !pReference.starBash.bashing)
        {
            //Debug.Log("Not on planet but in well, not bashing or nesting");
            TurnPointEffectorOn();
        }
        //this one should have the point effector off if the player is bashing
        else if(!onPlanet && objectsInWell.Contains(player) && (pReference.starBash.nesting || pReference.starBash.bashing) && !gravityWellOn)
        {
            //Debug.Log("Not on planet but in well, but I AM bashing or nesting");
            TurnPointEffectorOff();
        }
        else if(!onPlanet && !objectsInWell.Contains(player) && gravityWellOn)
        {
            //Debug.Log("Not on planet AND NOT IN GRAVITY WELL");
            TurnPointEffectorOff();
        }
    }

  


    public override void OnChildTriggerEnter2D(Collider2D hit, GameObject hitChild)
    {
        if (hitChild == gravityWellObject)
        {
            //if this is the gravity well
            if (!hit.isTrigger && !objectsInWell.Contains(hit.gameObject))
            {
                objectsInWell.Add(hit.gameObject);
                //if (hit.gameObject == player )
                //{
                //   // locationHandler.AddOrbitedPlanet(this.gameObject);
                //}
            }
        }
        else if(hitChild == proximityTriggerObject)
        {
            //check if object is in proximity to planet (not in gravity well) -- this will trigger the player turning to rotate their feat toward the planet
            if (!hit.isTrigger)
            {
                if(hit.gameObject == player)
                {

                    pReference.locationHandler.AddOrbitedPlanet(this.gameObject);
                    if (pReference.locationHandler.planetsInOrbitOf.Count == 1)
                    {//as in, if this planet is the only planet the player is near
                        pReference.locationHandler.closeToPlanet = true;
                        pReference.locationHandler.closestPlanet = this.gameObject;
                    }
                }
            }
        }
           

    }

    public override void OnChildTriggerExit2D(Collider2D hit, GameObject hitChild)
    {
        if (hitChild == gravityWellObject)
        {
            if (!hit.isTrigger && objectsInWell.Contains(hit.gameObject))
            {
                objectsInWell.Remove(hit.gameObject);
                //if (hit.gameObject == player)
                //{
                //   // locationHandler.planetsInOrbitOf.Remove(this.gameObject);
                //}
            }
        }

        else if (hitChild == proximityTriggerObject)
        {
            //check if object is in proximity to planet (not in gravity well) -- this will trigger the player turning to rotate their feat toward the planet
            if (!hit.isTrigger)
            {
                if (hit.gameObject == player)
                {
                    pReference.locationHandler.planetsInOrbitOf.Remove(this.gameObject);
                    if (pReference.locationHandler.planetsInOrbitOf.Count == 0)
                    {
                        pReference.locationHandler.closeToPlanet = false;
                        pReference.locationHandler.closestPlanet = null;
                    }
                }

            }
        }
    }
}
