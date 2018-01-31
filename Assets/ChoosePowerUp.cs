using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosePowerUp : MonoBehaviour {

	bool acceptingCollision;
	Collider2D ourCollider;

	void Awake(){
		ourCollider = GetComponent<Collider2D>();
		ourCollider.enabled = false;
		LaunchSoul.SoulToBeLaunched += this.AcceptingCollision;
		LaunchSoul.SoulNotLaunching += this.NotAcceptingCollision;

	}

	void AcceptingCollision(){
		acceptingCollision = true;
		ourCollider.enabled = true;

	}

	void NotAcceptingCollision(){
		acceptingCollision = false;
		ourCollider.enabled = false;
	}
public string powerUpType;

	void OnTriggerEnter2D(Collider2D hit){

		SoulBehavior soulBehavior = hit.GetComponent<SoulBehavior>();
		if(soulBehavior != null){
			Debug.Log(powerUpType + " was chosen!")	;
			soulBehavior.ReturnToPool();
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
