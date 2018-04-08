using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceUpAndDown : MonoBehaviour {

	public float amplitude;
	public float frequency;

	public float speed;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float yPos = amplitude + Mathf.Sin(Time.timeSinceLevelLoad * frequency);
		transform.position = new Vector2(transform.position.x, yPos);
	}
}
