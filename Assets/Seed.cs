using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SeedPlanet : MonoBehaviour {

    public static event Action LifeSeedPlanted;
    public static event Action<GameObject> PlanetHarvested;
    public LocationHandler ourLocationHandler;

    void PlantLifeSeed()
    {
        if (LifeSeedPlanted != null && ourLocationHandler.onPlanet)
            LifeSeedPlanted(); 
    }

    void HarvestPlanet()
    {
        if(PlanetHarvested != null && ourLocationHandler.onPlanet && ourLocationHandler.currentPlanet.GetComponent<Planet>().ourPlanetType == Planet.planetType.fullOfLife) //make the current planet an actual planet)
        {
            PlanetHarvested(ourLocationHandler.currentPlanet);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HarvestPlanet(); 
        }
		
	}
}
