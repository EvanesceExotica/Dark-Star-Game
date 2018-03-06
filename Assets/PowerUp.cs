using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PowerUp : MonoBehaviour
{

    public float powerUpUseWindowDuration;
    public float extendableIntervalDuration;
    public bool extendableDuration;
    public virtual void Awake()
    {
        //this is a bit wonky but I think it should work?
        NowUsingPowerUp += CantStartPowerUp;
        NowUsingPowerUp += CurrentlyUsingPowerUp;
        StoppedUsingPowerUp += NotCurrentlyUsingPowerUp;
        Switch.SwitchEntered += SetOnSwitch;
        Switch.SwitchExited += SetOffSwitch;
    }
    //this maybe should be static so that it cancels out the powerups for all 6 powerups
    public static event Action NowUsingPowerUp;

    public void NowUsingPowerUpWrapper()
    {
        //currentlyUsingPowerUp = true;
        if (NowUsingPowerUp != null)
        {
            NowUsingPowerUp();
        }
    }
    public static event Action StoppedUsingPowerUp;

    public void StoppedUsingPowerUpWrapper()
    {
       // currentlyUsingPowerUp = false;
        if (StoppedUsingPowerUp != null)
        {
            StoppedUsingPowerUp();
        }
    }

    void CurrentlyUsingPowerUp()
    {
        currentlyUsingPowerUp = true;
    }

    void NotCurrentlyUsingPowerUp()
    {
        currentlyUsingPowerUp = false;
    }

    void CantStartPowerUp()
    {
        requirementsForPowerUpStartSatisfied = false;
    }
    public enum Requirement
    {
        OnlyUseOnSwitch,
        OnlyUseOffSwitch
    }

    public Requirement ourRequirement;

    public bool autoActivated;
    public bool requirementsForPowerUpStartSatisfied;

    public bool onSwitch;

    public bool PoweredUp;

    public bool currentlyUsingPowerUp;

    public void SetPoweredUp()
    {
        PoweredUp = true;
        if (ourRequirement == Requirement.OnlyUseOnSwitch)
        {
            if (onSwitch)
            {
                //if we're also on a switch, we can chain enemy now
                if (!currentlyUsingPowerUp)
                {
                    requirementsForPowerUpStartSatisfied = true;
                }
            }
        }
        else if (ourRequirement == Requirement.OnlyUseOffSwitch)
        {
            if (!onSwitch)
            {
                if (!currentlyUsingPowerUp)
                {
                    requirementsForPowerUpStartSatisfied = true;
                }
            }
        }
    }

    public void RemovePoweredUp()
    {
        //both powered up and on switch hae to be true to chain enemy, so set canChainEnemy to false
        PoweredUp = false;
        requirementsForPowerUpStartSatisfied = false;
    }


    public virtual void SetOnSwitch(GameObject ourSwitch)
    {
        //we're on a switch
        onSwitch = true;
        if (ourRequirement == Requirement.OnlyUseOffSwitch)
        {
            requirementsForPowerUpStartSatisfied = false;
        }
        else
        {
            if (PoweredUp)
            {
                if (!currentlyUsingPowerUp)
                {
                    requirementsForPowerUpStartSatisfied = true;
                }
            }
        }


    }

    public virtual void SetOffSwitch(GameObject ourSwitch)
    {
        //we're off of a switch
        onSwitch = false;
        if (ourRequirement == Requirement.OnlyUseOnSwitch)
        {
            //if we can only use this powerup off of a switch, we immediately can't use it
            requirementsForPowerUpStartSatisfied = false;
        }
        else
        {
            //but if we can use this powerup off of a switch
            if (PoweredUp)
            {
                //and we're powered up, and not already using a powerup, we can use it!
                if (!currentlyUsingPowerUp)
                {
                    requirementsForPowerUpStartSatisfied = true;
                }
            }
        }

    }

    public virtual void StartPowerUp()
    {
        NowUsingPowerUpWrapper();
    }

    public virtual void Update()
    {
        if (autoActivated && requirementsForPowerUpStartSatisfied)
        {
            Debug.Log("Activating  power up! " + this.GetType().Name);
            StartPowerUp();
        }
        else if (!autoActivated && requirementsForPowerUpStartSatisfied)
        {
            Debug.Log("PRESS E TO ACTIVATE POWER UP " + this.GetType().Name);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartPowerUp();
            }
        }
        // if(currentlyUsingPowerUp){
        //     //if we're using the powerup right now, we don't want to start using it again
        //     canStartPowerUp = false;
        // }
    }
    // Use this for initialization

}
