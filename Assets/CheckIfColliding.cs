using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfColliding : MonoBehaviour{

public Collider2D ourCollider;
    void Start(){

    }


    void Awake(){

        ourCollider = gameObject.GetComponent<CapsuleCollider2D>();
    }
    void Update(){
    }

void OnCollisionEnter2D(Collision2D hit){
    if(hit.collider){
        Debug.Log("Player is colliding with " + hit.collider.name);
    }
}


}