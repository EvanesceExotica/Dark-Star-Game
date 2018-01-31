﻿using System.Collections;
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
    void Depowered()
    {
        ourPoweredUpEffect.enabled = false;
        currentChargeState = ChargeStates.normal;
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
        SoulBehavior.AttachToPlayer += AddsoulToList;
        SoulBehavior.DetatchFromPlayer -= RemovesoulFromList;
        ourPoweredUpEffect = GetComponent<_2dxFX_PlasmaShield>();
        ourPoweredUpEffect.enabled = false;
    }
    public void AddsoulToList(GameObject soulToAdd)
    {
        soulsAttachedToPlayer.Add(soulToAdd);
    }

    public void RemovesoulFromList(GameObject soulToRemove)
    {
        soulsAttachedToPlayer.Remove(soulToRemove);
    }


    void ConsumeSoul()
    {
        soulAmount--;
        WerePoweredUp();
        if (soulsAttachedToPlayer.Count > 0)
        {
            GameObject soulToBeConsumed = soulsAttachedToPlayer[0];
            
           // soulsAttachedToPlayer.Remove(soulToBeConsumed);
          //  soulToBeConsumed.GetComponent<SoulBehavior>().ReturnToPool();
        }
        ourPoweredUpEffect.enabled = true;

        StartCoroutine(TimeOutConsumedSoul());
    }

    public void UseUpSoulPower()
    {
        ourPoweredUpEffect.enabled = false;
        currentChargeState = ChargeStates.normal;
    }

    IEnumerator TimeOutConsumedSoul()
    {
        yield return new WaitForSeconds(15.0f);
        Depowered();
    }
    void OnEnable()
    {

    }

    // Use this for initialization
    void Start()
    {
        currentChargeState = ChargeStates.normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && soulsAttachedToPlayer.Count > 0)
        {

            ConsumeSoul();
        }

    }
}
