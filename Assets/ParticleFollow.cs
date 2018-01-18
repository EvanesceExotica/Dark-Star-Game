using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]


public class ParticleFollow : MonoBehaviour {

    public bool targetMouse;

    GameObject target;
    GameObject player;
    public float speed = 8.0f;
    public float distanceFromCamera = 5.0f;
    
    public Action WhoAreWeFollowing;
    // Use this for initialization

    void SetTargetAsPlayer()
    {
        Vector3 playerPosition = player.transform.position;
        Vector2 position = Vector2.Lerp(transform.position, playerPosition, 1.0f - Mathf.Exp(-speed * Time.deltaTime));
        transform.position = position;

    }

    void SetTargetAsMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromCamera;

        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld, 1.0f - Mathf.Exp(-speed * Time.deltaTime));

        transform.position = position;
    }

    private void Update()
    {
        WhoAreWeFollowing(); 
    }


    void Start () {
        
        player = GameObject.Find("Player");
        if (targetMouse)
        {
            WhoAreWeFollowing = SetTargetAsMouse;
        }
        else if (!targetMouse)
        {
            WhoAreWeFollowing = SetTargetAsPlayer;
        }
		
	}
	

}
