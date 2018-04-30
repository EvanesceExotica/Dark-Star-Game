using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour {

    GameObject player;
    public float speed;
    public Rigidbody2D rb;
    public float jumpForce;
    public TrailRenderer trailRenderer;
    public PlayerMovement playerMovement;
    public Transform spawner;
    public GameObject groundCheck;
    public GameObject ceilingCheck;
    public GameObject darkStar;
    public SoulHandler playerSoulHandler;
    public Bash bash;
    public PullToSwitch switchPuller;
    public StarBash starBash;

    public LocationHandler locationHandler;

    public SpriteRenderer ourSpriteRenderer;
    public PlayerHealth playerHealth;
	// Use this for initialization
    public SpacetimeSlingshot slingshot;
    public LaunchSoul launchSoul;
    public PowerupHandlerDeprecated powerupHandler;

    public PlayerTriggerHandler triggerHandler;

    public GameObject switchHolder;
	void Awake () {
        speed = 5.0f;
        jumpForce = 500f;
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        ourSpriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<SpaceMovement>();
        locationHandler = GetComponent<LocationHandler>();
        darkStar = GameObject.Find("Dark Star");
        playerSoulHandler = GetComponent<SoulHandler>();
        bash = GetComponentInChildren<Bash>();
        switchPuller = GetComponentInChildren<PullToSwitch>();
        starBash = GetComponent<StarBash>();
        playerHealth = GetComponent<PlayerHealth>();
        slingshot = GetComponent<SpacetimeSlingshot>();
        launchSoul = GetComponent<LaunchSoul>();
        powerupHandler = GetComponent<PowerupHandlerDeprecated>();
        triggerHandler = GetComponentInChildren<PlayerTriggerHandler>();
        switchHolder = GameObject.Find("Switch Holder");
    }

    private void OnEnable()
    {
        LocationHandler.MovementTypeSwitched += SetPlayerMovement;
    }

    private void OnDisable()
    {
        LocationHandler.MovementTypeSwitched -= SetPlayerMovement;
    }

    void SetPlayerMovement(PlayerMovement movement)
    {
        playerMovement = movement;
    }

    private void ResetPositionToSpawn()
    {
        transform.position = spawner.position;
    }

    private void Update()
    {
        if (locationHandler.inVoid && Input.GetKeyDown(KeyCode.Space))
        {
            ResetPositionToSpawn();
        }
       // playerMovement = locationHandler.currentMovement;
    }

}
