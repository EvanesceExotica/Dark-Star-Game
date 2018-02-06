using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDarkStarColor : MonoBehaviour
{

	Color previousColor;
	void Awake(){
		DarkStarSizeWarning.DarkStarGettingTooLarge += TriggerWarningColor;
		DarkStarSizeWarning.DarkStarLosingSize += ResetToPreviousColor;
		DarkStarTooBig.DarkStarReachedTooLargeBounds += TriggerDoomColor;
		DarkStarTooBig.DarkStarReceeded += ResetToPreviousColor;
	}
    SpriteRenderer ourSpriteRenderer;
    // Use this for initialization
    void Start()
    {
        ourSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

	void ResetToPreviousColor(){
		ourSpriteRenderer.color = previousColor;
	}

	void TriggerWarningColor(){
		previousColor = ourSpriteRenderer.color;
		ChangeColorWrapper(ourSpriteRenderer.color, Color.yellow, 4.0f)	;
	}

	void TriggerDoomColor(){
		Debug.Log("We're changing color now");
		previousColor = ourSpriteRenderer.color;
		ChangeColorWrapper(ourSpriteRenderer.color, Color.red, 3.0f);

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
			time += Time.deltaTime/duration;
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
