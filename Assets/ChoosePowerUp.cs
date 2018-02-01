using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ChoosePowerUp : MonoBehaviour
{

    bool acceptingCollision;
    Collider2D ourCollider;
	PowerupHandler powerupHandler;

	public PowerUpTypes ourPowerUpType;
    public enum PowerUpTypes
    {
        laser,
        connector,
        chain
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

    void AcceptingCollision()
    {
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
        if (soulBehavior != null)
        {
			powerupHandler.HideIcons();
           // Debug.Log(powerUpType + " was chosen!");
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
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
