using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryCounter : MonoBehaviour {

    bool illuminationAtMax = true;
    Text ourTimerText;
    CanvasGroup ourCanvasGroup;

    private void Awake()
    {
        ourTimerText = GetComponent<Text>();
        ourCanvasGroup = GetComponentInParent<CanvasGroup>();
        DarkStar.IlluminationAtMax += this.CountdownWrapper;
    }

    private void OnEnable()
    {
        DarkStar.IlluminationAtMax += this.CountdownWrapper;
        DarkStar.LostMaxIllumination += this.CancelCountdown;
    }

    private void OnDisable()
    {
        DarkStar.IlluminationAtMax -= this.CountdownWrapper;
        DarkStar.LostMaxIllumination -= this.CancelCountdown;
    }

    void CancelCountdown()
    {
        illuminationAtMax = false;
    }

    void CountdownWrapper(float duration)
    {
        Debug.Log("THIS HAPPENED");
        //FadeUIHelperClass.FadeInWrapper(this, 5.0f, ourCanvasGroup);
        StartCoroutine(CountDownToVictory(duration));
    }

    public IEnumerator CountDownToVictory(float duration)
    {
        float currentNumber = duration;
        float goalNumber = 0.0f;

        while (currentNumber > goalNumber)
        {
            //if (!illuminationAtMax)
            //{
            //    break;
            //}
            ourTimerText.text = ((int)currentNumber).ToString();
            currentNumber -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }

       // FadeUIHelperClass.FadeOutWrapper(this, 5.0f, ourCanvasGroup);
    }

 


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
