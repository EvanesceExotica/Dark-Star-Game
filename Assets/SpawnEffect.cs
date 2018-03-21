using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : PooledObject {

	public ParticleSystem ps;
	public float startTime;
	public float duration;

	public ParticleSystem.MainModule main;

	public virtual void Awake(){
		ps = GetComponent<ParticleSystem>();
		main = ps.main;
	//	duration = main.simulationSpeed;
	}
	public virtual void Start(){
		//main = ps.main;
		duration = 3.0f;

	}
	public virtual void OnEnable(){
		
		//TODO: Figure out why the particle systems aren't playing
		ps.Play();
		startTime = Time.time;
		
	}
	
	public virtual void Update () {
		if(Time.time > startTime + duration){
			this.ReturnToPool();	
		}
		
	}
}
