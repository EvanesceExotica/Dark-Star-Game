using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTransformSpot : PooledObject {

	public Transform parent;

	public void SetParent(){
		parent = transform.parent;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
