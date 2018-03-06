using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerSoulHandler : MonoBehaviour
{


    _2dxFX_PlasmaShield ourPoweredUpEffect;
    int soulAmount;
    public ChargeStates currentChargeState;
    public List<GameObject> soulsAttachedToPlayer;

    public static event Action PoweredUp;

    public SoulRotateScript rotateScript;

    public GameObject soulPoweringUsUp;
    void SetSoulPoweringUsUp(GameObject soul)
    {
        soulPoweringUsUp = soul;
        soulsAttachedToPlayer.Remove(soul);
        WerePoweredUp();

    }
    void WerePoweredUp()
    {
        Debug.Log("We've been powered up");
        currentChargeState = ChargeStates.soulCharged;
        ourPoweredUpEffect.enabled = true;
        if (PoweredUp != null)
        {
            PoweredUp();
        }
    }

    public static event Action PowerUpTimedOut;
    public void Depowered()
    {
        Debug.Log("We're depowered");
        //soulsAttachedToPlayer.RemoveAt(soulsAttachedToPlayer.Count - 1 );
        ourPoweredUpEffect.enabled = false;
        currentChargeState = ChargeStates.normal;
        rotateScript.soulSuckedIn = false;
        //soulPoweringUsUp.GetComponent<SoulBehavior>().ReturnToPool();
        soulPoweringUsUp = null;
        if (PowerUpTimedOut != null)
        {
            PowerUpTimedOut();
        }
    }

    public enum ChargeStates
    {
        soulCharged,
        normal
    }
    private void Awake()
    {
        PowerUp.StoppedUsingPowerUp += this.Depowered;
        //TODO: I THINK THE Below functionality works, but make sure
        SoulBehavior.MissedPowerUp += this.Depowered;
        //LaunchSoul.SoulNotLaunching += this.Depowered;
        SoulBehavior.AttachToPlayer += AddsoulToList;
        SoulBehavior.DetachFromPlayer -= RemovesoulFromList;
        ourPoweredUpEffect = GetComponent<_2dxFX_PlasmaShield>();
        ourPoweredUpEffect.enabled = false;
        rotateScript = GetComponentInChildren<SoulRotateScript>();
        SoulRotateScript.SuckedInASoul += this.SetSoulPoweringUsUp;
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
        ourPoweredUpEffect.enabled = false;
        currentChargeState = ChargeStates.normal;
    }

    IEnumerator TimeOutConsumedSoul()
    {
        yield return new WaitForSeconds(15.0f);
        Depowered();
    }

    // Use this for initialization
    void Start()
    {
        currentChargeState = ChargeStates.normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && soulsAttachedToPlayer.Count > 0 && currentChargeState != ChargeStates.soulCharged)
        {//press R as long as we have souls and we're not already powered up

            ConsumeSoul();
        }

        if (rotateScript.soulSuckedIn == true)
        {

        }

    }
}
