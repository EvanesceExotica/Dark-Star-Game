using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarlightHandler : MonoBehaviour {

    Light starLight;
    float defaultIntensity;
    float currentIntensity;

    float intensityIncreaseDuration;

    void AdjustIntensity(float adjustmentValue)
    {
        StartCoroutine(AdjustIntensityCoroutine(adjustmentValue));

       // starLight.intensity += adjustmentValue;
    }

    IEnumerator AdjustIntensityCoroutine(float adjustmentValue)
    {
        float newIntensity = currentIntensity + adjustmentValue;

        float currentTime = 0.0f;
        while (currentTime < intensityIncreaseDuration)
        {
            starLight.intensity = Mathf.Lerp(starLight.intensity, newIntensity, currentTime);
            currentTime += Time.deltaTime;
        }

        while (starLight.intensity < newIntensity)
        {
            starLight.intensity += 1.0f * Time.deltaTime;
            yield return null;
        }
    }

    private void OnEnable()
    {
        DarkStar.AdjustLuminosity += this.AdjustIntensity;
    }

    private void OnDisable()
    {
        DarkStar.AdjustLuminosity -= this.AdjustIntensity;
    }

    // Use this for initialization
    void Start () {
        starLight = GetComponent<Light>(); 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
