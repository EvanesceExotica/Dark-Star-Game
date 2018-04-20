using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MirzaBeig.ParticleSystems;
public class ActivateBashShield : MonoBehaviour
{

    GameObject chargeParticleEffect;
    ParticleSystems chargeSystems;

    ParticleSystems shieldSystems;

    ParticleSystems burstSystems;
    public GameObject colliderObject;

	public Collider2D collider;

    bool colliderActive;
    bool hitMonster;
    float startTime;
    float duration;

    bool chargedUp;
	bool hookshotUsed;
    PlayerReferences playerReferences;

    // Use this for initialization
    void Awake()
    {
        shieldSystems = transform.Find("LightningShield").GetComponent<ParticleSystems>();
        chargeSystems = transform.Find("LightningCharge").GetComponent<ParticleSystems>();
       // colliderObject = transform.Find("EnemyBasher").gameObject;
	   collider = GetComponent<CircleCollider2D>();
	   collider.enabled = false;
        playerReferences = transform.parent.GetComponent<PlayerReferences>();
        SpacetimeSlingshot.PrimingToBashEnemy += ActivateEffectsWrapper;
        SpacetimeSlingshot.NoLongerPrimingToBashEnemy += this.DeactivateEffects;
        SpacetimeSlingshot.Launching += this.ActivateColliderWrapper;
        duration = 10.0f;
    }
    public void ActivateEffectsWrapper()
    {
        StartCoroutine(ActivateEffects());
    }
    public IEnumerator ActivateEffects()
    {
        startTime = Time.time;
        chargeSystems.play();
        chargeSystems.setPlaybackSpeed(4);
        yield return new WaitForSeconds(1.0f * SpacetimeSlingshot.slowdownTime);
        chargeSystems.stop();
        shieldSystems.play();
        shieldSystems.setPlaybackSpeed(4);

    }

    public void SlowEffectsToNormalTime(){
        chargeSystems.setPlaybackSpeed(1);
        shieldSystems.setPlaybackSpeed(1);
    }

    public void DeactivateEffects()
    {
        chargeSystems.stop();
        shieldSystems.stop();
    }

    void HitReaction()
    {
		//burstSystems.play();

    }


    void ActivateColliderWrapper()
    {
        SlowEffectsToNormalTime();
        startTime = Time.time;
		collider.enabled = true;
        //colliderObject.SetActive(true);
		StartCoroutine(ActivateCollider());

    }

    void DeactivateColliderObject()
    {
		collider.enabled = false;
        //colliderObject.SetActive(false);
    }
    public IEnumerator ActivateCollider()
    {
        while (Time.time < startTime + duration)
        {
            if (hitMonster)
            {
				HitReaction();
				DeactivateEffects();
                break;
            }
			if(hookshotUsed){
				DeactivateEffects();
				 break;

			}
            yield return null;
        }
		DeactivateColliderObject();

    }

	void SetHookshotUsed(){
		//TODO: Add an action for the hookshot being used, or when it grabs ahold of something at least
		hookshotUsed = true;
	}
    void OnCollisionEnter2D(Collision2D hit)
    {
        //TODO -- while launching, the player's collider is on. It's reset if the player uses the hookshot
        if (hit.collider.GetComponent<SpaceMonster>() != null)
        {
            Debug.Log("We hit something!!!111!!");
            hitMonster = true;
            //We want to knock the monster away at top speed
            UniversalMovement theirMovement = hit.collider.GetComponent<UniversalMovement>();
            if (theirMovement != null)
            {
                playerReferences.rb.velocity = new Vector2(0, 0);
                //TODO: For some reason, instead of being knocked back, it's being knocked forward
                //TODO: I tseems to be working ok without the knockback though
                //theirMovement.KnockBack(hit, 80.0f);
            }
        }
    }


    void Update()
    {
        

    }

}
