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

    Vector2 bashDirection;
    Vector2 enemyFlyDirection;

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
        bashDirection = Vector2.zero;
        enemyFlyDirection = Vector2.zero;
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

    public void SlowEffectsToNormalTime()
    {
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

    void SetBashDirection(Vector2 directionWereFlying){
        bashDirection = directionWereFlying;
        enemyFlyDirection = -bashDirection;
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
            if (hookshotUsed)
            {
                DeactivateEffects();
                break;

            }
            yield return null;
        }
        DeactivateColliderObject();

    }

    void SetHookshotUsed()
    {
        //TODO: Add an action for the hookshot being used, or when it grabs ahold of something at least
        hookshotUsed = true;
    }
    void OnCollisionEnter2D(Collision2D hit)
    {
        //TODO -- while launching, the player's collider is on. It's reset if the player uses the hookshot
        if (hit.collider.GetComponent<SpaceMonster>() != null)
        {
            hitMonster = true;
            //We want to knock the monster away at top speed
            UniversalMovement theirMovement = hit.collider.GetComponent<UniversalMovement>();
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (theirMovement != null)
            {
                playerReferences.rb.velocity = new Vector2(0, 0);
                enemy.CrashedInto();
                //TODO: For some reason, instead of being knocked back, it's being knocked forward
                //TODO: I tseems to be working ok without the knockback though
                if (theirMovement.ourTypesOfIncapacitation.Contains(UniversalMovement.IncapacitationType.Pulled))
                {
                    //theirMovement.KnockBack(hit, 80.0f, enemyFlyDirection);
                }
            }
        }
    }


    void Update()
    {


    }

}
