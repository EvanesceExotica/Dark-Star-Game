using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSprite : MonoBehaviour {

	_2dxFX_Frozen ourFrozenEffect;

	UniversalMovement ourUniversalMovement;

	void OnEnable(){
		ourUniversalMovement.MovementStopped += this.DisplayFrozenEffect;
		ourUniversalMovement.MovementBegan += this.HideFrozenEffect;
	}


	void OnDisable(){
		ourUniversalMovement.MovementStopped -= this.DisplayFrozenEffect;
		ourUniversalMovement.MovementBegan -= this.HideFrozenEffect;
	}
	public void DisplayFrozenEffect(){
		ourFrozenEffect.enabled = true;
	}	

	public void HideFrozenEffect(){

		ourFrozenEffect.enabled = false;
	}

	void Awake(){
		ourFrozenEffect = GetComponent<_2dxFX_Frozen>();
		ourUniversalMovement = GetComponent<UniversalMovement>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
