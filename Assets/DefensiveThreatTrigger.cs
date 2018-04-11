using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveThreatTrigger : ThreatTrigger
{

    public enum ReactsTo
    {
        player,

        playerHook,
        otherSpaceMonsters
    }

    public List<ReactsTo> thingsToReactTo = new List<ReactsTo>();

    // Use this for initialization
    public override void OnTriggerEnter2D(Collider2D hit)
    {

        if (thingsToReactTo.Contains(ReactsTo.otherSpaceMonsters))
        {

            SpaceMonster theirSpaceMonster = hit.GetComponent<SpaceMonster>();
            if (theirSpaceMonster != null)
            {
                if (theirSpaceMonster.ID != ourEnemy.ourSpaceMonster.ID)
                {
                    potentialThreatsInTrigger.Add(gameObject);
                    TriggerThreatReaction(hit.gameObject);
                }
            }
        }
        if (thingsToReactTo.Contains(ReactsTo.player))
        {
            PlayerReferences playerReferences = hit.GetComponent<PlayerReferences>();
            if (playerReferences != null)
            {
                potentialThreatsInTrigger.Add(gameObject);
                TriggerThreatReaction(hit.gameObject);
            }
        }
        if (thingsToReactTo.Contains(ReactsTo.playerHook))
        {
            Hookshot hookshot = hit.GetComponent<Hookshot>();
            if (hookshot != null)
            {
                potentialThreatsInTrigger.Add(gameObject);
                TriggerThreatReaction(hit.gameObject);
            }
        }

    }


    public override void OnTriggerExit2D(Collider2D hit)
    {

        if (thingsToReactTo.Contains(ReactsTo.otherSpaceMonsters))
        {
            SpaceMonster theirSpaceMonster = hit.GetComponent<SpaceMonster>();
            if (theirSpaceMonster != null)
            {
                if (theirSpaceMonster.ID != ourEnemy.ourSpaceMonster.ID)
                {
                    potentialThreatsInTrigger.Remove(hit.gameObject);
                }
            }
        }
        if (thingsToReactTo.Contains(ReactsTo.player))
        {

            PlayerReferences playerReferences = hit.GetComponent<PlayerReferences>();
            if (playerReferences != null)
            {
                potentialThreatsInTrigger.Remove(gameObject);

            }
        }
        if (thingsToReactTo.Contains(ReactsTo.playerHook))
        {
            Hookshot hookshot = hit.GetComponent<Hookshot>();
            if (hookshot != null)
            {
                potentialThreatsInTrigger.Remove(gameObject);
                TriggerThreatReaction(hit.gameObject);
            }
        }
        if (potentialThreatsInTrigger.Count == 0)
        {
            GiveAllClearSignal();
        }
    }
}
