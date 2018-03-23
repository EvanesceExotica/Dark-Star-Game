using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SoulHandler : MonoBehaviour
{

    public GameObject ourPoweredUpEffect;
    List<ParticleSystem> powerUpParticles;
    _2dxFX_PlasmaShield ourChargedEffect;

    int soulAmount;
    public ChargeStates currentChargeState;
    public List<GameObject> soulsAttachedToPlayer;


    public static event Action Charged;

    public SoulRotateScript rotateScript;

    public GameObject soulChargingUs;
    void SetSoulChargingUsUp(GameObject soul)
    {
        soulChargingUs = soul;
        soulsAttachedToPlayer.Remove(soul);
        WereCharged();

    }
    void WereCharged()
    {
        Debug.Log("We've been powered up");
        currentChargeState = ChargeStates.soulCharged;
        ourChargedEffect.enabled = true;
        if (Charged != null)
        {
            Charged();
        }
    }

    public static event Action ChargeTimedOut;

    public void ChargeTransferredToPowerUp(){
        //TODO: PLay some sort of bursty effect here to demonstrate the charge has been transfered
       ParticleSystemPlayer.PlayChildParticleSystems(powerUpParticles);
        ourChargedEffect.enabled = false;

        currentChargeState = ChargeStates.usingPowerUp;
        rotateScript.soulSuckedIn = false;
        //soulChargingUs.GetComponent<SoulBehavior>().ReturnToPool();
        soulChargingUs = null;
    }
    public void Discharged()
    {
        Debug.Log("We're depowered");

        ParticleSystemPlayer.StopChildParticleSystems(powerUpParticles);
        //soulsAttachedToPlayer.RemoveAt(soulsAttachedToPlayer.Count - 1 );
        ourChargedEffect.enabled = false;
        currentChargeState = ChargeStates.normal;
      //  rotateScript.soulSuckedIn = false;
        //soulPoweringUsUp.GetComponent<SoulBehavior>().ReturnToPool();
    //    soulPoweringUsUp = null;
        if (ChargeTimedOut != null)
        {
            ChargeTimedOut();
        }
    }

    public enum ChargeStates
    {
        soulCharged,

        usingPowerUp,
        normal
    }
    private void Awake()
    {
        if(ourPoweredUpEffect != null){
            powerUpParticles = ourPoweredUpEffect.GetComponentsInChildren<ParticleSystem>().ToList();
        }
        ChoosePowerUp.powerupChosen += this.ChargeTransferredToPowerUp;
        //PowerUp.NowUsingThisPowerUp += this.ChargeTransferredToPowerUp;
        PowerUp.StoppedUsingPowerUp += this.Discharged;
        //TODO: I THINK THE Below functionality works, but make sure
        SoulBehavior.MissedPowerUp += this.Discharged;
        //LaunchSoul.SoulNotLaunching += this.Depowered;
        SoulBehavior.AttachToPlayer += AddsoulToList;
        SoulBehavior.DetachFromPlayer -= RemovesoulFromList;
        ourChargedEffect = GetComponent<_2dxFX_PlasmaShield>();
        ourChargedEffect.enabled = false;
        rotateScript = GetComponentInChildren<SoulRotateScript>();
        SoulRotateScript.SuckedInASoul += this.SetSoulChargingUsUp;
    }
    public void AddsoulToList(GameObject soulToAdd)
    {
        if (!soulsAttachedToPlayer.Contains(soulToAdd))
        {
            soulsAttachedToPlayer.Add(soulToAdd);
            rotateScript.AddNewSoulWrapper(soulToAdd);
        }
    }

    public void RemovesoulFromList(GameObject soulToRemove)
    {
        soulsAttachedToPlayer.Remove(soulToRemove);
    }

    void ConsumeSoul()
    {
        Debug.Log("Ready to consume soul");
        //soulAmount--;
        // WerePoweredUp();
        if (soulsAttachedToPlayer.Count > 0)
        {
            rotateScript.SuckInSoulWrapper();

        }
        //  ourPoweredUpEffect.enabled = true;

        //   StartCoroutine(TimeOutConsumedSoul());
    }

    public void UseUpSoulPower()
    {

        //  soulsAttachedToPlayer.RemoveAt(soulsAttachedToPlayer.Count - 1);
        ourChargedEffect.enabled = false;
        currentChargeState = ChargeStates.normal;
    }

    IEnumerator TimeOutConsumedSoul()
    {
        yield return new WaitForSeconds(15.0f);
        Discharged();
    }

    // Use this for initialization
    void Start()
    {
        currentChargeState = ChargeStates.normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && soulsAttachedToPlayer.Count > 0 && currentChargeState != ChargeStates.soulCharged && currentChargeState != ChargeStates.usingPowerUp)
        {//press R as long as we have souls and we're not already powered up

            ConsumeSoul();
        }

        
    }
}
