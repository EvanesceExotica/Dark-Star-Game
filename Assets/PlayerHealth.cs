using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : Health {

    PlayerReferences pReference;

    public GameObject deathParticlePrefab;

    int deathStarLightpenalty;
    //TODO: Dying should also take away souls? The "souls" act as lives? 
    public  static event Action<float> PlayerDied;

    void RemoveSoul(int number){
        pReference.playerSoulHandler.RemovesoulFromList(pReference.playerSoulHandler.soulsAttachedToPlayer[0]);

    }

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

    public override void BeingDevoured(){
        RemoveSoul(1);
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
