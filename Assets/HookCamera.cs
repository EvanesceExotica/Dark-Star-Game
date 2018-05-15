using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCamera : MonoBehaviour {

	//This should only go on the Hook Object
	public void Awake(){
		Camera camera = GetComponent<Camera>();
		//Hookshot.Scoping += this.ZoomOutToFollowCamera;
		//Hookshot.StoppedScoping += this.ZoomInToOnlyFollowPlayer;
	}

	float defaultSize;

	void ActivateCamera(){

	}

	void ZoomOutToFollowCamera(){
		Debug.Log("Following hook!");
		//This should zoom slowly out while the hook gets farther to allow the player to see more in front of them. Perhaps the collider should be turned off when scoping?	
		GameStateHandler.ourProCamera2D.AddCameraTarget(this.transform)	;
		GameStateHandler.ourProCamera2D.RemoveCameraTarget(GameStateHandler.player.transform);
		//GameStateHandler.ourProCamera2D.Zoom(10, 3.0f);
	}

	void ZoomInToOnlyFollowPlayer(){
		
		Debug.Log("Stopped following hook!");
		GameStateHandler.ourProCamera2D.RemoveCameraTarget(this.transform);
		GameStateHandler.ourProCamera2D.AddCameraTarget(GameStateHandler.player.transform);
		GameStateHandler.ourProCamera2D.Reset();
		//GameStateHandler.ourProCamera2D.Zoom(10, 3.0f);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
