using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

    public GameObject body;
    public float orbitSpeed; 
	// Use this for initialization
	void Start () {
        orbitSpeed = 10.0f;
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(body.transform.position, Vector3.forward, orbitSpeed * Time.deltaTime);
	
	}
}
