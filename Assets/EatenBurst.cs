using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatenBurst : SpawnEffect {



	public override void OnEnable(){
		base.OnEnable();
	}
	void Start () {
		ps = GetComponent<ParticleSystem>();
		var main = ps.main;
		main.simulationSpeed = 1.6f;
	}
	
	
	// Update is called once per frame
	public override void Update () {
		base.Update();	
	}
}
