using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarChain : PowerUp {

	DistanceJoint2D ourDistanceJoint;
	SpringJoint2D ourSpringJoint;
	LineRenderer ourLineRenderer;

	
		Color whiteWithZeroAlpha = new Color(Color.white.r, Color.white.g, Color.white.b, 0);
	bool chained;
	public override void Awake(){
		base.Awake();

		playerReferences = GetComponent<PlayerReferences>();
		ourDistanceJoint = GetComponent<DistanceJoint2D>();
		ourSpringJoint = GetComponent<SpringJoint2D>();

		ChoosePowerUp.chainChosen += this.SetPoweredUp;
		Switch.SwitchEntered += this.SetOnSwitch;
		Switch.SwitchExited += this.SetOffSwitch;
		ourRequirement = Requirement.OnlyUseOffSwitch;
		autoActivated = true;

		ourLineRenderer = transform.Find("ChainToStar").GetComponent<LineRenderer>();
		ourSpringJoint.enabled = false;
		ourLineRenderer.startColor = whiteWithZeroAlpha;
		ourLineRenderer.endColor = whiteWithZeroAlpha;
		ourLineRenderer.SetPosition(0, GameStateHandler.DarkStarGO.transform.position);
		ourLineRenderer.SetPosition(1, gameObject.transform.position);
		//ourLineRenderer.enabled = false;
		ourDistanceJoint.enabled = false;
	}

	void Start(){

		ourLineRenderer.SetPosition(0, GameStateHandler.DarkStarGO.transform.position);
		ourLineRenderer.SetPosition(1, gameObject.transform.position);
	}
	
	PlayerReferences playerReferences;

	public override void StartPowerUp(){
		base.StartPowerUp();
		StartCoroutine(BeginChain());
	}
	void StartChain(){
		StartCoroutine(BeginChain());
	}
	IEnumerator BeginChain(){

		StartCoroutine(FadeChainIn(2.0f));
		yield return new WaitForSeconds(0.24f);
		chained = true;
	//	ourLineRenderer.enabled = true;
		ourSpringJoint.enabled = true;
		//ourDistanceJoint.enabled = true;
		//ourLineRenderer.SetPosition(0, ourDistanceJoint.anchor);
	//	ourLineRenderer.SetPosition(1, ourDistanceJoint.connectedAnchor);
		ourLineRenderer.SetPosition(0, GameStateHandler.DarkStarGO.transform.position);
		ourLineRenderer.SetPosition(1, gameObject.transform.position);
		yield return new WaitForSeconds(1.5f);
		ourSpringJoint.enabled = false;
		StartCoroutine(FadeChainOut(2.0f));
		EndChain();	

	}

	void EndChain(){
		chained = false;
		StoppedUsingPowerUpWrapper();
	}

	public IEnumerator FadeChainIn(float speed){
		while(ourLineRenderer.startColor.a < 1){
			float newValue = ourLineRenderer.startColor.a +  2 * Time.deltaTime;
			Color newColor = new Color(ourLineRenderer.startColor.r, ourLineRenderer.startColor.b, ourLineRenderer.startColor.b, newValue);
			ourLineRenderer.startColor = newColor;
			float newValue2 = ourLineRenderer.endColor.a +  2 * Time.deltaTime;
			Color newColor2 = new Color(ourLineRenderer.endColor.r, ourLineRenderer.endColor.b, ourLineRenderer.endColor.b, newValue2);
			ourLineRenderer.endColor = newColor2;
			yield return null;	
		}
	}

	public IEnumerator FadeChainOut(float speed){
		while(ourLineRenderer.startColor.a > 0){
			float newValue = ourLineRenderer.startColor.a -  2 * Time.deltaTime;
			Color newColor = new Color(ourLineRenderer.startColor.r, ourLineRenderer.startColor.b, ourLineRenderer.startColor.b, newValue);
			ourLineRenderer.startColor = newColor;
			float newValue2 = ourLineRenderer.endColor.a -  2 * Time.deltaTime;
			Color newColor2 = new Color(ourLineRenderer.endColor.r, ourLineRenderer.endColor.b, ourLineRenderer.endColor.b, newValue2);
			ourLineRenderer.endColor = newColor2;
			yield return null;	
		}
	}
	
	public override void Update () {
		base.Update();
		if(Input.GetKeyDown(KeyCode.U)){
			StartCoroutine(FadeChainIn(2.0f));
		}
		if(Input.GetKeyDown(KeyCode.I)){
			StartCoroutine(FadeChainOut(2.0f));
		}
		if(chained){
			ourLineRenderer.SetPosition(0, GameStateHandler.DarkStarGO.transform.position);
			ourLineRenderer.SetPosition(1, gameObject.transform.position);
		}
		
	}
}
