using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveThreatTrigger : ThreatTrigger {

    public bool devouredEnemy;
	
	public bool applyingDamageThroughTrigger;
    public bool applyingIncapacitationThroughTrigger;
	
	// void SubscribeToTargetEvents(GameObject target){
    //     target.GetComponent<Health>().Died += this.CheckIfWeKilledEnemy;
    // }

    // void UnsubscribeFromTargetEvents(GameObject target){
    //     target.GetComponent<Health>().Died -= this.CheckIfWeKilledEnemy;
        
    // }
	 void CheckIfWeKilledEnemy(GameObject source, SpaceMonster deadMonster)
    {
        if (source == this.gameObject)
        {
            devouredEnemy = true;
        }
    }
	 void CheckWhatsIncapacitatingEnemy(GameObject source){
        if(source != this.gameObject){

        }
    }

	public override void OnTriggerEnter2D(Collider2D hit){
		
        if (hit.GetComponent<SpaceMonster>() != null)
        {
            if (applyingDamageThroughTrigger)
            {
                //if we're currently applying damage somehow, add this damage source once it enters
                Health health = hit.GetComponent<Health>();
                if (!health.persistentDamageSources.Contains(transform.parent.gameObject))
                {
                    health.AddDamageSource(this.gameObject);
                    health.Died += this.CheckIfWeKilledEnemy;
                }

            }
            if (applyingIncapacitationThroughTrigger)
            {

                //if we're currently applying incapacitation somehow, add this source once it leaves
                UniversalMovement theirMovement = hit.GetComponent<UniversalMovement>();
                if (!theirMovement.incapacitationSources.Contains(transform.parent.gameObject))
                {
                    theirMovement.AddIncapacitationSource(this.gameObject);
                }

            }

            enemiesInThreatTrigger.Add(hit.gameObject);
        }
		 if (hit.GetComponent<PlayerReferences>() != null)
        {
            //TODO: Fix this even further -- the comet for example needs to know if the player is in the trigger and that's all that it worries about
            DetectedPlayerInTrigger();
            if (applyingIncapacitationThroughTrigger)
            {
                hit.GetComponent<PlayerMovement>().AddIncapacitationSource(this.gameObject);
            }
        }
	}

	public override void OnTriggerExit2D(Collider2D hit){
		 if (hit.GetComponent<SpaceMonster>() != null)
        {
             if (applyingDamageThroughTrigger)
            {
                //if we're currently applying damage somehow, add this damage source once it enters
                Health health = hit.GetComponent<Health>();
                if (health.persistentDamageSources.Contains(transform.parent.gameObject))
                {
                    health.RemovePersistentDamageSource(this.gameObject);
                    health.Died -= this.CheckIfWeKilledEnemy;
                }

            }
            if (applyingIncapacitationThroughTrigger)
            {
                UniversalMovement theirMovement = hit.GetComponent<UniversalMovement>();
                if (theirMovement.incapacitationSources.Contains(transform.parent.gameObject))
                {
                    theirMovement.RemoveIncapacitationSource(this.gameObject);
                }
            }
            enemiesInThreatTrigger.Remove(hit.gameObject);
        }

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
