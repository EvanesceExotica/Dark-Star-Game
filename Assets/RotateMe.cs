using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour {

	public Vector3 rotationSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotationSpeed * Time.deltaTime);
		
	}
}
