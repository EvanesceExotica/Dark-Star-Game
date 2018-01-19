using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThreatTrigger : MonoBehaviour
{

    List<GameObject> potentialTreatsInTrigger = new List<GameObject>();
    GameStateHandler ourGameStateHandler;
    void Awake()
    {
        DarkStar.DarkStarIsGrowing += this.SetDarkStarAsThreat;
        DarkStar.DarkStarIsStable += this.RemoveDarkStarAsThreat;
        ourGameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
    }
    bool threatenedByDarkStarGrowth;
    void SetDarkStarAsThreat()
    {

        TriggerThreatReaction(ourGameStateHandler.darkStar);
        threatenedByDarkStarGrowth = true;
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
        BlueDwarf anotherBlueDwarf = hit.GetComponent<BlueDwarf>();
        GoapAgent goapAgent = hit.GetComponent<GoapAgent>();
        PlayerReferences pReference = hit.GetComponent<PlayerReferences>();
        if (goapAgent != null || pReference != null)
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

    void OnTriggerExit2D(Collider2D hit)
    {

        BlueDwarf anotherBlueDwarf = hit.GetComponent<BlueDwarf>();
        GoapAgent goapAgent = hit.GetComponent<GoapAgent>();
        PlayerReferences pReference = hit.GetComponent<PlayerReferences>();
        if (goapAgent != null || pReference != null)
        {
            if (anotherBlueDwarf == null)
            {
                potentialTreatsInTrigger.Remove(gameObject);
                if (potentialTreatsInTrigger.Count == 0)
                {

                }
            }
        }
        if (threatenedByDarkStarGrowth)
        {

            //add something that takes radius into account?
            TriggerThreatReaction(ourGameStateHandler.darkStar);
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
