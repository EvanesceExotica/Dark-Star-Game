using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class Doomclock : MonoBehaviour
{
    //TODO: Make it flash red when the wrong phase happens
    private bool cancelWait = false;
    public float timeUntilNextDisaster;
    public float defaultCooldownDuration; 
    public bool tickingDown;
    public float disasterCooldown;
    public bool disasterPlaying;
    public static event Action TriggerDisaster;
    public float disasterTimeOverflow; 

    void Start()
    {
        defaultCooldownDuration = 100.0f;
        //This should get shorter the larger the star is? So it sort of snowballs, but not too much, maybe like -5 ? 
        timeUntilNextDisaster = defaultCooldownDuration;
        disasterCooldown = 10.0f;
        tickingDown = false;
    }
    
    void StopClock()
    {
        tickingDown = false;
        timerImage.fillAmount = 0.0f;
    }

    void CheckForOverflow()
    {
        //this method checks to see if there was extra failure time. It will make the time until the last disaster even smaller.
        timeUntilNextDisaster = defaultCooldownDuration;
        if(disasterTimeOverflow > 0)
        {
            timeUntilNextDisaster -= disasterTimeOverflow;
        }
       // timerImage.fillAmount = 1;
    }

    void ResetClock()
    {
        disasterPlaying = false;
        timerImage.fillAmount = 1.0f;
    }

    void TimerAdjusted(float amount)
    {
        //if an emeny falls into the star, if the phase is correct, it will add time to the clock, if it's incorrect, it will subtract
    //i//    Debug.Log("Our timer has been adjusted!");
        if (amount > 0)
        {
            //if the phase was correct, and the amount greater than zero due to the phase being correct, add time
            StartCoroutine(flashColor(Color.green));

        }
        else if (amount < 0)
        {
            //if the phase didn't match, and the amount is less than zero due to that, subtract time
            StartCoroutine(flashColor(Color.red));



        }

//        Debug.Log("Here is our original fill amount " + timerImage.fillAmount);

        timerImage.fillAmount += amount / defaultCooldownDuration;
//        Debug.Log("Here is our UPDATED fill about " + timerImage.fillAmount);
        timeUntilNextDisaster += amount;
        if (timeUntilNextDisaster < 0)
            {
                disasterTimeOverflow += Mathf.Abs(timeUntilNextDisaster);
                timeUntilNextDisaster = 0.0f;
            }


    }

  //  IEnumerator DisasterPlayingStartCooldown()
    //{
    //    yield return new WaitForSeconds(disasterCooldown);
    //    disasterPlaying = false;

    //}

    void DisasterTriggered()
    {
        disasterPlaying = true;
      //  StartCoroutine(DisasterPlayingStartCooldown());
        if(TriggerDisaster != null)
        {
            TriggerDisaster();
        }
        StopClock();
        //reset time until next disaster
        //if it is equal to zero, take time off of next disaster
    }

    public IEnumerator flashColor(Color ourFlashColor)
    {

        timerImage.color = ourFlashColor;
        yield return new WaitForSeconds(0.3f);
        timerImage.color = Color.white;
    }
  public IEnumerator DisplayCooldownCounter()
    { 
        tickingDown = true;

        float startTime = Time.time;

        while (timeUntilNextDisaster > 0)
        {
            timeUntilNextDisaster-=1.0f;
            yield return new WaitForSeconds(1.0f);
         // yield return StartCoroutine(Wait(1.0f));
        }
        tickingDown = false;

        if (GameStateHandler.currentGameState != GameStateHandler.GameState.dark)
        {
            DisasterTriggered();

            CheckForOverflow();
        }
    }

	
	// Update is called once per frame
	void Update () {

        if (!tickingDown && !disasterPlaying)
        {
            StartCoroutine(DisplayCooldownCounter());
        }
        if(tickingDown)
        {
            //is this the issue? change this to reflect time left? 
            timerImage.fillAmount -= (Time.deltaTime/defaultCooldownDuration);
        }
	}
    IEnumerator Wait(float waitTime)
    {
        float t = 0.0f;
        while(t<= waitTime && !cancelWait)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }
    Image timerImage;
    
    // Use this for initialization
    void Awake()
    {
        GameStateHandler.DarkPhaseStarted += this.StopClock;
        DarkStar.AugmentDoomTimer += this.TimerAdjusted;
        //once the star is finished pulsing, reset the clock to countdown to the next disaster
        Pulse.DisasterCompleted += this.ResetClock;
        timerImage = GetComponent<Image>();
    }

    // Update is called once per frame
   
}
