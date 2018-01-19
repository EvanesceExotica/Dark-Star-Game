using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThreatTrigger : MonoBehaviour
{

GameStateHandler ourGameStateHandler;
void Awake(){
    DarkStar.DarkStarIsGrowing += this.SetDarkStarAsThreat;
    DarkStar.DarkStarIsStable+= this.RemoveDarkStarAsThreat;
    ourGameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
}
bool threatenedByDarkStarGrowth;
void SetDarkStarAsThreat(){

threatenedByDarkStarGrowth = true;
}

void RemoveDarkStarAsThreat(){

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

    private void OnTriggerEnter2D(Collider2D hit)
    {
        BlueDwarf anotherBlueDwarf = hit.GetComponent<BlueDwarf>();
        GoapAgent goapAgent = hit.GetComponent<GoapAgent>();
        PlayerReferences pReference = hit.GetComponent<PlayerReferences>();
        if (goapAgent != null || pReference != null)
        {
            Debug.Log("something entered our sphere");
            if (anotherBlueDwarf == null)
            {        
                TriggerThreatReaction(hit.gameObject);
            }
        }
        if(threatenedByDarkStarGrowth){

            Debug.Log("<color=red> OH SHIT DARK STAR IS GROWING </color");
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
