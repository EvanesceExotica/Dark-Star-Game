using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour {

	public PowerUp currentPowerUp;

	[SerializeField] bool powerUpCurrentlyPlaying;

	bool onSwitch;

	public PowerUp[] OnSwitchPowerUps = new PowerUp[3];
	public PowerUp[] OffSwitchPowerUps = new PowerUp[3];
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
