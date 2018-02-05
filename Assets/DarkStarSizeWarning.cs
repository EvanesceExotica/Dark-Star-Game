using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class DarkStarSizeWarning : MonoBehaviour {

	public static event Action DarkStarGettingTooLarge;
	public static event Action DarkStarLosingSize;

	public void DarkStarGettingTooLargeWrapper(){
		if(DarkStarGettingTooLarge != null){
			DarkStarGettingTooLarge();
		}
	}

	public void DarkStarLosingSizeWrapper(){
		if(DarkStarLosingSize != null){
			DarkStarLosingSize();
		}
	}
	void OnTriggerEnter2D(Collider2D hit){
		if(hit.gameObject == GameStateHandler.DarkStarGO){
			DarkStarGettingTooLargeWrapper();

		}
	}


	void OnTriggerExit2D(Collider2D hit){
		if(hit.gameObject == GameStateHandler.DarkStarGO){
			DarkStarLosingSizeWrapper();
		}
	}

}
