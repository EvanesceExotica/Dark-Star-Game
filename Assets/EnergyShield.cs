using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnergyShield : MonoBehaviour {

    [SerializeField]
    float energyShieldDuration;

    [SerializeField]
    bool shieldActive;

    [SerializeField]
    bool displayingShield;
     GameObject shieldVisual;
    GameObject child;
    List<ParticleSystem> childParticles;
    int soulAmount = 5;
    PlayerReferences pReference;
    CircleCollider2D ourBarrierCollider;

    public static event Action ActivateShield;
    //TODO:THIS SHOULD PROVIDE A BUMPER AGAINST THE VOID THAT NEEDS TO BE TIMED RIGHT
    public void ShieldActivated()
    {

     //   shieldActive = true;
        StartCoroutine(ActivateEnergyShield());
        if (ActivateShield != null)
        {
            ActivateShield();
        }
    }

    public static event Action DropShield;

    void ShieldActivated2Test()
    {
        StartCoroutine(ActivateEnergyShield());
    }

    void ShieldDropped2Test()
    {
        HideShield();
        DropShield();
    }

   public void ShieldDropped()
    {
        if(DropShield != null)
        {
            shieldActive = false;
    
        }
    }

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        childParticles = child.GetComponentsInChildren<ParticleSystem>().ToList();
        energyShieldDuration = 10.0f;
        shieldVisual = child;
        pReference = GetComponentInParent<PlayerReferences>();
        ourBarrierCollider = GetComponentInChildren<CircleCollider2D>();
        ourBarrierCollider.enabled = false;
    }


    // Use this for initialization
    void Start () {
      
	}

    void DisplayShield()
    {
        displayingShield = true;
        if (!shieldVisual.activeSelf)
        {
            shieldVisual.SetActive(true);

        }
        else
        {
            foreach(ParticleSystem ps in childParticles)
            {
                ps.Play();
            }
        }
    }

    void HideShield()
    {

        foreach(ParticleSystem ps in childParticles)
        {
            ps.Stop(); 
        }
      //  transform.GetChild(0).gameObject.SetActive(false);
        displayingShield = false;
    }

    public IEnumerator ActivateEnergyShield()
    {
        ourBarrierCollider.enabled = true;
        shieldActive = true;
        float startTime = Time.time;

        while (Time.time < startTime + energyShieldDuration)
        {
            if (!displayingShield)
            {
                DisplayShield();
            }
            yield return null;
        }
        shieldActive = false;
        ourBarrierCollider.enabled = false;
        HideShield();
    }
    
    IEnumerator StartShieldCooldown()
    {
        yield return new WaitForSeconds(10.0f);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.H) && pReference.playerSoulHandler.currentChargeState == SoulHandler.ChargeStates.soulCharged){
            Debug.Log("Shield activated");
            if (!shieldActive)
            {
                ShieldActivated();
            }
        }

    
	}
}
