﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThreatTrigger : MonoBehaviour
{


    public event Action PlayerInMyTrigger;
    public void DetectedPlayerInTrigger()
    {
        if (PlayerInMyTrigger != null)
        {
            PlayerInMyTrigger();
        }
    }
    GameObject currentThreat;
    GameStateHandler gameStateHandler;

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
        if (hit.GetComponent<PlayerReferences>() != null)
        {
            DetectedPlayerInTrigger();
        }
        if (hit.GetComponent<SpaceMonster>() != null)
        {
            enemiesInThreatTrigger.Add(hit.gameObject);
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
