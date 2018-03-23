using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSoulAction : GoapAction
{

    bool hasEatenSoul;

    List<GameObject> soulsFloatingInTheVoid = new List<GameObject>();
    public CollectSoulAction()
    {

        AddEffect(new Condition("eat", true));
        cost = 100f;
    }

    public void OnEnable()
    {
        SoulBehavior.SoulSpawned += AddSoulToList;
    }

    public void OnDisable()
    {
        SoulBehavior.SoulSpawned -= AddSoulToList;
    }

    void AddSoulToList(GameObject soul)
    {
        soulsFloatingInTheVoid.Add(soul);
    }

    void RemoveSoulFromList(GameObject soul)
    {
        soulsFloatingInTheVoid.Remove(soul);
    }


    public override void reset()
    {
        doReset();
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

        if (soulsFloatingInTheVoid.Count > 0)
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
        return performing;
    }

    void PlayDevourParticleEffect()
    {

    }

    void OnCollisionEnter2D(Collider2D hit)
    {
        SoulBehavior soulBehavior = hit.GetComponent<SoulBehavior>();
        if (soulsFloatingInTheVoid.Contains(hit.gameObject) && soulBehavior != null)
        {
            PlayDevourParticleEffect();
            soulBehavior.ReturnToPool();
        }
    }
    // Use this for initialization

}
