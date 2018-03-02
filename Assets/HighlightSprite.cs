using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSprite : MonoBehaviour {

	_2dxFX_AL_Frozen ourFrozenEffect;

	void DisplayFrozenEffect(){
		ourFrozenEffect.enabled = true;
	}	

	void HideFrozenEffect(){

		ourFrozenEffect.enabled = false;
	}

	void Awake(){
		ourFrozenEffect = GetComponent<_2dxFX_AL_Frozen>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
