using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScaleObject
{
    public static void AdjustScale(MonoBehaviour ourMonoBehaviour, GameObject objectToBeScaled, float adjustmentValue, float buffer, float scaleDuration, bool canBeInterrupted)

    {
        ourMonoBehaviour.StartCoroutine(AdjustScaleCoroutine(objectToBeScaled, adjustmentValue, buffer, scaleDuration, canBeInterrupted));
    }
    public static IEnumerator AdjustScaleCoroutine(GameObject objectToBeScaled, float adjustmentValue, float buffer, float scaleDuration, bool canBeInterrupted)
    {
        adjustmentValue *= buffer;
        Vector2 currentScale = objectToBeScaled.transform.localScale;
        float elapsedTime = 0;

        Vector2 desiredScale = new Vector2(currentScale.x + adjustmentValue, currentScale.y + adjustmentValue);
        //   Debug.Log(transform.localScale + "   " + desiredScale);
        while (elapsedTime < scaleDuration)
        {
            objectToBeScaled.transform.localScale = Vector3.Lerp(currentScale, desiredScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


    }

    public static void AdjustLightIntensity(MonoBehaviour ourMonoBehaviour, Light lightToBeAdjusted, float adjustmentValue, float duration)
    {
        ourMonoBehaviour.StartCoroutine(DimLightOverTime(lightToBeAdjusted, adjustmentValue, duration));
    }



    public static IEnumerator DimLightOverTime(Light lightToBeAdjusted, float adjustmentValue, float duration)
    {
        Debug.Log(lightToBeAdjusted.gameObject.name + " point light is being dimmed");
        while (lightToBeAdjusted.intensity > 0)
        {
            lightToBeAdjusted.intensity -= Time.deltaTime / 1.0f;
            yield return null;
        }
    }
}
