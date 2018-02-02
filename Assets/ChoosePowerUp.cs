using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
public class ChoosePowerUp : MonoBehaviour
{

    bool acceptingCollision;
    Collider2D ourCollider;
	PowerupHandler powerupHandler;

    GameObject soulBeingLaunched;
	public PowerUpTypes ourPowerUpType;
    public enum PowerUpTypes
    {
        laser,
        connector,
        chain
    }



	public static event Action powerupChosen;

	void ChoseAnyPowerUp(){

		if(powerupChosen != null){
			powerupChosen();
		}
	}

    public static event Action chainChosen;

    void ChoseChain()
    {
        if (chainChosen != null)
        {
            chainChosen();
        }
    }
    public static event Action connectorChosen;

    void ChoseConnector()
    {

        if (connectorChosen != null)
        {
            connectorChosen();
        }
    }

    public static event Action laserChosen;


    void ChoseLaser()
    {

        if (laserChosen != null)
        {
            laserChosen();
        }
    }

    [SerializeField] ParticleSystem shatterParticles;
    void Awake()
    {
        ourCollider = GetComponent<Collider2D>();
        ourCollider.enabled = false;
        LaunchSoul.SoulToBeLaunched += this.AcceptingCollision;
        LaunchSoul.SoulNotLaunching += this.NotAcceptingCollision;
        
        shatterParticles = GetComponentInChildren<ParticleSystem>();
		powerupHandler = GetComponentInParent<PowerupHandler>();
    }

    void AcceptingCollision(GameObject launchedSoul)
    {
        soulBeingLaunched = launchedSoul;
        acceptingCollision = true;
        ourCollider.enabled = true;

    }

    void NotAcceptingCollision()
    {
        acceptingCollision = false;
        ourCollider.enabled = false;
    }
    public string powerUpType;

    void OnTriggerEnter2D(Collider2D hit)
    {

        SoulBehavior soulBehavior = hit.GetComponent<SoulBehavior>();
        if (hit.gameObject == soulBeingLaunched && soulBehavior != null)
        {
			powerupHandler.HideIcons();
		   ChoseAnyPowerUp();
            shatterParticles.Play();
			DeterminePowerup();
          soulBehavior.ReturnToPool();
        }
    }

	void DeterminePowerup(){
		if(ourPowerUpType == PowerUpTypes.connector){
			Debug.Log("connector was chosen");
			ChoseConnector();
		}
		else if(ourPowerUpType == PowerUpTypes.laser){
            
			Debug.Log("laser was chosen");
			ChoseLaser();
		}
		else if(ourPowerUpType == PowerUpTypes.chain){
			Debug.Log("chain was chosen");
			ChoseChain();
		}
	}

}
