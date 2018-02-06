using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleStarParticles : MonoBehaviour {

    public List<GameObject> particleSystemGameObjectsToScale;
    public GameObject standardStarParticlesGO;

    public GameObject starTooBigGO;

    public GameObject starUltraNovaGO;

	public GameObject starUltraNovaTwoGO;
	void Awake(){

       particleSystemGameObjectsToScale.Add(standardStarParticlesGO) ;
       particleSystemGameObjectsToScale.Add(starTooBigGO);
       particleSystemGameObjectsToScale.Add(starUltraNovaGO);
	   particleSystemGameObjectsToScale.Add(starUltraNovaTwoGO);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
