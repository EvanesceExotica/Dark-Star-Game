using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class DarkStarTooBig : MonoBehaviour {

public static event Action DarkStarReachedTooLargeBounds;
public static event Action DarkStarReceeded;
void DarkStarTooLarge(){
	if(DarkStarReachedTooLargeBounds != null){
		DarkStarReachedTooLargeBounds();
	}
}

void DarkStarReceededWrapper(){
	if(DarkStarReceeded != null){
		DarkStarReceeded();
	}
}
	void OnTriggerEnter2D(Collider2D hit){
		if(hit.gameObject == GameStateHandler.DarkStarGO){
			DarkStarTooLarge();
		}
	}

	void OnTriggerExit2D(Collider2D hit){
		if(hit.gameObject == GameStateHandler.DarkStarGO){
			DarkStarReceeded();
		}
	}

}
