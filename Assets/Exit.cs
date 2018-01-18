using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Exit : MonoBehaviour {

    public bool open;
    SpriteRenderer spriteRenderer;
    Collider2D doorEnterTrigger;
 //   public Sprite openSprite;
    _2dxFX_BlackHole blackHoleEffect;

    public static event Action DoorEntered;


    void ChangeDoorState()
    {
        doorEnterTrigger.enabled = true;
        open = true;
    }
  

    void EnterDoor()
    {
        if (DoorEntered != null)
        {
            DoorEntered();
        }
    }

    // Use this for initialization
    void Start() {
        open = false;

    }

    private void Awake()
    {
      //  GameStateHandler.DoorOpened += this.ChangeDoorState;
        Key.KeyGrabbedByPlayer += this.ChangeDoorState;

     //   spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        doorEnterTrigger = gameObject.GetComponent<CircleCollider2D>();
        doorEnterTrigger.enabled = false;
       // blackHoleEffect.enabled = false;

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (!open)
        {
            return;
        }
        if (hit.gameObject.tag == "Player")
            {
                EnterDoor();
            Debug.Log("Entering door!");
            }
        
    }

}
