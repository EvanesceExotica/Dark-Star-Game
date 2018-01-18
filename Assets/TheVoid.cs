using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheVoid : ParentTrigger {

    Collider2D innerZone;
    Collider2D outerZone;

    public bool inInnerZone;
    public bool inOuterZone;

    public bool inVoid;

    bool voidEntered;
    bool voidExited;

    private void Awake()
    {
        voidBoundaryWarning = transform.GetChild(1).gameObject;
        voidBoundaryWarningSpriteRenderer = voidBoundaryWarning.GetComponent<SpriteRenderer>();
    }
    GameObject voidBoundaryWarning;
    SpriteRenderer voidBoundaryWarningSpriteRenderer;
    public static event Action PlayerEnteredVoid;

    public static event Action PlayerExitedVoid; 

    void EnteredVoid()
    {
        Debug.Log("Player entered void");
        if(PlayerEnteredVoid != null)
        {
            PlayerEnteredVoid();
        }
    }

    void ExitedVoid()
    {
        Debug.Log("Player exited void");
        if(PlayerExitedVoid != null)
        {
            PlayerExitedVoid(); 
        }
    }



    public override void OnChildTriggerEnter2D(Collider2D hit, GameObject hitChild)
    {
        inInnerZone = true;
    }

    public override void OnChildTriggerExit2D(Collider2D hit, GameObject hitChild)
    {
        inInnerZone = false;

    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        inOuterZone = true;
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        inOuterZone = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (inOuterZone && !inInnerZone)
        {
            //The player is not inside the inner safe circle, but is of course still in size the outer circle, placing them in the void
            inVoid = true;
        }
        else if(inInnerZone && inOuterZone)
        {
            //The player is inside the inner safe circle AND the outer circle, placing them in the safe zone
            inVoid = false; 
        }

        if(inVoid && !voidEntered)
        {
            //if the player is in the void but we haven't considered it yet, make it true so it's only considered once
            voidEntered = true;
            //make it false that they've exited the void at any point so this can be triggered later
            voidExited = false;
            EnteredVoid();
            

        }

        if(!inVoid && !voidExited)
        {
            voidExited = true;
            voidEntered = false;
            ExitedVoid();
        }
		
	}
}
