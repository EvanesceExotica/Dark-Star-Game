using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : PooledObject {

	public ParticleSystem ps;
	public float startTime;
	public float duration;
	public virtual void OnEnable(){
		startTime = Time.time;
		var main = ps.main;
		main.simulationSpeed = duration;
	}
	
	public virtual void Update () {
		if(Time.time > startTime + duration){
			this.ReturnToPool();	
		}
		
	}
}
