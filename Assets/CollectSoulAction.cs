using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSoulAction : GoapAction
{

    bool hasEatenSoul;

   public List<GameObject> floatingSouls = new List<GameObject>();
   SoulHandler playerSoulHandler;
    public CollectSoulAction()
    {

        AddEffect(new Condition("eat", true));
        cost = 200f;
    }

    public override void Awake(){
        base.Awake();
        playerSoulHandler = GameStateHandler.player.GetComponent<PlayerReferences>().playerSoulHandler;
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
            return true;
        }
        else
        {
            return false;
        }

    }

    public override bool perform(GameObject agent)
    {
        performing = true;

        if (interrupted)
        {
            //TODO: MAke it so a new enemy spawning interrupts IF it's chasing the player
            performing = false;
        }
        base.perform(agent);
        return performing;
    }

    void PlayDevourParticleEffect()
    {

    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        SoulBehavior soulBehavior = hit.collider.GetComponent<SoulBehavior>();
        if (floatingSouls.Contains(hit.gameObject) && soulBehavior != null)
        {
            PlayDevourParticleEffect();
            soulBehavior.ReturnToPool();
        }
    }
    // Use this for initialization

}
