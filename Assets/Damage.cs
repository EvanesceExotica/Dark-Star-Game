using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour {


    public int damage;

    enum Faction
    {
        Player,
        Enemy
    }

	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D hit)
    {
        IDamageable ourDamagableObject = hit.GetComponent(typeof(IDamageable)) as IDamageable;
        if (ourDamagableObject != null)
        {
            ourDamagableObject.adjustCurrentHealth(damage, this.gameObject);
        }
    }
}
