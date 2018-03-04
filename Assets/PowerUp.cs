using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public enum Requirement
    {
        OnlyUseOnSwitch,
        OnlyUseOffSwitch
    }

    public Requirement ourRequirement;

    bool canUsePowerUp;

    bool onSwitch;

    bool PoweredUp;

    public void SetPoweredUp()
    {
        PoweredUp = true;
        if (ourRequirement == Requirement.OnlyUseOnSwitch)
        {
            if (onSwitch)
            {
                //if we're also on a switch, we can chain enemy now
                canUsePowerUp = true;
            }
        }
        else if (ourRequirement == Requirement.OnlyUseOffSwitch)
        {
            if (!onSwitch)
            {
                canUsePowerUp = true;
            }
        }
    }

    public void RemovePoweredUp()
    {
        //both powered up and on switch hae to be true to chain enemy, so set canChainEnemy to false
        PoweredUp = false;
        canUsePowerUp = false;
    }


    public void SetOnSwitch(GameObject ourSwitch)
    {
        onSwitch = true;
        if (PoweredUp)
        {
            if (ourRequirement == Requirement.OnlyUseOnSwitch)
            {
                //if we can only use this power up on a switch
                if (PoweredUp)
                {
                    //if we're also  poweredUp, we can chain enemy now
                    canUsePowerUp = true;
                }
            }
            else if (ourRequirement == Requirement.OnlyUseOffSwitch)
            {
                //if we can only use this powoerup off of a switch
                if (!onSwitch)
                {
                    //if we're also off of a switch, we can chain enemy now
                    canUsePowerUp = false;
                }
            }
        }
    }

    public void SetOffSwitch(GameObject ourSwitch)
    {
        onSwitch = false;
        if (ourRequirement == Requirement.OnlyUseOnSwitch)
        {
            //if we can only use this powerup on a switch
            canUsePowerUp = false;
        }
        else if (ourRequirement == Requirement.OnlyUseOffSwitch)
        {
            //if we can only use this powoerup off of a switch
            //if we're also off of a switch, we can chain enemy now
            if (PoweredUp)
                canUsePowerUp = true;

        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
