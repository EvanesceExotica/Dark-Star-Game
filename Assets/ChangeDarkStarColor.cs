using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDarkStarColor : MonoBehaviour
{

	DarkStar darkStar;
	Color previousColor;
	void Awake(){
		DarkStarSizeWarning.DarkStarGettingTooLarge += FlareWarningColor;
		DarkStarSizeWarning.DarkStarLosingSize += ResetToPreviousColor;
		DarkStarTooBig.DarkStarReachedTooLargeBounds += FlareDoomColor;
		DarkStarTooBig.DarkStarReceeded += ResetToPreviousColor;
	}
    SpriteRenderer ourSpriteRenderer;
	Light ourStarlight;
    // Use this for initialization
    void Start()
    {
        ourSpriteRenderer = GetComponent<SpriteRenderer>();
		ourStarlight = GetComponent<Light>();
		darkStar = GetComponent<DarkStar>();
    }

	void ResetToPreviousColor(){
		ourSpriteRenderer.color = previousColor;
	}

	void FlareWarningColor(){
		previousColor = ourSpriteRenderer.color;
		ChangeColorWrapper(ourSpriteRenderer.color, DarkStar.warningColor, 4.0f)	;
	}

	void FlareDoomColor(){
		Debug.Log("We're changing color now");
		previousColor = ourSpriteRenderer.color;
		ChangeColorWrapper(ourSpriteRenderer.color, DarkStar.doomColor, 3.0f);

	}
    public void ChangeColorWrapper(Color startColor, Color endColor, float duration)

    {
        StartCoroutine(ChangeColor(startColor, endColor, duration));
    }
    public IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
    {

        float startTime = Time.time;
		float time = 0;
        while (Time.time < startTime + duration)
        {
            ourSpriteRenderer.color = Color.Lerp(startColor, endColor, time);
			ourStarlight.color = Color.Lerp(startColor, endColor, time);
			time += Time.deltaTime/duration;
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
