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
        soulsAttachedToPlayer.RemoveAt(0);
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
        rotateScript = GetComponentInChildren<SoulRotateScript>();
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
            soulToBeConsumed.transform.position = gameObject.transform.position;
           // soulsAttachedToPlayer.Remove(soulToBeConsumed);
          //  soulToBeConsumed.GetComponent<SoulBehavior>().ReturnToPool();
        }
        ourPoweredUpEffect.enabled = true;

        StartCoroutine(TimeOutConsumedSoul());
    }

    public void UseUpSoulPower()
    {

        soulsAttachedToPlayer.RemoveAt(0);
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

    }
}
