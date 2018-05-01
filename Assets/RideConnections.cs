using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public class RideConnections : PowerUp
{
    public List<SwitchConnection> switchConnectionsWeveRidden = new List<SwitchConnection>();
    List<SwitchConnection> allSwitchConnections = new List<SwitchConnection>();
    [SerializeField] GameObject switchConnectionPool;
    public SwitchConnection connectionWereRiding;
    public GameObject previousSwitch;
    PlayerReferences pReference;


    LineRenderer pullToSwitchLineRenderer;
    bool riding;

    public Conduit switchHolder;
    public GameObject currentSwitchGO;
    public Switch currentSwitch;
    public GameObject destinationSwitchGO;
    GameObject trackSparksGO;
    List<ParticleSystem> trackSparksSystems = new List<ParticleSystem>();


    GameObject jumpToSwitchGO;
    List<ParticleSystem> jumpToSwitchParticlesystems = new List<ParticleSystem>();

    public static event Action RidingConnection;

    void WereRidingConnect()
    {
        if (RidingConnection != null)
        {
            RidingConnection();
        }
    }

    void WeveStoppedRidingConnection()
    {
        if (StoppedRidingConnection != null)
        {
            StoppedRidingConnection();
        }
    }
    public static event Action StoppedRidingConnection;


    //let's just have this travel clockwise for now 

    void SetCurrentSwitch(GameObject switchGO)
    {
        currentSwitchGO = switchGO;
        currentSwitch = currentSwitchGO.GetComponent<Switch>();
    }

    public override void StartPowerUp()
    {
        base.StartPowerUp();
        StartCoroutine(LerpToStartPoint());
    }

    void RemoveCurrentSwitch(GameObject switchGO)
    {
        currentSwitchGO = null;
        currentSwitch = null;
    }

    bool CheckForConnection()
    {
        //for now let's just find the closest switch with a connection
        //add the previous connection, search for a new one
            switchConnectionsWeveRidden.Add(connectionWereRiding);
        bool connectionExists = false;
        if (currentSwitch == null)
        {
            Debug.Log("Current switch is null is the issue");
        }
        if (currentSwitch.switchConnectionList != null && currentSwitch.switchConnectionList.Count > 0)
        {
            destinationSwitchGO = FindNearestConnectedSwitch(currentSwitch.switchConnectionList);
            if (destinationSwitchGO != null)
            {
                connectionExists = true;
            }

        }


        return connectionExists;
    }

    bool startingFromConnectionTransformSpot;

    public override void Awake()
    {
        base.Awake();
        extendableIntervalDuration = 4.0f;
        //TODO: PUT THESE BACK IN 
        //   pullToSwitchLineRenderer = transform.Find("PullToSwitchEffect").GetComponent<LineRenderer>();
        //  pullToSwitchLineRenderer.enabled = false;

        switchHolder = GameObject.Find("Switch Holder").GetComponent<Conduit>();
        if (trackSparksGO != null)
            trackSparksSystems = trackSparksGO.GetComponentsInChildren<ParticleSystem>().ToList();
        if (jumpToSwitchGO != null)
        {
            jumpToSwitchParticlesystems = jumpToSwitchGO.GetComponentsInChildren<ParticleSystem>().ToList();
        }

        autoActivated = false;
        ourRequirement = Requirement.OnlyUseOffSwitch;

        ChoosePowerUp.connectorChosen += this.SetPoweredUp;
        Switch.SwitchEntered += this.SetCurrentSwitch;

        Switch.SwitchExited += this.RemoveCurrentSwitch;
    }

    // IEnumerator FindConnectorToRide(GameObject switch1, GameObject switch2)
    // {
    //     while (true)
    //     {
    //         //  RaycastHit2D raycastHit = Physics2D.Raycast()
    //         yield return null;
    //     }
    // }

    Vector3 FindNearestConnectionPoint()
    {

        Vector3 point = Vector3.zero;
        if (switchConnectionPool == null)
        {
            switchConnectionPool = GameObject.Find("SwitchConnectionPool");
        }
        else
        {
            return point;
        }
        allSwitchConnections = switchConnectionPool.GetComponentsInChildren<SwitchConnection>().ToList();
        if (allSwitchConnections == null || allSwitchConnections.Count == 0)
        {
            return point;
        }
        List<Vector3> connectionPoints = new List<Vector3>();
        foreach (SwitchConnection connection in allSwitchConnections)
        {
            connectionPoints.AddRange(connection.prunedPathPoints);
        }
        Vector3 closest = FindClosest.FindClosestVector(connectionPoints, this.gameObject);
        return closest;
    }
    GameObject FindNearestSwitch()
    {
        GameObject nearestSwitch = FindClosest.FindClosestObject(switchHolder.ourSwitchGameObjects, gameObject);
        return nearestSwitch;
    }

    GameObject FindNearestConnectedSwitch(List<SwitchConnection> currentSwitchConnections)
    {
        GameObject closestConnectedSwitchGO = null;
        float shortestDistance = Mathf.Infinity;
        SwitchConnection shortestConnection = currentSwitchConnections[0];
        if (currentSwitchConnections.Count > 1)
        {
            foreach (SwitchConnection con in currentSwitchConnections)
            {
                if (switchConnectionsWeveRidden.Contains(con))
                {
                    //This is so we don't land on the same connection and go back and forth, so we want to skip it
                    continue;
                }
                if (con.Distance < shortestDistance)
                {
                    shortestDistance = con.Distance;
                    shortestConnection = con;
                }
            }
        }
        else
        {
            if (switchConnectionsWeveRidden.Contains(shortestConnection))
            {
                //we only have one connection and its one we've already ridden
                Debug.Log("We've already ridden this connection");
                return null;
            }
        }
        if (currentSwitch == shortestConnection.switchA)
        {
            closestConnectedSwitchGO = shortestConnection.switchBGO;
        }
        else if (currentSwitch == shortestConnection.switchB)
        {
            closestConnectedSwitchGO = shortestConnection.switchAGO;
        }
        connectionWereRiding = shortestConnection;

        return closestConnectedSwitchGO;
    }



    GameObject FindNearestConnectedSwitchToCurrentSwitch()
    {
        GameObject closestConnectedSwitchGO = FindClosest.FindClosestObject(currentSwitch.connectedSwitches, currentSwitchGO);
        if (currentSwitch.connectedSwitches.Count == 1)
        {
            if (closestConnectedSwitchGO == previousSwitch)
            {
                //we want to return null if it's a switch we've already travelled from to avoid it bouncing back and forth
                closestConnectedSwitchGO = null;
            }
        }
        else if (currentSwitch.connectedSwitches.Count > 1)
        {
            if (closestConnectedSwitchGO == previousSwitch)
            {
                List<GameObject> listWithoutPrevious = new List<GameObject>();
                listWithoutPrevious = currentSwitch.connectedSwitches.ToList();
                listWithoutPrevious.Remove(previousSwitch);
                closestConnectedSwitchGO = FindClosest.FindClosestObject(listWithoutPrevious, currentSwitchGO);
            }
        }
        return closestConnectedSwitchGO;
    }

    GameObject GetTransformSpot()
    {
        List<GameObject> spots = new List<GameObject>();
        GameObject closestSpot = null;
        foreach (GameObject go in pReference.triggerHandler.objectsHoveredOver)
        {
            if (go.GetComponent<InteractableTransformSpot>() != null)
            {
                spots.Add(go);
                //if we're hovering over a transform spot, lerp to that
            }
        }
        if (spots.Count != 0)
        {
            closestSpot = FindClosest.FindClosestObject(spots, gameObject);
            startingFromConnectionTransformSpot = true;
            connectionWereRiding = closestSpot.transform.parent.GetComponent<SwitchConnection>(); //this should be the switch connection
        }
        return closestSpot;

    }

    public Vector3 FindWhatToLerpTo()
    {
        if (pReference.triggerHandler.objectsHoveredOver != null && pReference.triggerHandler.objectsHoveredOver.Count > 0)
        {

            GameObject closestSpot = GetTransformSpot();
            if (closestSpot != null)
            {
                return closestSpot.transform.position;
            }
        }
        Vector3 nearestConnectionPoint = FindNearestConnectionPoint();
        GameObject nearestSwitch = FindNearestSwitch();
        //else if we're not hovering over a transform spot;
        if (nearestConnectionPoint != Vector3.zero)
        {
            //if there ARE connections, however, find if a connection point or a switch is closer

            float distanceFromNearestPoint = Vector3.Distance(transform.position, nearestConnectionPoint);
            float distanceFromNearestSwitch = Vector3.Distance(transform.position, nearestSwitch.transform.position);
            if (distanceFromNearestPoint < distanceFromNearestSwitch)
            {
                startingFromConnectionTransformSpot = true;
                return nearestConnectionPoint;
            }
            else if (distanceFromNearestSwitch < distanceFromNearestPoint)
            {

                return nearestSwitch.transform.position;
            }
        }
        //there are no connections, so jump to the nearest switch 
        //TODO: May just have this return null to avoid the function of the StarChain being useless
        return FindNearestSwitch().transform.position;
    }



    public IEnumerator LerpToStartPoint()
    {
        Vector3 startPoint = FindWhatToLerpTo();
        float time = 0.0f;
        while (Vector2.Distance(transform.position, startPoint) > 0.5)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                NoLongerRiding();
                WeveStoppedRidingConnection();
                yield break;

            }
            time += Time.deltaTime / 1.0f;
            transform.position = Vector2.Lerp(transform.position, startPoint, Mathf.SmoothStep(0.0f, 1.0f, time));

            yield return null;
        }
        if (startingFromConnectionTransformSpot)
        {
            //have a flag so that this doesn't trigger twice if you're jumping to the transform spot rather than starting hovering over it;
            if (pReference.triggerHandler.objectsHoveredOver != null && pReference.triggerHandler.objectsHoveredOver.Count > 0)
            {
                GetTransformSpot();
                SetDestinationSwitch();
            }
            switchConnectionsWeveRidden.Add(connectionWereRiding);
            if (!connectionWereRiding.temporary)
            {
               // List<Vector3> pointsToTravel = DeterminePathPointsFromCurrentPoint(connectionWereRiding.pathPoints, startPoint, destinationSwitchGO.transform.position);
                StartCoroutine(RideSwitchConnection(startPoint));
            }
            else if (connectionWereRiding.temporary)
            {
                List<Vector3> pointsToTravel = DeterminePathPointsFromCurrentPoint(connectionWereRiding.pathPoints, startPoint, destinationSwitchGO.transform.position);
                StartCoroutine(RideTemporarySwitchConnection(pointsToTravel));
            }

        }
        else
        {
            if (CheckForConnection() == true)
            {

                Debug.Log("A connection was found!");
                //if a connection exists between this switch and another;
                if (!connectionWereRiding.temporary)
                {
                    StartCoroutine(RideSwitchConnection(startPoint));
                }
                else if (connectionWereRiding.temporary)
                {
                    StartCoroutine(RideTemporarySwitchConnection(connectionWereRiding.pathPoints));
                }

            }
            else
            {
                Debug.Log("We didn't find a connection");
            }
        }
    }

    void SetDestinationSwitch()
    {
        GameObject closestSwitchOfTheTwo = FindClosest.ClosestOfTwo(connectionWereRiding.switchAGO, connectionWereRiding.switchBGO, this.gameObject);
        destinationSwitchGO = closestSwitchOfTheTwo;
    }

    void JumpToNearestSwitch()
    {
        if (currentSwitch == null)
        {
            //in order to start this, we must be off of the switch
            currentSwitchGO = FindNearestSwitch();

            currentSwitch = FindNearestSwitch().GetComponent<Switch>();
            Transform transformToJumpTo = currentSwitchGO.transform;
            transform.position = transformToJumpTo.position;
            pReference.rb.velocity = new Vector2(0, 0);
            ParticleSystemPlayer.PlayChildParticleSystems(jumpToSwitchParticlesystems);
            if (CheckForConnection() == true)
            {
                Debug.Log("A connection was found!");
                //if a connection exists between this switch and another;
               // StartCoroutine(RideSwitchConnection());
            }
        }
    }


    // Use this for initialization
    void Start()
    {
        pReference = GetComponentInParent<PlayerReferences>();
    }
    void PlayRideSparks()
    {
        ParticleSystemPlayer.PlayChildParticleSystems(trackSparksSystems);
    }

    void StopRideSparks()
    {

        ParticleSystemPlayer.StopChildParticleSystems(trackSparksSystems);
    }

    List<Vector3> DeterminePathPointsFromCurrentPoint(List<Vector3> pathPoints, Vector3 currentPoint, Vector3 switchToTravelToPosition)
    {
        //this method cuts the plotted path to only take into account the shortest part of the path starting from a point where the character overlapped or was closest to.
        //it will take the starting point point, find wether the point to its left or right is closest to the destination switch, and add those points t o a new list.

        //Vector3 pointInOurDirection = FindClosest.ClosestOfTwo() 
        //TODO: Have to determine the direction that it's going to travel depending on which switch is closest -- SEE ABOVE 
        //TODO: MAke sure the origin point isn't the first point in the list
        Vector3 point1 = Vector3.zero;
        Vector3 point2 = Vector3.zero;
        List<Vector3> truncatedPoints = new List<Vector3>();
        int desiredIndex = 0;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (pathPoints[i] == currentPoint)
            {
                point1 = pathPoints[i + 1];
                point2 = pathPoints[i - 1];
                desiredIndex = i;
                break;
            }
        }
        float distanceFromSwitch1 = Vector3.Distance(switchToTravelToPosition, point1);
        float distancefromswitch2 = Vector3.Distance(switchToTravelToPosition, point2);
        if (distanceFromSwitch1 < distancefromswitch2)
        {
            //if i + 1 is closer to the destination switch than i - 1, we want to travel from i (currentPoint) in the direction of i++ to the destination switch

            //this switch is physically closer to the switch, but it doesn't mean the list is in the right direction, so test if it's also
            //less than or greater than the current point.
            //travel in this dierction
            //add a list and remove everything before this point
            for (int i = desiredIndex; i < pathPoints.Count; i++)
            {
                truncatedPoints.Add(pathPoints[i]);
            }
        }
        else if (distancefromswitch2 < distanceFromSwitch1)
        {
            //travel in the other direction
            //add a list and remove everything after this ponit
            for (int i = desiredIndex; i < pathPoints.Count; i--)
                truncatedPoints.Add(pathPoints[i]);
        }
        //TODO: FINISH THIS
        return truncatedPoints;
    }



    IEnumerator RideTemporarySwitchConnection(List<Vector3> points)
    {
        //this takes the list of points left by a temporary connection dropped by the comet
        //if the player begins ON a connection, it will start from there
        Vector3 pointToReach = points[0];
        int index = 0;
        riding = true;
        float time = 0.0f;
        float startTime = Time.time;
        previousSwitch = currentSwitchGO;
        GameObject beginningSwitch = currentSwitchGO;
        GameObject endingSwitch = destinationSwitchGO;
        Vector2 startPosition = transform.position;
        Vector2 point = new Vector2(0, 0);
        Debug.Log("This is how many points we have " + points.Count);
        while (index < points.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[index], Mathf.SmoothStep(0.0f, 1.0f, time));
            index++;
            time += Time.deltaTime / 1.0f;
            yield return null;
        }
        if (CheckForConnection())
        {
            if (!connectionWereRiding.temporary)
            {
                StartCoroutine(RideSwitchConnection(currentSwitch.transform.position));
            }
            else if (connectionWereRiding.temporary)
            {
                StartCoroutine(RideTemporarySwitchConnection(connectionWereRiding.pathPoints));
            }
            yield break;
        }

        NoLongerRiding();


        // while (Time.time < startTime + extendableIntervalDuration)
        // {
        //     float distanceFromPoint = Vector2.Distance(transform.position, point);
        //     float distanceFromNextSwitch = Vector2.Distance(transform.position, endingSwitch.transform.position);
        //     if (/*currentSwitchGO == endingSwitch &&*/ distanceFromNextSwitch < 0.4f)
        //     {
        //         //the logic here is that we want to be on the new switch, but it's going to likely be inaccurate if we don't make sure they're on position first
        //         //this starts the process over from the switch we reached, checking if a connection exists, finding the closest and cancelling this coroutine
        //         if (CheckForConnection())
        //         {
        //             if (!connectionWereRiding.temporary)
        //             {
        //                 StartCoroutine(RideSwitchConnection());
        //             }
        //             else if (connectionWereRiding.temporary)
        //             {
        //                 StartCoroutine(RideTemporarySwitchConnection(connectionWereRiding.pathPoints));
        //             }
        //         }

        //         yield break;
        //     }
        //     if (distanceFromPoint < 0.3f)
        //     {
        //         startPosition = pointToReach;
        //         index++;
        //         pointToReach = points[index];
        //     }
        //     time += Time.deltaTime / 1.0f;
        //     transform.position = Vector2.Lerp(startPosition, pointToReach, Mathf.SmoothStep(0.0f, 1.0f, time));
        //     yield return null;
        // }
    }
    IEnumerator RideSwitchConnection(Vector3 beginningPosition)
    {
        riding = true;
        float time = 0.0f;
        float startTime = Time.time;
        previousSwitch = currentSwitchGO;
        GameObject beginningSwitch = currentSwitchGO;
        GameObject endingSwitch = destinationSwitchGO;
        Vector2 startPosition = transform.position;
        Vector2 point = new Vector2(0, 0);

        if (!connectionWereRiding.temporary)
        {
            while (Time.time < startTime + extendableIntervalDuration)
            {
                float distance = Vector2.Distance(transform.position, endingSwitch.transform.position);
                if (currentSwitchGO == endingSwitch && distance < 0.4f)
                {
                    //the logic here is that we want to be on the new switch, but it's going to likely be inaccurate if we don't make sure they're on position first
                    //this starts the process over from the switch we reached, checking if a connection exists, finding the closest and cancelling this coroutine
                    if (CheckForConnection())
                    {
                        if (!connectionWereRiding.temporary)
                        {
                            //TODO: This is going tbe null probably
                            StartCoroutine(RideSwitchConnection(currentSwitch.transform.position));
                        }
                        else if (connectionWereRiding.temporary)
                        {
                            StartCoroutine(RideTemporarySwitchConnection(connectionWereRiding.pathPoints));
                        }
                    }

                    yield break;
                }
                time += Time.deltaTime / 1.0f;
                if (Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonDown(1))
                {
                    //if we let go of e or hit the mouse button to slingshot, break us out of this
                    //Debug.Log("We're no longer holding on!");
                    break;
                }

                transform.position = Vector2.Lerp(beginningPosition, endingSwitch.transform.position, Mathf.SmoothStep(0.0f, 1.0f, time));

                yield return null;
            }
        }

        NoLongerRiding();
    }

    public IEnumerator RideCurvedSwitchConnection(Vector2 point)
    {
        riding = true;
        Vector2 startPosition = transform.position;
        float time = 0.0f;
        while (true)
        {
            time += Time.deltaTime / 1.0f;
            transform.position = Vector3.Lerp(startPosition, point, Mathf.SmoothStep(0.0f, 1.0f, time));
        }
    }

    void NoLongerRiding()
    {
        Debug.Log("WE've stopped riding switches");
        riding = false;
        switchConnectionsWeveRidden.Clear();
        connectionWereRiding = null;
        destinationSwitchGO = null;
        currentSwitch = null;
        StoppedUsingPowerUpWrapper();
    }

    // void DetermineDirection()
    // {
    //     List<GameObject> connectedSwitches = pReference.locationHandler.currentSwitch.GetComponent<Switch>().connectedSwitches;



    // }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

    }
}
