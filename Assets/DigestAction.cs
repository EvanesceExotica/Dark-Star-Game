using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigestAction : GoapAction {

	public float startTime;
	public float digestionDuration = 15.0f;
	public DigestAction(){
		cost = 200;
		AddPrecondition(new Condition("eat", true));
		AddEffect(new Condition("growLarger", true));

	}
	// Use this for initialization
	bool hasDigested;

	public override void reset(){
		hasDigested = false;
	}
	public override bool requiresInRange(){
		return false;
	}

	public override bool checkProceduralPrecondition(GameObject agent){
		return true;
	}
	public override bool perform(GameObject agent){
		if(!performing){
			StartCoroutine(Digest());
		}
		performing = true;
		if(interrupted){
			performing = false;
		}
		return performing;
	}

	public override bool isDone(){
		return hasDigested;
	}

	public IEnumerator Digest(){

		startTime = Time.time;
		Vector2 originalScale = transform.localScale;
		Vector2 destinationScale = new Vector2(transform.localScale.x * 1.5f, transform.localScale.y * 1.5f) ;
		float currentTime = 0.0f;

		while(Time.time < startTime + digestionDuration){

			if(interrupted){
				yield break;
			}
			transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime/digestionDuration);
			currentTime+= Time.deltaTime;

			yield return null;
		}
		hasDigested = true;
	}
	
	
	// Update is called once per frame
	void Update () {
		

	}
}
