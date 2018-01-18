using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : Health {

    PlayerReferences pReference;

    public GameObject deathParticlePrefab;

    int deathStarLightpenalty;
    public  event Action<float> PlayerDied;

    public int DeathStarLightPenalty
    {
        get
        {
            return deathStarLightpenalty;
        }
        set
        {
            deathStarLightpenalty = value; 
        }
    }

	// Use this for initialization
	void Start () {
        deathStarLightpenalty = -2;
        pReference = GetComponent<PlayerReferences>();
		
	}

    public override void Die()
    {
        pReference.rb.velocity = new Vector3(0, 0, 0);
            
       
        if(PlayerDied != null){

            PlayerDied(deathStarLightpenalty); 
        }
        Debug.Log("I died!");
        InstantiateDeathParticles();
        StartCoroutine(Respawn());
    }

    void InstantiateDeathParticles()
    {
    
        GameObject blast = Instantiate(deathParticlePrefab) as GameObject;
        blast.transform.position = gameObject.transform.position;
        pReference.ourSpriteRenderer.enabled = false;
        
    }
	
    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2.0f);
        transform.position = pReference.darkStar.transform.position;
        pReference.ourSpriteRenderer.enabled = true;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
