using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hookshot : MonoBehaviour
{

    List<GameObject> hookedObjects = new List<GameObject>();

    GameObject player;
    SpaceMovement playerMovement;
    public Transform originFromPlayer;
    Rigidbody2D ourRigidbody;
    LineRenderer chainRenderer;
    public float chainMaxLength;
    bool throwing;
    bool retracting;
    public float throwSpeed;

    public LayerMask whatIsHookable;
    public bool hookedOn;
    public bool pulledTaut;

    MeshRenderer ourMeshRenderer;
    public GameObject hookedObject;
    public GameObject previouslyHookedObject;
    bool playerArrivedAtHook;
    string alreadyhookableLayerName;
    string hookableLayerName;
    List<GameObject> chainRendererVertexPositions;
    public List<LayerMask> hookableLayers;

    PlayerReferences pReference;

    public Vector3 hitPoint;
    public static event Action<GameObject> ObjectHooked;
    public static event Action Scoping;

    public static event Action StoppedScoping;

    void ScopingWrapper(){
        Debug.Log("<color=green>We're started scoping</color>");
        if(Scoping != null){
            Scoping();
        }
    }

    void StoppedScopingWrapper(){
        if(StoppedScoping != null){
            StoppedScoping();
        }
    }


    void HookedAnObject(GameObject hookedObj)
    {
        if (ObjectHooked != null)
        {
            ObjectHooked(hookedObj);
        }
    }

    void Awake()
    {
        PlayerHealth.PlayerDied += this.ReleaseAndBreak;
    }

    void OnDisable()
    {
        PlayerHealth.PlayerDied += this.ReleaseAndBreak;
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        pReference = player.GetComponent<PlayerReferences>();
        //        playerMovement = player.GetComponent<PlayerMovement>();
        ourRigidbody = GetComponent<Rigidbody2D>();
        transform.position = originFromPlayer.position;
        throwing = false;
        ourMeshRenderer = GetComponent<MeshRenderer>();
        alreadyhookableLayerName = "CurrentlyHooked";
        hookableLayerName = "Hookable";
        chainRenderer = GetComponent<LineRenderer>();

    }

    float howLongFireHeld = 0;
    bool holding = false;

    bool holdingThresholdPassed = false;
    bool waitingForDoubleTap;
    float waitingForDoubleTapTime = 0.0f;
     float doubleTapThreshold = 0.5f;
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Q) && !waitingForDoubleTap){
            waitingForDoubleTap = true;
        }
        if(waitingForDoubleTap){
            if(Input.GetKeyDown(KeyCode.Q)){
                //WE DOUBLE TAPPED
                ScopingWrapper();
            }
            waitingForDoubleTapTime+=Time.deltaTime;
            if(waitingForDoubleTapTime >= doubleTapThreshold){
                waitingForDoubleTapTime = 0.0f;
                waitingForDoubleTap = false;
            }
        }
        // if(Input.GetKeyDown(KeyCode.Q) && !throwing)
        // {
        //     holding = true;
        // }
        // if(holding == true){
        //     howLongFireHeld +=  Time.deltaTime;
        //     if(howLongFireHeld >= 0.5f){

        //         holdingThresholdPassed = true;
        //     }
        // }
        // if(Input.GetKeyUp(KeyCode.Q) && !throwing){
        //     if(howLongFireHeld <  0.5f){
        //        //ScopingWrapper(); 
        //        StartCoroutine(TossChain());
        //        StartCoroutine(UpdateChainVisuals());
        //     }
        //     // else{
        //     //     StartCoroutine(TossChain());
        //     //     StartCoroutine(UpdateChainVisuals());

        //     // }
        //     holding = false;
        // }
        // else if(Input.GetKeyUp(KeyCode.Q) && throwing && holding){

        //     StoppedScopingWrapper();
        //     holding = false;
        // }
        // if(holdingThresholdPassed && !throwing){
        //     ScopingWrapper();
        // }
        if (Input.GetKeyDown(KeyCode.Q) && !throwing)
        {
            StartCoroutine(TossChain());
            StartCoroutine(UpdateChainVisuals());
        }
        if (throwing || retracting || hookedOn)
        {
            ourMeshRenderer.enabled = true;
            chainRenderer.enabled = true;
        }

        else
        {
            ourMeshRenderer.enabled = false;
            chainRenderer.enabled = false;
        }

        if (hookedOn && hookedObject != null)
        {
            transform.parent = hookedObject.transform;
            player.transform.parent = hookedObject.transform;
            pReference.playerMovement.incapacitated = true;
        }
        else
        {
            pReference.playerMovement.incapacitated = false;
            //TODO: Change this so that there's a list of "movementInhibitors".
        }
        if (hookedOn && hookedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                ReleaseHook();
            }
            if (pReference.locationHandler.onPlanet)
            {

                ReleaseHook();
            }
            if (pReference.locationHandler.currentSwitch != null && Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Hook released");
                ReleaseHook();
            }

        }
    }

    void Bash()
    {

    }



    void PullHookedObjToMe()
    {

    }

    void PullMeToHookedObj()
    {


    }

    void Swing()
    {

    }

    public IEnumerator UpdateChainVisuals()
    {
        chainRenderer.enabled = true;
        Vector2 playerPosition = player.transform.position;
        Vector2 hookPosition = transform.position;
        while (throwing || retracting || hookedOn)
        {
            playerPosition = player.transform.position;
            hookPosition = transform.position;

            if (chainRenderer != null)
            {
                chainRenderer.SetPosition(0, playerPosition);
                chainRenderer.SetPosition(1, hookPosition);

            }

            yield return null;
        }
        chainRenderer.enabled = false;
    }

    void PullObjectToUs(Collider2D ours, Collider2D theOneHit)
    {

    }

    public IEnumerator TossChain()
    {
        //place two raycasts for more accuracy 
        Collider2D ourHit = null; // Physics2D.Raycast(transform.position, transform.right, 1.0f, whatIsHookable);
        Debug.DrawRay(transform.position, transform.right * 30, Color.blue);

        transform.position = player.transform.position;
        throwing = true;
        bool hooked = false;
        ourRigidbody.bodyType = RigidbodyType2D.Dynamic;

        Vector2 origin = player.transform.position;
        Vector2 mousePos = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        Vector2 trans = mousePositionWorld - origin;
        trans.Normalize();


        while (Mathf.Abs((Vector2.Distance(origin, transform.position))) <= chainMaxLength)
        {
            ourHit = Physics2D.OverlapCircle(transform.position, 1.0f, whatIsHookable);
            // //Debug.Log("we hit " + ourHit.gameObject.name + " and is it " + pReference.locationHandler.currentSwitch.name + " ");
            // ourHit = Physics2D.Raycast(transform.position, transform.right, 1.0f, whatIsHookable);
            if (ourHit && ourHit.gameObject != hookedObject)
            {
                if (pReference.locationHandler.onPlanet && ourHit.gameObject == pReference.locationHandler.currentPlanet || ourHit.gameObject == pReference.locationHandler.currentSwitch)
                {
                    hooked = false;
                }

                else
                {
                    hooked = true;
                }
            }

            if (hooked)
            {
                break;
            }
            ourRigidbody.velocity = trans * throwSpeed;
            var dir = (Vector3)mousePositionWorld - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


            yield return null;

        }
        throwing = false;
        ZeroOutVelocity();

        if (hooked && ourHit.gameObject != hookedObject && ourHit.gameObject != pReference.locationHandler.currentSwitch)
        {

            ObjectHooked(ourHit.gameObject);
            IPullable pullableObject = ourHit.GetComponent<IPullable>();
            if (pullableObject != null)
            {
                Debug.Log("Pullable object does not equal null");

                pullableObject.BeginPull(player.transform);
            }
            else
            {
                Debug.Log("We're moving toward hookshot");
                // //Debug.Log("Terrain hook " + ourHit.gameObject.name);
                MoveToHookShot(transform.position, ourHit.gameObject);
                DetermineHookedObject(ourHit.gameObject);
            }
            //  


        }
        else
        {
            StartCoroutine(ReturnChain());
        }




    }


    void DetermineHookedObject(GameObject hookedObj)
    {
        if (hookedObj != null)
        {
            hookedObject = hookedObj;
            HookedAnObject(hookedObj);

            if (hookedObject != null)
            {
                previouslyHookedObject = hookedObject;


                hookedObject = hookedObj;

                //  hookedObject.layer = LayerMask.NameToLayer(alreadyhookableLayerName);


            }
        }
        else
        {
            //Debug.Log("Not hooked");
        }

    }

    void IgnoreCollisionWhileHooked()
    {

    }

    void MoveToHookShot(Vector2 hookPosition, GameObject hookedGO)
    {


        //hitPoint = hit.point;
        hookedOn = true;
        ourRigidbody.bodyType = RigidbodyType2D.Kinematic;
        //if (hit)
        //{
        //   
        pReference.playerMovement.MoveToHookshot(hookPosition, hookedGO);
        //}
        //else
        //{
        //    //Debug.Log("hit nothing");
        //}
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(hitPoint, Color.red, 4.0f);

    }

    public IEnumerator ReturnChain()
    {


        retracting = true;


        while (Mathf.Abs((Vector2.Distance(player.transform.position, transform.position))) > 0.5f)
        {

            Vector2 playerPosition = player.transform.position;
            Vector2 hookPosition = transform.position;
            Vector2 targetDirection = (Vector2)Vector3.Normalize(playerPosition - hookPosition);

            ourRigidbody.velocity = new Vector2(targetDirection.x * throwSpeed, targetDirection.y * throwSpeed);
            yield return null;

        }
        ZeroOutVelocity();
        ourRigidbody.bodyType = RigidbodyType2D.Kinematic;
        retracting = false;

    }

    void ReleaseAndBreak(float lightPenalty)
    {

        ReleaseHook();
        BreakHooked();
    }

    void ReleaseHook()
    {
        //this method will stop the player from being pulled/hooked onto something static
        hookedOn = false;
        transform.parent = null;
        player.transform.parent = null;
        previouslyHookedObject = hookedObject;
        hookedObject = null;
        pReference.playerMovement.SetRigidbodyDynamic();
    }

    void BreakHooked()
    {
        //this method will stop a pullableobject from being hooked toward the player
        IPullable pullableObject = null;
        if (hookedObject != null)
        {
            pullableObject = hookedObject.GetComponent<IPullable>();
        }
        if (pullableObject != null)
        {
            pullableObject.CancelPull();
        }
    }

    void ZeroOutVelocity()
    {
        ourRigidbody.velocity = new Vector2(0.0f, 0.0f);

    }




}
