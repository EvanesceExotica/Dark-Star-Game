using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertPSChildrenToList {

	public List<ParticleSystem> ConvertParentPSToList(GameObject particleSystemGameObject){
		List<ParticleSystem> childParticleSystems = new List<ParticleSystem>();
		foreach(ParticleSystem child in particleSystemGameObject.GetComponentsInChildren<ParticleSystem>()){
			childParticleSystems.Add(child);
		}
		return childParticleSystems;
	}
}
