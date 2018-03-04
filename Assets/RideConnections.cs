using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class RideConnections : PowerUp
{

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


    //let's just have this travel clockwise for now 

    void SetCurrentSwitch(GameObject switchGO)
    {
        currentSwitchGO = switchGO;
        currentSwitch = currentSwitchGO.GetComponent<Switch>();
    }

    public override void StartPowerUp(){
        LerpToNearestSwitch();
    }

    void RemoveCurrentSwitch(GameObject switchGO)
    {
        currentSwitchGO = null;
        currentSwitch = null;
    }
    void TestMethod()
    {

    }
    bool CheckForConnection()
    {
        //for now let's just find the closest switch with a connection
        bool connectionExists = false;
        if (currentSwitch == null)
        {
            Debug.Log("Current switch is null is the issue");
        }

        if (currentSwitch.connectedSwitches != null && currentSwitch.connectedSwitches.Count > 0)
        {
            destinationSwitchGO = FindNearestConnectedSwitchToCurrentSwitch();
            if (destinationSwitchGO != null)
            {
                Debug.Log("We've found a connection!");
                //if there is a connection and the connection isn't where we came from, a connection we can use exists
                connectionExists = true;
            }



        }
        return connectionExists;
    }

    public override void Awake()
    {
        base.Awake();
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


    GameObject FindNearestSwitch()
    {
        GameObject nearestSwitch = FindClosest.FindClosestObject(switchHolder.ourSwitchGameObjects, gameObject);
        return nearestSwitch;
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

    public IEnumerator LerpToNearestSwitch(){

        float time = 0.0f;
        Transform transformToJumpTo = null;
        if(currentSwitch == null){
            currentSwitchGO = FindNearestSwitch();
            transformToJumpTo = currentSwitchGO.transform;

        }
        pullToSwitchLineRenderer.enabled = true;
        while(Vector2.Distance(transform.position, transformToJumpTo.position) > 0.5f){
            if(Input.GetKeyUp(KeyCode.E)){
                yield break;
            }
            time += Time.deltaTime / 1.0f;
            transform.position = Vector2.Lerp(transform.position, transformToJumpTo.position, Mathf.SmoothStep(0.0f, 1.0f, time));
            yield return null;

        }
        pullToSwitchLineRenderer.enabled = false;
        if (CheckForConnection() == true){
            
                Debug.Log("A connection was found!");
                //if a connection exists between this switch and another;
                StartCoroutine(RideSwitch());
            
        }
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
                StartCoroutine(RideSwitch());
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

    IEnumerator RideSwitch()
    {
        riding = true;
        float time = 0.0f;
        previousSwitch = currentSwitchGO;
        GameObject beginningSwitch = currentSwitchGO;
        GameObject endingSwitch = destinationSwitchGO;
        while (true)
        {
            float distance = Vector2.Distance(transform.position, endingSwitch.transform.position);
            if (currentSwitchGO == endingSwitch && distance < 0.4f)
            {
                //the logic here is that we want to be on the new switch, but it's going to likely be inaccurate if we don't make sure they're on position first
                //this starts the process over from the switch we reached, checking if a connection exists, finding the closest and cancelling this coroutine
                if (CheckForConnection())
                {
                    StartCoroutine(RideSwitch());
                }
                yield break;
            }
            time += Time.deltaTime / 1.0f;
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Debug.Log("We're no longer holding on!");
                break;
            }
            transform.position = Vector2.Lerp(beginningSwitch.transform.position, endingSwitch.transform.position, Mathf.SmoothStep(0.0f, 1.0f, time));
            yield return null;
        }
        riding = false;
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
