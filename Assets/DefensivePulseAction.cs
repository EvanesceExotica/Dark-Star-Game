using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DefensivePulseAction : GoapAction {

    bool playerKnockedFromRange;
    bool isPulsing;
    float pulseChargeTime = 2.0f;
    float pulseDuration = 1.0f;
    public GameObject miniPulsePrefab;
    public GameObject miniPulseInstantiated;

    public GameObject defensivePulseChargeParticleEffect;
    GameObject chargeParticleSystemGO;
    List<ParticleSystem> chargeParticleSystems = new List<ParticleSystem>();

    public GameObject defensivePulseBlastParticleEffect;
    GameObject blastParticleSystemGO;
    List<ParticleSystem> blastParticleSystems = new List<ParticleSystem>();


    public DefensivePulseAction()
    {
        AddEffect(new Condition("charge", false));
        AddEffect(new Condition("stayAlive", true));


        AddPrecondition(new Condition("charge", true));


        cost = 300f;
    }


    public override void importantEventTriggered(GameObject intruder)
    {
        target = intruder; 
    }

    IEnumerator MiniPulse()
    {
        foreach(ParticleSystem particleSystem in chargeParticleSystems)
        {
            particleSystem.Play(); 
        }
        yield return new WaitForSeconds(pulseChargeTime);
        foreach(ParticleSystem particleSystem in chargeParticleSystems)
        {
            particleSystem.Stop(); 
        }
        miniPulseInstantiated.SetActive(true);
        foreach(ParticleSystem particleSystem in blastParticleSystems)
        {
            particleSystem.Play();

        }
        yield return new WaitForSeconds(pulseDuration);
        miniPulseInstantiated.SetActive(false);
    }

    public override void reset()
    {

    }

    public override bool isDone()
    {
        return playerKnockedFromRange; 

    }

    public override bool requiresInRange()
    {
        return false;
    }



    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //TODO: Fix this so that gamestateHandler has a player ref;
        //target = gameStateHandler.player;
        
        return true;
    }

    public override bool perform(GameObject agent)
    {
        performing = true;
        SpaceMonster currentSpaceMonster = agent.GetComponent<SpaceMonster>();
        if (!isPulsing)
        {
          

            StartCoroutine(MiniPulse());


            return true;
        }
        if (interrupted)
        {
            Debug.Log("We're being interrupted!" + " " + this.name);
            performing = false;
        }

        return performing;

    }

    // Use this for initialization
    void Start () {

        miniPulseInstantiated = InstantiateGameobjectAndSetInactive(miniPulsePrefab);    
        miniPulseInstantiated.GetComponent<CircleCollider2D>().radius = gameObject.GetComponent<CircleCollider2D>().radius * 8;

    

        blastParticleSystemGO = Instantiate(defensivePulseBlastParticleEffect, gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
        blastParticleSystems = blastParticleSystemGO.GetComponentsInChildren<ParticleSystem>().ToList();
        if(blastParticleSystems.Count == 0)
        {
            Debug.Log("THIS IS WRONG!!! NOTHING PARTICLES");
        }

        chargeParticleSystemGO = Instantiate(defensivePulseChargeParticleEffect, gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
        chargeParticleSystems = chargeParticleSystemGO.GetComponentsInChildren<ParticleSystem>().ToList();

        if(chargeParticleSystems.Count == 0)
        {
            Debug.Log("No charge particles either");
        }

    }
	
    GameObject InstantiateGameobjectAndSetInactive(GameObject goToInstantiate)
    {
        GameObject instantiatedGO = Instantiate(goToInstantiate, gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
        if (instantiatedGO.activeSelf)
        {
            instantiatedGO.SetActive(false);
        }
        return instantiatedGO;
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(MiniPulse());
        }
		
	}
}
