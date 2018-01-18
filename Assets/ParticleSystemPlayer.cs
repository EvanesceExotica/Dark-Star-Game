using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleSystemPlayer  {

	public static void PlayChildParticleSystems(List<ParticleSystem> allParticlesInChildren)
    {
        
        foreach(ParticleSystem ps in allParticlesInChildren)
        {
           
            ps.Play();
        }
    }

    public static void StopChildParticleSystems(List<ParticleSystem> allParticlesInChildren)
    {
        foreach(ParticleSystem ps in allParticlesInChildren)
        {
            ps.Stop();
        }
    }
}
