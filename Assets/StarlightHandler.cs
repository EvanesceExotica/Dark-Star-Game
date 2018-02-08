using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarlightHandler : MonoBehaviour
{

    Light starLight;
    float defaultIntensity;
    float currentIntensity;

    float currentSize;

    float defaultSize;
    float intensityIncreaseDuration;

    void AdjustIntensityAndSize(float adjustmentValue)
    {
        StartCoroutine(AdjustIntensityCoroutine(adjustmentValue));
        StartCoroutine(AjdustRangeCoroutine(adjustmentValue));

        // starLight.intensity += adjustmentValue;
    }

    IEnumerator AdjustIntensityCoroutine(float adjustmentValue)
    {
        float newIntensity = starLight.intensity + adjustmentValue;
        float currentIntensity = starLight.intensity;

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

    void FadeLightCoroutineWrapper()
    {
        StartCoroutine(FadeLightCoroutine());
    }
    IEnumerator FadeLightCoroutine()
    {
        float newIntensity = 0.0f;
        float currentIntensity = starLight.intensity;
        float currentTime = 0.0f;
        while (currentTime < intensityIncreaseDuration)
        {
            starLight.intensity = Mathf.Lerp(starLight.intensity, newIntensity, currentTime / intensityIncreaseDuration);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }


    IEnumerator AjdustRangeCoroutine(float adjustmentValue)
    {
        yield return new WaitForSeconds(6.0f);
        float newSize = starLight.range + adjustmentValue;
        float startTime = Time.time;
        float time = 0;
        while (Time.time < startTime + intensityIncreaseDuration)
        {

            starLight.range = Mathf.Lerp(starLight.range, newSize, time / intensityIncreaseDuration);
            time += Time.deltaTime;

            yield return null;
        }
    }

    private void OnEnable()
    {
        DarkStar.AdjustLuminosity += this.AdjustIntensityAndSize;

    }

    void Awake()
    {
        DarkStar.Overcharged += this.FadeLightCoroutineWrapper;
    }

    private void OnDisable()
    {
        DarkStar.AdjustLuminosity -= this.AdjustIntensityAndSize;
    }

    // Use this for initialization
    void Start()
    {
        starLight = GetComponent<Light>();
        intensityIncreaseDuration = 3.0f;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
