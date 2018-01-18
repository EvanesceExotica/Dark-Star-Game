using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ThreatTrigger : MonoBehaviour
{


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
