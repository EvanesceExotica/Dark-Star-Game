using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PowerUp : MonoBehaviour
{

    public virtual void Awake(){
        //this is a bit wonky but I think it should work?
        NowUsingPowerUp += SetCantUserPowerUp;
    }
    //this maybe should be static so that it cancels out the powerups for all 6 powerups
   public static event Action NowUsingPowerUp;

   public void NowUsingPowerUpWrapper(){
       if(NowUsingPowerUp != null){
           NowUsingPowerUp();
       }
   }
   public static event Action StoppedUsingPowerUp; 

   public void StoppedUsingPowerUpWrapper(){
       if(StoppedUsingPowerUp != null){
           StoppedUsingPowerUp();
       }
   }

   void SetCantUserPowerUp(){
       canStartPowerUp = false;
   }
    public enum Requirement
    {
        OnlyUseOnSwitch,
        OnlyUseOffSwitch
    }

    public Requirement ourRequirement;

    public bool autoActivated;
    public bool canStartPowerUp;

    public bool onSwitch;

    public bool PoweredUp;

    public bool currentlyUsingPowerUp;

    public void SetPoweredUp()
    {
        Debug.Log("We're ready to perform a power up! " + this.GetType().Name);
        PoweredUp = true;
        if (ourRequirement == Requirement.OnlyUseOnSwitch)
        {
            if (onSwitch)
            {
                //if we're also on a switch, we can chain enemy now
                canStartPowerUp = true;
            }
        }
        else if (ourRequirement == Requirement.OnlyUseOffSwitch)
        {
            if (!onSwitch)
            {
                canStartPowerUp = true;
            }
        }
    }

    public void RemovePoweredUp()
    {
        //both powered up and on switch hae to be true to chain enemy, so set canChainEnemy to false
        PoweredUp = false;
        canStartPowerUp = false;
    }


    public virtual void SetOnSwitch(GameObject ourSwitch)
    {
        Debug.Log("ON SWITCH AND READY TO POWER UP");
        //we're on a switch
        onSwitch = true;
        if(ourRequirement == Requirement.OnlyUseOffSwitch){
            canStartPowerUp = false;
        }
        else{
            if(PoweredUp){
                canStartPowerUp = true;
            }
        }

      
    }

    public virtual void SetOffSwitch(GameObject ourSwitch)
    {
        //we're off of a switch
        onSwitch = false;
        if(ourRequirement == Requirement.OnlyUseOnSwitch){
            //if we can only use this powerup off of a switch, we immediately can't use it
            canStartPowerUp = false;
        }
        else{
            //but if we can use this powerup off of a switch
            if(PoweredUp){
                //and we're powered up, we can use it!
                canStartPowerUp = true;
            }
        }
       
    }

    public virtual void StartPowerUp(){
        NowUsingPowerUpWrapper();
    }

    public virtual void Update(){
        if(autoActivated && canStartPowerUp){
            Debug.Log("Starting to power up!");
            StartPowerUp();
        }
        else if(!autoActivated && canStartPowerUp && Input.GetKeyDown(KeyCode.E)){
            StartPowerUp();
        }
        // if(currentlyUsingPowerUp){
        //     //if we're using the powerup right now, we don't want to start using it again
        //     canStartPowerUp = false;
        // }
    }
    // Use this for initialization

}
