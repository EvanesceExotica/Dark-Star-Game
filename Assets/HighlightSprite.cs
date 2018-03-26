using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSprite : MonoBehaviour {

	_2dxFX_Frozen ourFrozenEffect;

	UniversalMovement ourUniversalMovement;

	void OnEnable(){
		ourUniversalMovement.SomethingImpededOurMovement += this.DisplayFrozenEffect;
		ourUniversalMovement.SomethingStoppedImpedingOurMovement += this.HideFrozenEffect;
	}


	void OnDisable(){
		ourUniversalMovement.SomethingImpededOurMovement -= this.DisplayFrozenEffect;
		ourUniversalMovement.SomethingStoppedImpedingOurMovement -= this.HideFrozenEffect;
	}
	public void DisplayFrozenEffect(GameObject irrelevant){
		ourFrozenEffect.enabled = true;
	}	

	public void HideFrozenEffect(GameObject irrelevant){

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
