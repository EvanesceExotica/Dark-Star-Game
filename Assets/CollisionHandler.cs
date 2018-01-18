using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {


    public GameObject hookedObject;
    PlayerReferences pReference;

    void DetermineHookedObject(GameObject hookedObj)
    {
        hookedObject = hookedObj;
    }
	// Use this for initialization
	void Start () {
        pReference = GetComponentInParent<PlayerReferences>(); 
	}


    private void OnEnable()
    {
        Hookshot.ObjectHooked += this.DetermineHookedObject;
    }

    private void OnDisable()
    {
        Hookshot.ObjectHooked -= this.DetermineHookedObject;
    }

    // Update is called once per frame
    void Update () {
		
	}

    void ZeroOutVelocity(Rigidbody2D rb)
    {
        if (rb != null)
        {
          //TODO:  rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if(hit.gameObject == hookedObject)
        {
            ////Debug.Log("HookedObjectHitUs");
            //ZeroOutVelocity(hookedObject.GetComponent<Rigidbody2D>());
            //ZeroOutVelocity(pReference.rb);
        }
    }
}


