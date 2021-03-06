﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Switch : MonoBehaviour
{
    public List<ParticleSystem> switchParticles = new List<ParticleSystem>();

   public enum switchStates
    {
        off,
        powered

    }

    public enum transferType
    {
        darkStarTouching,
        switchConnection
    }

    GameObject DarkStar;
    public switchStates currentSwitchState;

    public PlayerReferences pReference;
    public List<GameObject> connectedSwitches;
    public List<LineRenderer> lineRendererConnections;

    public Dictionary<GameObject, LineRenderer> previouslyMadeConnections = new Dictionary<GameObject, LineRenderer>();

    public static event Action<GameObject, GameObject> ConnectionMade;

    bool touchedByDarkStar;
    bool connectedToPoweredSwitch;

    public static event Action<GameObject> SwitchEntered;

    public static event Action<GameObject> SwitchExited;

    public List<GameObject> poweredConnectedSwitches;

    //TODO: PERHAPS MAKE ^^ NOT STATIC, NOT SURE
    public GameObject switchConnectionPrefab;

   public void EnteredSwitch(GameObject currentSwitch)
    {
        if(SwitchEntered != null)
        {
            SwitchEntered(currentSwitch); 
        }
    }
    public void ExitedSwitch(GameObject currentSwitch)
    {
        if(SwitchExited != null)
        {
            SwitchExited(currentSwitch); 
        }
    }

    public event Action<transferType, GameObject > PoweredUp;

    public void Powered(transferType ourTransferType, GameObject poweredObject)
    {
       // //Debug.Log("Gameobject " + gameObject.name + " is powered up!");


        if (currentSwitchState != switchStates.powered)
        {
            currentSwitchState = switchStates.powered;

            if (!touchedByDarkStar )
            {//if the GO isn't touched by the dark star, it is likely powered by a powered go
                poweredConnectedSwitches.Add(poweredObject);
            }
            if (PoweredUp != null)
            {
                PoweredUp(transferType.switchConnection, this.gameObject);
                //This MUST take into account the situation if it's touched by a dark star but ALSO powered by another conduit
                //BLEH I DON'T THINK THIS WILL WORK TODO: FIX THIS
                ////TODO: FIX THIS 
                //if (touchedByDarkStar)
                //{
                //  //  PoweredUp(transferType.darkStarTouching);
                //}
                //else
                //{
                //  //  PoweredUp(transferType.switchConnection);
                //}
            }
        }
    }

    public event Action<GameObject> PowerLost;

    public void Depowered(GameObject poweredObject)
    {
        if (poweredObject != DarkStar)
        {
            poweredConnectedSwitches.Remove(poweredObject);
        } 

        if(poweredConnectedSwitches.Count <= 0)
        {
            connectedToPoweredSwitch = false;
        }

        if (currentSwitchState != switchStates.off && !touchedByDarkStar && !connectedToPoweredSwitch)
        {

            currentSwitchState = switchStates.off;
            if (PowerLost != null)
            {
               PowerLost(poweredObject);
            }
        }
    }

    public void SubscribeToPowerEvents(Switch connectedSwitch)
    {
        connectedSwitch.PoweredUp += this.Powered;
        connectedSwitch.PowerLost += this.Depowered;
    }

    public void DesubscribeFromPowerEvents(Switch connectedSwitch)
    {
        //since there's no connection, that switch being powered up will no longer apply to this switch as well
        connectedSwitch.PoweredUp -= this.Powered;
        connectedSwitch.PowerLost -= this.Depowered;
    }


    void ConnectBack(GameObject switchWereConnectedTo)
    {
        Switch thisSwitch = switchWereConnectedTo.GetComponent<Switch>(); 
    }

    public void MakeConnection(GameObject connectedSwitchGO)
    {
     
        //this method connects two switches by adding the connected switch to the dictionary, transferring power, then calling the same method on the other switch.
        //It also calls the "Connection Made" event.
        
        //You avoided issues with recursion causing an infinite loop by _____. 

        if (!connectedSwitches.Contains(connectedSwitchGO)) { 
            //if the connection already exists between these two

            Switch connectedSwitch = connectedSwitchGO.GetComponent<Switch>();
            connectedSwitches.Add(connectedSwitchGO);

            if(currentSwitchState == switchStates.powered)
            {
                connectedSwitch.Powered(transferType.switchConnection, this.gameObject);
            }
            //if(connectedSwitch.currentSwitchState == switchStates.powered && !poweredConnectedSwitches.Contains(connectedSwitchGO))
            //{
                
            //    poweredConnectedSwitches.Add(connectedSwitchGO);
            //}

            GameObject newSwitchConnection = Instantiate(switchConnectionPrefab, transform);

            SubscribeToPowerEvents(connectedSwitch);

            ////Debug.Log("This is the switch that " + gameObject + " Is connected to: " + connectedSwitch.gameObject);
            if (!connectedSwitch.connectedSwitches.Contains(gameObject))
            {
                connectedSwitch.MakeConnection(gameObject);
              //  connectedSwitch.SubscribeToPowerEvents(gameObject.GetComponent<Switch>());
                
            }
            if (!previouslyMadeConnections.ContainsKey(connectedSwitchGO) ) 
            {
                //so here, we should be making sure that this connection hasn't been made before
                LineRenderer ourLineRenderer = newSwitchConnection.GetComponent<LineRenderer>();
                lineRendererConnections.Add(ourLineRenderer);
                ourLineRenderer.SetPosition(0, transform.position);
                ourLineRenderer.SetPosition(1, connectedSwitchGO.transform.position);
            }
            else
            {
                previouslyMadeConnections[connectedSwitchGO].enabled = true; 
            }


        }
      
        if(ConnectionMade != null)
        {
            ConnectionMade(this.gameObject, connectedSwitchGO);
        }

    }

    void SendEnergyToConnection()
    {

        foreach(GameObject go in connectedSwitches)
        {
            Switch theirSwitch = go.GetComponent<Switch>();
            if (!theirSwitch.poweredConnectedSwitches.Contains(this.gameObject))
            {
                theirSwitch.poweredConnectedSwitches.Add(this.gameObject);

            }
            theirSwitch.currentSwitchState = switchStates.powered; 
        }
    }

    private void Awake()
    {
        pReference = GameObject.Find("Player").GetComponent<PlayerReferences>();
        DarkStar = GameObject.Find("Dark Star");
        switchParticles = GetComponentsInChildren<ParticleSystem>().ToList();
        
    }

    public void BrightenSwitchParticles()
    {
        ParticleSystemPlayer.PlayChildParticleSystems(switchParticles);
    }

    public void DimSwitchParticles()
    {

        ParticleSystemPlayer.StopChildParticleSystems(switchParticles);
    }

    public virtual void OnTriggerEnter2D(Collider2D hit)
    {

       // base.OnTriggerEnter2D(hit);
        if(hit.gameObject.tag == "Player")
        {
          //  //Debug.Log("We hit this switch " + this.gameObject.name);
           // //Debug.Log("We're touching the player");
            //pReference.locationHandler.currentSwitch = this.gameObject;

            //TODO: COMMENT THIS BACK IN
            SwitchEntered(this.gameObject);
        }
        else if(hit.gameObject.tag == "Dark Star")
        {
            Powered(transferType.darkStarTouching, this.gameObject);// currentSwitchState = switchStates.powered;
            touchedByDarkStar = true;
        }
    }
    public virtual void OnTriggerExit2D(Collider2D hit)
    {
      //  base.OnTriggerExit2D(hit);
        if (hit.gameObject.tag == "Player")
        {
             SwitchExited(this.gameObject);
        }
        if (hit.gameObject.tag == "Dark Star") {

            if (!connectedToPoweredSwitch) {

                Depowered(DarkStar);
               // currentSwitchState = switchStates.off;

            }
            touchedByDarkStar = false;
            //this boolean is to make sure they still get power from the star even if they're not connected 
        }
    }

    public void DestroySpecificConnection(GameObject connectedSwitchGO)
    {
        //this removes the connection from the dictionary and desubscribes from the events of
        //the connected switch


        //Debug.Log("This is happening");
        //they SHOULD have the same index -- need to make sure nothing changes this 
        int index = connectedSwitches.IndexOf(connectedSwitchGO);
        ///ONLY ONE LINE RENDERER SHOULD BE CREATED
        //TODO: MAKE IT SO ONLY ONE LINE RENDERER PER CONNECTION HAPPENS
         
        connectedSwitches.Remove(connectedSwitchGO);
        LineRenderer ourLineRenderer = lineRendererConnections[index];
        ourLineRenderer.enabled = false;
        //Debug.Log(ourLineRenderer.gameObject.name);
        lineRendererConnections.Remove(ourLineRenderer);
        //OKAY YOU REALLY NEED TO PLAN THIS OUT
        DesubscribeFromPowerEvents(connectedSwitchGO.GetComponent<Switch>());
         if (poweredConnectedSwitches.Contains(connectedSwitchGO))
            {
                poweredConnectedSwitches.Remove(connectedSwitchGO);
                if (poweredConnectedSwitches.Count == 0)
                {
                    connectedToPoweredSwitch = false;
                }
                if (!connectedToPoweredSwitch && !touchedByDarkStar)
                {
                    Depowered(connectedSwitchGO);
                }
            }


        Switch lastConnectedSwitch = connectedSwitchGO.GetComponent<Switch>();
       
        previouslyMadeConnections.Add(connectedSwitchGO, ourLineRenderer);


        DesubscribeFromPowerEvents(lastConnectedSwitch);

        if (lastConnectedSwitch.connectedSwitches.Contains(this.gameObject))
        {
            lastConnectedSwitch.DestroySpecificConnection(this.gameObject);
        }
        //OKAY YOU REALLY NEED TO PLAN THIS OUT
       
    }

    public void DestroyLastConnection()
    {
        //this needs to destroy the connection on the other switch's side too
        GameObject lastConnectedSwitchGO = connectedSwitches.Last();
        Switch lastConnectedSwitch = lastConnectedSwitchGO.GetComponent<Switch>();
        LineRenderer lastLineRenderer = lineRendererConnections.Last();

        connectedSwitches.Remove(lastConnectedSwitchGO);

        lastLineRenderer.enabled = false;
        lineRendererConnections.Remove(lastLineRenderer);
        previouslyMadeConnections.Add(lastConnectedSwitchGO, lastLineRenderer);

        DesubscribeFromPowerEvents(lastConnectedSwitch);
        //OKAY YOU REALLY NEED TO PLAN THIS OUT
        if (lastConnectedSwitch.connectedSwitches.Contains(gameObject))
        {
            lastConnectedSwitch.DesubscribeFromPowerEvents(this);
            lastConnectedSwitch.connectedSwitches.Remove(this.gameObject);
            if (lastConnectedSwitch.poweredConnectedSwitches.Contains(this.gameObject))
            {
                lastConnectedSwitch.poweredConnectedSwitches.Remove(this.gameObject);
                if(lastConnectedSwitch.poweredConnectedSwitches.Count == 0)
                {
                    lastConnectedSwitch.connectedToPoweredSwitch = false;
                }
                if(!lastConnectedSwitch.connectedToPoweredSwitch && !lastConnectedSwitch.touchedByDarkStar)
                {
                    lastConnectedSwitch.Depowered(this.gameObject);
                }
            }
        }
    }

    // Use this for initialization

    // Update is called once per frame
    void Update()
    {

    }
}
