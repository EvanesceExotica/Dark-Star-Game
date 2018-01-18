using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Conduit : ParentTrigger {

    public List<Switch> ourSwitches = new List<Switch>();
    public List<GameObject> ourConduitPoints = new List<GameObject>();
    public List<GameObject> pointsCurrentlyTouched = new List<GameObject>();
    Dictionary<GameObject, List<GameObject>> ourSwitchConnections = new Dictionary<GameObject, List<GameObject>>();
    Stack<Switch> latestConnectionsMade = new Stack<Switch>();

    public GameObject switchPrefab;

    int switchNumber;

    void GenerateSwitches()
    {

        
    }

    private void Awake()
    {
        Pulse.DisasterBegun += this.PotentiallyDestoryConnections;
        Switch.ConnectionMade += this.UpdateConnections;
    }
    private void OnEnable()
    {
        ourSwitches = GameObject.FindObjectsOfType<Switch>().ToList();
        foreach(Switch s in ourSwitches)
        {
            s.PoweredUp += this.CheckIfAllArePowered;
        }
    }

    private void OnDisable()
    {

    }



    public static event Action AllSwitchesPowered;

    public static void AllPowered()
    {
        //Debug.Log("All the switches are powered now");
        if (AllSwitchesPowered != null)
        {
            AllSwitchesPowered();
        }
    }

    void PotentiallyDestoryConnections(bool anchored)
    {
        if (!anchored)
        {
            DestroyLatestConnection();
        }
    }

    void UpdateConnections(GameObject anchor, GameObject connectedTo)
    {
        Switch ourSwitch = anchor.GetComponent<Switch>();
        latestConnectionsMade.Push(ourSwitch);
        if (ourSwitchConnections.ContainsKey(anchor))
        {
            //adds to dictionary for safe keeping, then to stack for removing connections
            ourSwitchConnections[anchor].Add(connectedTo);
            latestConnectionsMade.Push(ourSwitch);
        }
        else
        {
            List<GameObject> newConnections = new List<GameObject>();
            newConnections.Add(connectedTo);
            ourSwitchConnections.Add(anchor, newConnections);
        }

    }

    void DestroyLatestConnection()
    {
        if (latestConnectionsMade.Count > 0)
        {
            Switch ourSwitch = latestConnectionsMade.Peek();
            latestConnectionsMade.Pop();
            ourSwitch.DestroySpecificConnection(ourSwitch.connectedSwitches.Last());
        }
        // ourSwitchConnections.Keys.Last();
    }
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.V))
        {
            AllPowered();
        }

    }

    void CheckIfAllArePowered(Switch.transferType transferType, GameObject poweredSwitch)
    {

       // Debug.Log("Checking if all are powered");
        bool allPowered = true;
        foreach(Switch s in ourSwitches)
        {
            if(s.currentSwitchState == Switch.switchStates.off)
            {
               // Debug.Log("Switch " + s.name + " is not powered");
                allPowered = false;
            }
        }
        if (allPowered)
        {
            AllPowered();
        }
    }


    public override void OnChildTriggerEnter2D(Collider2D hit, GameObject hitChild)
    {
        if (hit.gameObject.tag == "Dark Star")
        {
            //Debug.Log(hitChild.name + " has been touched by dark star");
            pointsCurrentlyTouched.Add(hitChild);
        }
    }

    public override void OnChildTriggerExit2D(Collider2D hit, GameObject hitChild)
    {
         if (hit.gameObject.tag == "Dark Star")
        {
            pointsCurrentlyTouched.Remove(hitChild);
        }
    }
}
