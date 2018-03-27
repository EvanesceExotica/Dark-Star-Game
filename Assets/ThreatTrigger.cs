using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThreatTrigger : MonoBehaviour
{

    public enum ReactionType
    {
        aggressiveToAll,
        defensiveToAll,
        aggressiveToPlayer,

        stationary
    }

    public ReactionType ourReactionType;
    public bool applyingDamageThroughTrigger;
    public bool applyingIncapacitationThroughTrigger;
    public event Action PlayerInMyTrigger;
    public void DetectedPlayerInTrigger()
    {
        if (PlayerInMyTrigger != null)
        {
            PlayerInMyTrigger();
        }
    }
    GameObject currentThreat;

    public bool devouredEnemy;
    bool reachedTargetLocation;
    bool isDashing;

    public float safeDistance = 10.0f;
    [SerializeField] List<GameObject> potentialTreatsInTrigger = new List<GameObject>();
    GameStateHandler ourGameStateHandler;



    public List<GameObject> enemiesInThreatTrigger = new List<GameObject>();
    void Awake()
    {
        DarkStar.DarkStarIsGrowing += this.SetDarkStarAsThreat;
        DarkStar.DarkStarIsStable += this.RemoveDarkStarAsThreat;
        ourGameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
    }
    bool threatenedByDarkStarGrowth;
    void SetDarkStarAsThreat()
    {
        if (Vector2.Distance(transform.parent.position, GameStateHandler.DarkStarGO.transform.position) < safeDistance)
        {
            TriggerThreatReaction(ourGameStateHandler.darkStar);
            threatenedByDarkStarGrowth = true;
        }
    }

    void RemoveDarkStarAsThreat()
    {

        if (potentialTreatsInTrigger.Count == 0)
        {
            GiveAllClearSignal();

        }
        threatenedByDarkStarGrowth = false;

    }
    public event Action<GameObject> threatInArea;

    void TriggerThreatReaction(GameObject threat)
    {
        if (threatInArea != null)
        {
            threatInArea(threat);
        }

    }
    void CheckIfWeKilledEnemy(GameObject source, SpaceMonster deadMonster)
    {
        if (source == this.gameObject)
        {
            devouredEnemy = true;
        }
    }


 void SubscribeToTargetEvents(GameObject target){
        target.GetComponent<Health>().Died += this.CheckIfWeKilledEnemy;
    }

    void UnsubscribeFromTargetEvents(GameObject target){
        target.GetComponent<Health>().Died -= this.CheckIfWeKilledEnemy;
        
    }
    public event Action SetAllClear;
    void GiveAllClearSignal()
    {
        if (SetAllClear != null)
        {
            SetAllClear();
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.GetComponent<SpaceMonster>() != null)
        {
            if (applyingDamageThroughTrigger)
            {
                //if we're currently applying damage somehow, add this damage source once it enters
                Health health = hit.GetComponent<Health>();
                if (!health.persistentDamageSources.Contains(transform.parent.gameObject))
                {
                    health.AddDamageSource(this.gameObject);
                    health.Died += this.CheckIfWeKilledEnemy;
                }

            }
            if (applyingIncapacitationThroughTrigger)
            {

                //if we're currently applying incapacitation somehow, add this source once it leaves
                UniversalMovement theirMovement = hit.GetComponent<UniversalMovement>();
                if (!theirMovement.incapacitationSources.Contains(transform.parent.gameObject))
                {
                    theirMovement.AddIncapacitationSource(this.gameObject);
                }

            }

            enemiesInThreatTrigger.Add(hit.gameObject);
        }
        if (hit.GetComponent<PlayerReferences>() != null)
        {
            //TODO: Fix this even further -- the comet for example neds to know if the player is in the trigger and that's all that it worries about
            DetectedPlayerInTrigger();
            if (applyingIncapacitationThroughTrigger)
            {
                hit.GetComponent<PlayerMovement>().AddIncapacitationSource(this.gameObject);
            }
        }
        if (GetComponent<BlueDwarf>() != null)
        {
            BlueDwarf anotherBlueDwarf = hit.GetComponent<BlueDwarf>();
            GoapAgent goapAgent = hit.GetComponent<GoapAgent>();
            PlayerReferences pReference = hit.GetComponent<PlayerReferences>();
            Hookshot hookshot = hit.GetComponent<Hookshot>();
            PlayerTriggerHandler playerTrigger = hit.GetComponent<PlayerTriggerHandler>();
            if (goapAgent != null || pReference != null || hookshot != null || playerTrigger != null)
            {
                if (anotherBlueDwarf == null)
                {
                    potentialTreatsInTrigger.Add(gameObject);
                    TriggerThreatReaction(hit.gameObject);
                }
            }
            if (threatenedByDarkStarGrowth)
            {

                //add something that takes radius into account?
                //TriggerThreatReaction(ourGameStateHandler.darkStar);

            }
        }

    }

    void OnTriggerExit2D(Collider2D hit)
    {

        if (hit.GetComponent<SpaceMonster>() != null)
        {
             if (applyingDamageThroughTrigger)
            {
                //if we're currently applying damage somehow, add this damage source once it enters
                Health health = hit.GetComponent<Health>();
                if (health.persistentDamageSources.Contains(transform.parent.gameObject))
                {
                    health.RemovePersistentDamageSource(this.gameObject);
                    health.Died += this.CheckIfWeKilledEnemy;
                }

            }
            if (applyingIncapacitationThroughTrigger)
            {
                UniversalMovement theirMovement = hit.GetComponent<UniversalMovement>();
                if (theirMovement.incapacitationSources.Contains(transform.parent.gameObject))
                {
                    theirMovement.RemoveIncapacitationSource(this.gameObject);
                }
            }
            enemiesInThreatTrigger.Remove(hit.gameObject);
        }

        if (GetComponent<BlueDwarf>() != null)
        {
            BlueDwarf anotherBlueDwarf = hit.GetComponent<BlueDwarf>();
            GoapAgent goapAgent = hit.GetComponent<GoapAgent>();
            PlayerReferences pReference = hit.GetComponent<PlayerReferences>();
            PlayerTriggerHandler playerTrigger = hit.GetComponent<PlayerTriggerHandler>();
            if (goapAgent != null || pReference != null || playerTrigger != null)
            {
                if (playerTrigger != null)
                {
                    Debug.Log("We sense a player trigger");
                }
                if (anotherBlueDwarf == null)
                {
                    potentialTreatsInTrigger.Remove(gameObject);
                    if (potentialTreatsInTrigger.Count == 0)
                    {
                        SetAllClear();

                    }
                }
            }
            if (threatenedByDarkStarGrowth)
            {

                //add something that takes radius into account?
                // TriggerThreatReaction(ourGameStateHandler.darkStar);
            }
        }
    }
    // Use this for initialization

}
