using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public class ShowDarkStarPhase : MonoBehaviour
{

    Text ourText;
   public static DarkStarPhases CurrentStarPhase;
    public float PullPhaseDuration;
    public float PushPhaseDuration;
    public static event Action ChangingToPullPhase;
    public static event Action ChangingToPushPhase;
    public float pullPhaseTime;
    public float pushPhaseTime;

    public SpriteRenderer PushSpriteRenderer;
    public SpriteRenderer PullSpriteRenderer;
     
    public static DarkStarPhases GetPhase()
    {
        return CurrentStarPhase;
    }

    void ShowPhase()
    {
        if(CurrentStarPhase == DarkStarPhases.pullPhase)
        {
            ourText.text = "Pull Phase";
        }
        else if(CurrentStarPhase == DarkStarPhases.pushPhase)
        {
            ourText.text = "Push Phase";
        }
    }
    void HidePhase()
    {
        ourText.text = " ";
    }
   public enum DarkStarPhases
    {
        none,
        pushPhase,
        pullPhase
    }

    void DisableMyself()
    {
       // Debug.Log(this.name + "Disabled itself");
        this.enabled = false;
        HideBothPhases();
    }


    private void Awake()
    {
        ourText = GetComponent<Text>();
        GameStateHandler.DarkPhaseStarted += this.DisableMyself;
     //   SpaceTimeTear.PlayerHookedOn += this.ShowPhase;
      //  SpaceTimeTear.PlayerReleased += this.HidePhase;
    }

    public  void Update() { 


		if(CurrentStarPhase == DarkStarPhases.pullPhase){
			pullPhaseTime += Time.deltaTime;
		}
        else if (CurrentStarPhase == DarkStarPhases.pushPhase) { 
		
			pushPhaseTime +=Time.deltaTime;
		}
      

		if (pullPhaseTime >= PullPhaseDuration)
		{
			StartingToPush();
            ShowPushPhase();
			pullPhaseTime = 0;
		}
        if (pushPhaseTime >= PushPhaseDuration)
        {
            StartingToPull();
            ShowPullPhase(); 
            pushPhaseTime = 0;
        }
       			
	}


    void ShowPushPhase()
    {
        PullSpriteRenderer.enabled = false;
        PushSpriteRenderer.enabled = true;
    }

    void ShowPullPhase()
    {

        PushSpriteRenderer.enabled = false;
        PullSpriteRenderer.enabled = true;

    }
    void HideBothPhases()
    {
        PushSpriteRenderer.enabled = false;
        PullSpriteRenderer.enabled = false;
    }

    public static void StartingToPull() {
        if (ChangingToPullPhase != null) {
            ChangingToPullPhase();
        }
        CurrentStarPhase = DarkStarPhases.pullPhase;
     //   Debug.Log("Current Phase is now Pull Phase");           
	}

  
	public static void StartingToPush(){
        if (ChangingToPushPhase != null)
        {
            ChangingToPushPhase();
        }
        CurrentStarPhase = DarkStarPhases.pushPhase;
      //  Debug.Log("Current Phase is now Push Phase");
	}
    void Start()
    {
        PushPhaseDuration = 5.0f;
        PullPhaseDuration = 5.0f;
        CurrentStarPhase = DarkStarPhases.pullPhase;
    }

    // Update is called once per frame
}
