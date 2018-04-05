using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveThreatTrigger : ThreatTrigger
{

    // Use this for initialization
    public override void OnTriggerEnter2D(Collider2D hit)
    {
        SpaceMonster theirSpaceMonster = hit.GetComponent<SpaceMonster>();
        PlayerReferences playerReferences = hit.GetComponent<PlayerReferences>();
        Hookshot hookshot = hit.GetComponent<Hookshot>();
        if (theirSpaceMonster != null)
        {
            if (theirSpaceMonster.ID != ourEnemy.ourSpaceMonster.ID)
            {
                potentialThreatsInTrigger.Add(gameObject);
                TriggerThreatReaction(hit.gameObject);
            }
        }
        else if(playerReferences != null || hookshot != null){
            potentialThreatsInTrigger.Add(gameObject);
            TriggerThreatReaction(hit.gameObject);
        }

    }

    public override void OnTriggerExit2D(Collider2D hit)
    {

        SpaceMonster theirSpaceMonster = hit.GetComponent<SpaceMonster>();
        if (theirSpaceMonster != null)
        {
            if (theirSpaceMonster.ID != ourEnemy.ourSpaceMonster.ID)
            {
                potentialThreatsInTrigger.Remove(hit.gameObject);
                if (potentialThreatsInTrigger.Count == 0)
                {
                    GiveAllClearSignal();
                }
            }
        }
    }
}
