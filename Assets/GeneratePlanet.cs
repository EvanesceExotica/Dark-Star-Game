using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GeneratePlanet : MonoBehaviour
{

    float startTime;
    float duration;

    bool cometsOnSwitchGreaterThanTwo;
    public GameObject planetCreationParticles;

    public GameObject planetPrefab;

    public List<ParticleSystem> planetCreationParticlesList = new List<ParticleSystem>();

    public List<GameObject> CometsOnThisSwitch = new List<GameObject>();

    Switch ourSwitch;
    void Awake()
    {
        planetCreationParticles = transform.Find("PlanetGenerationParticleSystem").gameObject;
        planetCreationParticlesList = planetCreationParticles.GetComponentsInChildren<ParticleSystem>().ToList();
        ourSwitch = GetComponent<Switch>();
    }
    public void AddCometToSwitch(GameObject comet, GameObject potentiallyOurSwitch)
    {

        if (potentiallyOurSwitch == ourSwitch)
        {
            CometsOnThisSwitch.Add(comet);
			if(CometsOnThisSwitch.Count >= 2){

				cometsOnSwitchGreaterThanTwo = true;
				GeneratePlanetWrapper();
			}
        }
    }

    public void RemoveCometFromSwitch(GameObject comet, GameObject potentiallyOurSwitch)
    {
        if (potentiallyOurSwitch == ourSwitch)
        {
            CometsOnThisSwitch.Remove(comet);
			if(CometsOnThisSwitch.Count < 2){
				cometsOnSwitchGreaterThanTwo = false;
			}

        }
    }

	void PlayGenerationParticles(){
		ParticleSystemPlayer.PlayChildParticleSystems(planetCreationParticlesList);
	}
   void StopGenerationParticles(){
		ParticleSystemPlayer.StopChildParticleSystems(planetCreationParticlesList);
	}
    void GeneratePlanetWrapper()
    {
        StartCoroutine(GeneratePlanetCoroutine());
    }
    IEnumerator GeneratePlanetCoroutine()
    {
		PlayGenerationParticles();	
        while (Time.time < startTime + duration)
        {
            if (!cometsOnSwitchGreaterThanTwo)
            {
				//if we have less than two comets on the switch because the player pulled one away
                break;
            }
            yield return null;
        }
        InstantiatePlanetAtSwitch();
		StopGenerationParticles();
		//destroy the comets
		foreach(GameObject comet in CometsOnThisSwitch){
			comet.GetComponent<SpaceMonster>().ReturnToPool();
		}

		//TODO: Add an effect to stop the normal switch particles as well
    }

    void InstantiatePlanetAtSwitch()
    {
        GameObject newPlanet = Instantiate(planetPrefab, gameObject.transform.position, Quaternion.identity);
    }
    // Use this for initialization

}
