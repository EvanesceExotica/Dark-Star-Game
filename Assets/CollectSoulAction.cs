using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollectSoulAction : GoapAction
{

    /* TODO: TEST IF STUNS AND PULLS TRIGGER STUN STATE, MAKE SURE THE SOULS CAN BE COLLECTED -- maybe change strength value of souls being pulled in slightly*/
    bool hasEatenSoul;
    bool tryingToSuckInSoul;
    public GameObject eatingParticleSystemGO;
    List<ParticleSystem> eatingParticleSystemList = new List<ParticleSystem>();
    //TODO: MAke the above list inheret to the EVENT HORIZON and not individual actions
    public List<GameObject> floatingSouls = new List<GameObject>();
    SoulHandler playerSoulHandler;
    public CollectSoulAction()
    {

        AddEffect(new Condition("eat", true));
        cost = 200f;
    }

    public override void ImportantEventTriggered(GameObject intruder){

    }

    public override void Awake()
    {
        base.Awake();
        playerSoulHandler = GameStateHandler.player.GetComponent<PlayerReferences>().playerSoulHandler;
        eatingParticleSystemList = eatingParticleSystemGO.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    public void OnEnable()
    {
        floatingSouls.AddRange(playerSoulHandler.soulsFloatingInAether);
        SoulBehavior.SoulSpawned += AddSoulToList;
    }

    public void OnDisable()
    {
        SoulBehavior.SoulSpawned -= AddSoulToList;
    }

    void AddSoulToList(GameObject soul)
    {
        floatingSouls.Add(soul);
    }

    void RemoveSoulFromList(GameObject soul)
    {
        floatingSouls.Remove(soul);
    }


    public override void reset()
    {
        target = null;
        hasEatenSoul = false;

    }

    public override bool isDone()
    {
        return hasEatenSoul;
    }

    public override bool requiresInRange()
    {
        return true; //yes we need to be near food
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {

        if (floatingSouls.Count > 0)
        {
            target = FindClosest.FindClosestObject(floatingSouls, this.gameObject);
            return true;
        }
        else
        {
            return false;
        }

    }

    public override bool perform(GameObject agent)
    {
        if (!performing)
        {
            performing = true;
            StartCoroutine(Devour());
        }
            //TODO: MAke it so a new enemy spawning interrupts IF it's chasing the player
        performing = base.perform(agent);
        return performing;
    }
    public IEnumerator Devour()
    {

        ourPointEffector2D.enabled = true;
        float startTime = Time.time;
        ParticleSystemPlayer.PlayChildParticleSystems(eatingParticleSystemList);
        tryingToSuckInSoul = true;
        float duration = 3.0f;


        yield return new WaitForSeconds(duration);

        tryingToSuckInSoul = false;
        ParticleSystemPlayer.StopChildParticleSystems(eatingParticleSystemList);
        ourPointEffector2D.enabled = false;
    }
    void PlayDevourParticleEffect()
    {

    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        if (tryingToSuckInSoul)
        {
            SoulBehavior soulBehavior = hit.collider.GetComponent<SoulBehavior>();
            if (floatingSouls.Contains(hit.gameObject) && soulBehavior != null)
            {
                PlayDevourParticleEffect();
                floatingSouls.Remove(hit.gameObject);
                soulBehavior.ReturnToPool();
                hasEatenSoul = true;
            }
        }
    }
    // Use this for initialization

}
