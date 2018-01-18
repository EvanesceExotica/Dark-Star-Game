using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpaceTimeTear : MonoBehaviour, IHookable
{
    bool playerHookedOn;
    public static event Action PlayerHookedOn;
    
    public static event Action PlayerReleased;

    public void SetHooked()
    {
        if(PlayerHookedOn != null)
        {
            PlayerHookedOn();
        }
        playerHookedOn = true;
    }
    
    public void RemoveHooked()
    {
        if(PlayerReleased != null)
        {
            PlayerReleased();
        }
        playerHookedOn = false;
    }
}
