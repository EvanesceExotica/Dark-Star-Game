using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselBehavior : MonoBehaviour {


    public int damage;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D hit)
    {
        IDamageable ourDamagableObject = hit.GetComponent(typeof(IDamageable)) as IDamageable;
       if(ourDamagableObject != null)
        {
            ourDamagableObject.adjustCurrentHealth(damage, this.gameObject);
        }
    }
}
