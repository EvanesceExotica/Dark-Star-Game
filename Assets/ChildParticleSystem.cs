using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChildParticleSystem : MonoBehaviour {

    public HandleChildParticleSystems.ParticleSystemObjectTypes ourPSObjectType; 

    public List<ParticleSystem> particleSystemsInChildren = new List<ParticleSystem>();

    private void Awake()
    {
        particleSystemsInChildren = GetComponentsInChildren<ParticleSystem>().ToList(); 
    }

    public List<ParticleSystem> GetParticleSystems()
    {
        if(particleSystemsInChildren.Count == 0)
        {
            //Debug.Log("Nothing to get!");
            return null;
        }
        return particleSystemsInChildren;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
