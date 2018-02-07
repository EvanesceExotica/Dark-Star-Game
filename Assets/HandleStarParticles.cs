using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class HandleStarParticles : MonoBehaviour
{

    public List<GameObject> particleSystemGameObjectsToScale;
    public GameObject standardStarParticlesGO;

    List<ParticleSystem> standardStarParticles;

    public GameObject starTooBigGO;

    public List<ParticleSystem> starTooBigParticles;

    public GameObject starUltraNovaGO;

    public List<ParticleSystem> UltraNovaParticles;

    public GameObject starUltraNovaTwoGO;

    public List<ParticleSystem> UltraNovaTwoParticles;
    void Awake()
    {

        particleSystemGameObjectsToScale.Add(standardStarParticlesGO);
        particleSystemGameObjectsToScale.Add(starTooBigGO);
        particleSystemGameObjectsToScale.Add(starUltraNovaGO);
        particleSystemGameObjectsToScale.Add(starUltraNovaTwoGO);
        starTooBigParticles = starTooBigGO.GetComponentsInChildren<ParticleSystem>().ToList();
        UltraNovaParticles = starUltraNovaGO.GetComponentsInChildren<ParticleSystem>().ToList();
        UltraNovaTwoParticles = starUltraNovaTwoGO.GetComponentsInChildren<ParticleSystem>().ToList();
        standardStarParticles = standardStarParticlesGO.GetComponentsInChildren<ParticleSystem>().ToList();
        DarkStarTooBig.DarkStarReachedTooLargeBounds += PlayTooLargeSystem;
        DarkStar.Overcharged += PlayFinalExplosionParticles;
    }

    void PlayTooLargeSystem()
    {
        ParticleSystemPlayer.StopChildParticleSystems(standardStarParticles);
        ParticleSystemPlayer.PlayChildParticleSystems(starTooBigParticles);
    }

    void PlayFinalExplosionParticles()
    {
        ParticleSystemPlayer.StopChildParticleSystems(starTooBigParticles);
        ParticleSystemPlayer.PlayChildParticleSystems(UltraNovaParticles);
        ParticleSystemPlayer.PlayChildParticleSystems(UltraNovaTwoParticles);

    }
    // Use this for initialization

}
