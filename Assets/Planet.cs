using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    GameObject player;
    
    float currentLifeValue;
    float maxLifeValue;

    float lifeStartTime;
    float interval;
    float incraseAmount;

    bool lifeSpreading;
    bool lifeFulfilled;

    int maxSeedingsBeforeDestruction;
    int numberOfSeedings;

    public enum planetType
    {
        barren,
        growing,
        fullOfLife

    }

    public planetType ourPlanetType;

    private void Awake()
    {
        player = GameObject.Find("Player");
        SeedPlanet.LifeSeedPlanted += this.GenerateLifeWrapper;
    }

    private void OnDisable()
    {
        SeedPlanet.LifeSeedPlanted -= this.GenerateLifeWrapper;
    }


    void GenerateLifeWrapper()
    {
        StartCoroutine(GenerateLife());
    }

    public IEnumerator GenerateLife()
    {
        lifeSpreading = true;
        while (currentLifeValue < maxLifeValue)
        {
            currentLifeValue += incraseAmount;
            yield return interval;
        }

        lifeSpreading = false;
        lifeFulfilled = true;
        numberOfSeedings++; 
        
    }

    void BeHarvested()
    {
        if (ourPlanetType == planetType.fullOfLife)
        {
            if (numberOfSeedings < maxSeedingsBeforeDestruction)
            {
                lifeFulfilled = false;
                ourPlanetType = planetType.barren;
            }
            else
            {
                BeDestroyed();
            }

        }
        else if (ourPlanetType == planetType.growing)
        {
            //add souls to player AND
            BeDestroyed(); 
        }
        else if (ourPlanetType == planetType.barren)
        {
            BeDestroyed(); 
        }
    }

    void BeDestroyed()
    {

    }

	// Use this for initialization
	void Start () {
        lifeSpreading = false;
        lifeFulfilled = false;
        maxSeedingsBeforeDestruction = 2;
        numberOfSeedings = 0;
	}
	
	
}
