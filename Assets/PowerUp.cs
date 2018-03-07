using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PowerUp : MonoBehaviour
{

    //Part of the issue of not starting immediately
    //Is it makes no sense to start 
    public float powerUpUseWindowDuration;
    public float extendableIntervalDuration;

    public float activationWindowDuration;
    public bool extendableDuration;
    public virtual void Awake()
    {
        powerUpUseWindowDuration = 15.0f;
        activationWindowDuration = 5.0f;
        //this is a bit wonky but I think it should work?
        PowerUpChosen += CurrentlyUsingPowerUp;
        StoppedUsingPowerUp += NotCurrentlyUsingPowerUp;
        Switch.SwitchEntered += SetOnSwitch;
        Switch.SwitchExited += SetOffSwitch;

    }
    //this maybe should be static so that it cancels out the powerups for all 6 powerups
    public static event Action PowerUpChosen;

    public static event Action<PowerUp> PowerUpActivated;


    public IEnumerator ActivationWindowCountdown(){
        float startTime = Time.time;
        while(Time.time <  startTime + activationWindowDuration){
            if(Input.GetKeyDown(KeyCode.E)){
                Debug.Log("Ability was activated before the window was used");
                yield break;
            }
            yield return null;
        }
        StoppedUsingPowerUpWrapper();
    }
    public IEnumerator CountdownOverarchingDuration(){
        //this coroutine will countdown the overarching duration of the powerup. If the player sits and does nothing, it'll eventually time out based on the duration
        //DropTrack and RideConnection can be slightly extended by reaching new switches but shouldn't go on indefinitely.
        yield return new WaitForSeconds(powerUpUseWindowDuration);
        StoppedUsingPowerUpWrapper();

    }
    public void NowUsingPowerUpWrapper()
    {
        //currentlyUsingPowerUp = true;
        if (PowerUpChosen != null)
        {
            PowerUpChosen();
        }
    }
    public static event Action StoppedUsingPowerUp;

    public void StoppedUsingPowerUpWrapper()
    {
        //TODO: FIX THIS SHIT -- THE ABILITIES AREN'T ACTIVATING FOR SOME REASON
        poweredUp = false;
       // currentlyUsingPowerUp = false;
        if (StoppedUsingPowerUp != null)
        {
            StoppedUsingPowerUp();
        }
    }

    void CurrentlyUsingPowerUp()
    {
        currentlyUsingAnyPowerUp = true;
    }

    void NotCurrentlyUsingPowerUp()
    {

        
        currentlyUsingAnyPowerUp = false;
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

    public bool poweredUp;

    public bool currentlyUsingAnyPowerUp;

    public void SetPoweredUp()
    {
        //this happens when the charge of the soul after being sucked in is CONSUMED and already chosen
        poweredUp = true;
       
        if (ourRequirement == Requirement.OnlyUseOnSwitch)
        {
            if (onSwitch)
            {
                Debug.Log("We've been satisfied " + this.GetType().Name);
                //if we're also on a switch, we can chain enemy now
                
                    requirementsForPowerUpStartSatisfied = true;
            }
        }
        else if (ourRequirement == Requirement.OnlyUseOffSwitch)
        {
            if (!onSwitch)
            {
               
                Debug.Log("We've been satisfied " + this.GetType().Name);
                    requirementsForPowerUpStartSatisfied = true;
            }
        } 
        if(!autoActivated){

            StartCoroutine(ActivationWindowCountdown());
        }
        StartCoroutine(CountdownOverarchingDuration());
        PowerUpChosen();
    }

    // public void SetChargeUsed(){
    //     poweredUp = false;

    // }

   


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
            if (poweredUp)
            {
                
                Debug.Log("We've been satisfied " + this.GetType().Name);
                    requirementsForPowerUpStartSatisfied = true;
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
            if (poweredUp)
            {
                //and we're powered up, and not already using a powerup, we can use it!
               
                Debug.Log("We've been satisfied " + this.GetType().Name);
                    requirementsForPowerUpStartSatisfied = true;
            }
        }

    }

    public virtual void StartPowerUp()
    {
        NowUsingPowerUpWrapper();
    }

    public virtual void Update()
    {
        if (autoActivated && requirementsForPowerUpStartSatisfied )
        {
            //TODO: We need to make this so it only activates once AND doesn't activate while it's already going
            Debug.Log("Activating  power up! " + this.GetType().Name);
           // SetChargeUsed();
            StartPowerUp();
        }
        else if (!autoActivated && requirementsForPowerUpStartSatisfied )
        {
            Debug.Log("PRESS E TO ACTIVATE POWER UP " + this.GetType().Name);
            if (Input.GetKeyDown(KeyCode.E))
            {
                //SetChargeUsed();
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
