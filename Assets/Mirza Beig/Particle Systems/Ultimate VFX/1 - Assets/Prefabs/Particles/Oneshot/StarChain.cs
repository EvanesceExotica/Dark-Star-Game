using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarChain : MonoBehaviour {

	DistanceJoint2D ourDistanceJoint;
	SpringJoint2D ourSpringJoint;
	LineRenderer ourLineRenderer;

	bool chained;
	void Awake(){
		playerReferences = GetComponent<PlayerReferences>();
		ourDistanceJoint = GetComponent<DistanceJoint2D>();
		ourSpringJoint = GetComponent<SpringJoint2D>();
		ChoosePowerUp.chainChosen += this.StartChain;
		ourLineRenderer = transform.Find("ChainToStar").GetComponent<LineRenderer>();
		ourSpringJoint.enabled = false;
		ourLineRenderer.enabled = false;
		ourDistanceJoint.enabled = false;
	}
	
	PlayerReferences playerReferences;
	void StartChain(){
		Debug.Log("Chain should be started");
		chained = true;
		ourLineRenderer.enabled = true;
		ourSpringJoint.enabled = true;
		//ourDistanceJoint.enabled = true;
		//ourLineRenderer.SetPosition(0, ourDistanceJoint.anchor);
	//	ourLineRenderer.SetPosition(1, ourDistanceJoint.connectedAnchor);
		ourLineRenderer.SetPosition(0, GameStateHandler.DarkStarGO.transform.position);
		ourLineRenderer.SetPosition(1, gameObject.transform.position);
		//playerReferences.rb.velocity = new Vector2(0, 0);

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(chained){
			ourLineRenderer.SetPosition(0, GameStateHandler.DarkStarGO.transform.position);
			ourLineRenderer.SetPosition(1, gameObject.transform.position);
		}
		
	}
}
