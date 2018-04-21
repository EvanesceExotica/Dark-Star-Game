using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class PowerUpHandler : MonoBehaviour
{

    public PowerUp currentPowerUp;

    [SerializeField] bool powerUpCurrentlyBeingUsed;

    bool onSwitch;

    public Dictionary<ChoosePowerUp.PowerUpTypes, PowerUp> onSwitchPowerUpDictionary = new Dictionary<ChoosePowerUp.PowerUpTypes, PowerUp>();

    public Dictionary<ChoosePowerUp.PowerUpTypes, PowerUp> offSwitchPowerUpDictionary = new Dictionary<ChoosePowerUp.PowerUpTypes, PowerUp>();

    public List<PowerUp> OnSwitchPowerUps = new List<PowerUp>();
    public List<PowerUp> OffSwitchPowerUps = new List<PowerUp>();

    void SetNothingPlaying()
    {
		currentPowerUp = null;
        powerUpCurrentlyBeingUsed = false;
		powerUpCurrentlyChosen = false;
    }

    void Awake()
    {
        //this is when the powerup is specifically chosen
        ChoosePowerUp.SpecificPowerUpChosen += this.SetCurrentPowerUp;
        PowerUp.StoppedUsingPowerUp += this.SetNothingPlaying;
        Switch.SwitchEntered += SetOnSwitch;
        Switch.SwitchExited += SetOffSwitch;
        PopulateDictionaries();
    }


    void SetOnSwitch(GameObject switchWereOn)
    {
        onSwitch = true;
    }

    void SetOffSwitch(GameObject switchWeLeft)
    {
        onSwitch = false;
    }

    void PopulateDictionaries()
    {
        List<PowerUp> powerUpsOnPlayer = GetComponentsInChildren<PowerUp>().ToList();
        foreach (PowerUp pu in powerUpsOnPlayer)
        {
            if (pu.ourRequirement == PowerUp.Requirement.OnlyUseOnSwitch)
            {
                OnSwitchPowerUps.Add(pu);
            }
            else if (pu.ourRequirement == PowerUp.Requirement.OnlyUseOffSwitch)
            {
                OffSwitchPowerUps.Add(pu);
            }
        }
        foreach (PowerUp pu in OnSwitchPowerUps)
        {
            if (pu.GetType() == typeof(BeamHandler))
            {

                onSwitchPowerUpDictionary.Add(ChoosePowerUp.PowerUpTypes.laser, pu);
            }
            else if (pu.GetType() == typeof(DropTrack))
            {
                onSwitchPowerUpDictionary.Add(ChoosePowerUp.PowerUpTypes.connector, pu);
            }
            else if (pu.GetType() == typeof(ChainSwap))
            {
                onSwitchPowerUpDictionary.Add(ChoosePowerUp.PowerUpTypes.chain, pu);

            }
        }
        foreach (PowerUp pu in OffSwitchPowerUps)
        {
            if (pu.GetType() == typeof(Shoot))
            {

                offSwitchPowerUpDictionary.Add(ChoosePowerUp.PowerUpTypes.laser, pu);
            }
            else if (pu.GetType() == typeof(RideConnections))
            {

                offSwitchPowerUpDictionary.Add(ChoosePowerUp.PowerUpTypes.connector, pu);
            }
            else if (pu.GetType() == typeof(StarChain))
            {

                offSwitchPowerUpDictionary.Add(ChoosePowerUp.PowerUpTypes.chain, pu);
            }
        }
    }

    PowerUp AlignChosenPowerUp(ChoosePowerUp.PowerUpTypes powerUpType, bool wereOnSwitch)
    {
        PowerUp chosen = null;


        return chosen;
    }

    public bool powerUpCurrentlyChosen;
    void SetCurrentPowerUp(ChoosePowerUp.PowerUpTypes chosenPowerUpType)
    {
        //TODO Add some element to forbid player from chosing another powerup once this triggers until the current one is done

        if (onSwitch)
        {
            currentPowerUp = onSwitchPowerUpDictionary[chosenPowerUpType];
        }
        else if (!onSwitch)
        {
            currentPowerUp = offSwitchPowerUpDictionary[chosenPowerUpType];
        }
        powerUpCurrentlyChosen = true;
        if (!currentPowerUp.autoActivated)
        {
            currentPowerUp.ActivationWindowCountdownWrapper();

        }
        currentPowerUp.CountdownOverarchingDurationWrapper();

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (powerUpCurrentlyChosen)
        {
            if (currentPowerUp.autoActivated && !powerUpCurrentlyBeingUsed)
            {
                //TODO: We need to make this so it only activates once AND doesn't activate while it's already going
                Debug.Log("Activating  power up! " + currentPowerUp.name.ToString());
                // SetChargeUsed();
                currentPowerUp.StartPowerUp();
            }
            else if (!currentPowerUp.autoActivated && !powerUpCurrentlyBeingUsed)
            {
                Debug.Log("PRESS E TO ACTIVATE POWER UP " + this.GetType().Name);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("<color=blue> E was pressed to activate </color>" + currentPowerUp.name.ToString());
                    //SetChargeUsed();
                    currentPowerUp.StartPowerUp();
                }
            }
        }

    }
}
