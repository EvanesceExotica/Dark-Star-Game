using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Pulse : MonoBehaviour
{

    public LayerMask NoLayers;
    public LayerMask AllLayers;
    public LayerMask NotPlayer;
    public GameObject pulseObjectPrefab;
    public GameObject pulseObjectInstantiated;
    PointEffector2D ourPointEffector;

    [SerializeField]
    GameObject supernovaGO;
    List<ParticleSystem> supernovaParticleSystems;

    [SerializeField]
    GameObject blackHoleGO;
    List<ParticleSystem> blackHoleParticleSystems;
    public static event Action DisasterCompleted;

    bool cantPulse = false;

    void TurnOffPulse()
    {
        Doomclock.TriggerDisaster -= DisasterTriggeredPulse;
    }

    public static event Action<bool> DisasterBegun;

    void ChangePlayerAnchorStatus(bool anchored)
    {
        if (!anchored)
        {
            SetColliderLayerToIncludePlayer();
        }
        else
        {
            SetcolliderLayerToExcludePlayer();
        }
    }
    void CompleteDisaster()
    {
        SetColliderLayerToIncludeNothing();
        if (DisasterCompleted != null)
        {
            DisasterCompleted();
        }
    }

    void BeginDisaster(bool playerAnchored)
    {
        //First determine if the player is anchored.  If so, set the point effector to
        //effect all layers EXCLUDING the player
        //If not, set the point effector to effect all layers INCLUDING the player.
        //Then, trigger the "Disaster Begun" event
        //TODO: We want to change this to also include anchored enemies -- maybe just make an "anchored" layer?

        if (ourPointEffector.useColliderMask == false)
        {
            ourPointEffector.useColliderMask = true;
        }
        if (playerAnchored)
        {
            SetcolliderLayerToExcludePlayer();
        }
        else
        {
            SetColliderLayerToIncludePlayer();
        }

        if (DisasterBegun != null)
        {
            //this is a problem if the player deanchors during the disaster
            DisasterBegun(playerAnchored);
        }
    }


    void SetColliderLayerToIncludePlayer()
    {
        //Debug.Log("Stack overflow happening because of me?");
        ourPointEffector.colliderMask = AllLayers;
    }

    void SetcolliderLayerToExcludePlayer()
    {
        ourPointEffector.colliderMask = NotPlayer;
    }

    void SetColliderLayerToIncludeNothing()
    {
        ourPointEffector.colliderMask = NoLayers;
    }

    void DisasterTriggeredPulse()
    {
        StartCoroutine(ImplodeAndPulseOut());
    }

    private bool cancelWait = false;

    IEnumerator Wait(float waitTime)
    {
        float t = 0.0f;
        while (t <= waitTime && !cancelWait)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    void ChangeInterval(float penalty)
    {
        //Debug.Log("Interval Changed");
        cancelWait = true;
        pulseInterval -= penalty;
        cancelWait = false;
    }

    public LayerMask affectedLayers;

    float pulseInterval;
    float minIntervalValue;
    float maxIntervalValue;

    bool canPulse;

    void StartItermittentPulses()
    {
        canPulse = true;
        StartCoroutine(PulseItermittently());
    }

    IEnumerator PulseItermittently()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(minIntervalValue, maxIntervalValue));
        while (canPulse)
        {

            pulseInterval = UnityEngine.Random.Range(minIntervalValue, maxIntervalValue);

            yield return StartCoroutine(Wait(pulseInterval));

        }
    }

    private void Start()
    {
        blackHoleGO = transform.Find("BlackHole").gameObject;
        supernovaGO = transform.Find("Supernova").gameObject;
        supernovaParticleSystems = supernovaGO.GetComponentsInChildren<ParticleSystem>().ToList();
        blackHoleParticleSystems = blackHoleGO.GetComponentsInChildren<ParticleSystem>().ToList();
        
        StartItermittentPulses();
        DisablePointEfectorAndCollider();
        SetColliderLayerToIncludeNothing();
    }

    private void OnEnable()
    {
        // DarkStar.IlluminationAtMax += this.StartItermittentPulses;
    }

    private void OnDisable()
    {
        //DarkStar.IlluminationAtMax -= this.StartItermittentPulses;
    }

    void DisablePointEfectorAndCollider(){
        ourCollider.enabled = false;
        ourPointEffector.enabled = false;

    }

    void EnablePointEffectorAndCollider(){
        ourCollider.enabled = true;
        ourPointEffector.enabled = true;
    }

    // Use this for initialization

    PlayerReferences pReference;

    Collider2D ourCollider;
    void Awake()
    {
        LocationHandler.AnchorStatusChanged += this.ChangePlayerAnchorStatus;
        DarkStar.AugmentDoomTimer += this.ChangeInterval;
        ourPointEffector = GetComponent<PointEffector2D>();
        ourCollider = GetComponent<Collider2D>();
        ourPointEffector.colliderMask = NoLayers;
        //  ourPointEffector.enabled = false;
        pReference = GameObject.Find("Player").GetComponent<PlayerReferences>();
        Doomclock.TriggerDisaster += DisasterTriggeredPulse;
        GameStateHandler.DarkPhaseStarted += this.TurnOffPulse;


    }
    void DetermineIfPlayerWasAnchored()
    {
        //Destroy conduits if player was not anchored 
    }


    IEnumerator ImplodeAndPulseOut()
    {
        // ourPointEffector.colliderMask = NotPlayer;
        //  ourPointEffector.enabled = true;


        //THIS WILL DETERMINE WHETHER OR NOT THE PLAYER IS CURRENTLY ANCHORED BEFORE IT DESTROYS SHIT
        EnablePointEffectorAndCollider();
        BeginDisaster(pReference.locationHandler.anchored);
        ParticleSystemPlayer.PlayChildParticleSystems(blackHoleParticleSystems);
        ourPointEffector.forceMagnitude = -200.0f;
        yield return new WaitForSeconds(10.0f);
        ParticleSystemPlayer.StopChildParticleSystems(blackHoleParticleSystems);
        yield return new WaitForSeconds(2.0f);
        ParticleSystemPlayer.PlayChildParticleSystems(supernovaParticleSystems);
        ourPointEffector.forceMagnitude = 100.0f;

        yield return new WaitForSeconds(2.0f);
        ourPointEffector.forceMagnitude = 0;
        DisablePointEfectorAndCollider();
        CompleteDisaster();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(ImplodeAndPulseOut());
            // pulseObjectInstantiated.SetActive(true);
        }
    }
}
