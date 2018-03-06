using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    PlayerMovement currentMovement;
    SpaceMovement spaceMovement;
    PlanetMovement planetMovement;

    LocationHandler locationHandler;

    Shoot shoot;
    float horizontal;
    float vertical;
    bool jump;
    bool fire;
    float minChargeTime;

    float pressStartTime;




    private void Awake()
    {
        spaceMovement = gameObject.GetComponent<SpaceMovement>();
        planetMovement = gameObject.GetComponent<PlanetMovement>();
        currentMovement = spaceMovement;
        locationHandler = GetComponent<LocationHandler>();
        shoot = gameObject.GetComponent<Shoot>();
        minChargeTime = 1.0f;
    }

    public enum ShootType
    {
        Flare,
        Laser
    }

    public ShootType ourShootType;
    void SwitchShootTypeToLaser(GameObject irrelevant)
    {

        ourShootType = ShootType.Laser;
    }

    void SwitchShootTypeToFlare(GameObject irrelevant)
    {
        ourShootType = ShootType.Flare;
    }
    private void OnEnable()
    {
        LocationHandler.MovementTypeSwitched += this.SwitchMovement;
        Switch.SwitchEntered += this.SwitchShootTypeToLaser;
        Switch.SwitchExited += this.SwitchShootTypeToFlare;
    }

    private void OnDisable()
    {
        LocationHandler.MovementTypeSwitched -= this.SwitchMovement;
    }

    // Use this for initialization
    void Start()
    {

    }


    void SwitchMovement(PlayerMovement ourTypeToSwitchTo)
    {
        if (currentMovement == spaceMovement)
        {
            currentMovement = planetMovement;
        }
        else if (currentMovement == planetMovement)
        {
            currentMovement = spaceMovement;
        }
    }

    private void Update()
    {



        if (!jump)
        {
            jump = Input.GetKeyDown(KeyCode.Space);

        }

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    shoot.Fire();
        //}

        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     if (ourShootType == ShootType.Flare && shoot.allowFire)
        //     {
        //         pressStartTime = Time.time;
        //     }

        // }


        // if (Input.GetKeyUp(KeyCode.F))
        // {
        //     if (ourShootType == ShootType.Flare && shoot.allowFire)
        //     {
        //         if (Time.time - pressStartTime < minChargeTime)
        //         {
        //             shoot.Fire();
        //         }
        //         else
        //         {
        //             shoot.ChargeFire();
        //         }
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     shoot.LaserFire();
        // }

        currentMovement.Move(horizontal, vertical, jump);

    }

    // Update is called once per frame


    // Update is called once per frame
    void FixedUpdate()
    {



        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");


        //   currentMovement.Move(horizontal, vertical, jump);

        jump = false;


    }
}
