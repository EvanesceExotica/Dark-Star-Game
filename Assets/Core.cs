using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : Switch {

    public override void OnTriggerEnter2D(Collider2D hit)
    {
        base.OnTriggerEnter2D(hit);
        //if(hit.gameObject.name == "Player")
        //{
        //    Debug.Log("The player is in here");
        //}
    }

    public override void OnTriggerExit2D(Collider2D hit)
    {
        base.OnTriggerExit2D(hit);
    }
    //TODO: We're going to make this work as a different type of switch. 
}
