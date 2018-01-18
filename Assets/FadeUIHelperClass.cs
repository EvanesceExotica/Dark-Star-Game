using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class FadeUIHelperClass  {

    
  public static void FadeInWrapper(this MonoBehaviour behaviour, float speed, CanvasGroup ourCanvasGroup)
    {
        behaviour.StartCoroutine(FadeInUIElement(speed, ourCanvasGroup));
    }

    public static void FadeOutWrapper(this MonoBehaviour behaviour, float speed, CanvasGroup ourCanvasGroup)
    {
        behaviour.StartCoroutine(FadeOutUIElement(speed, ourCanvasGroup));
    }

    public static IEnumerator FadeInUIElement(float speed, CanvasGroup ourCanvasGroup)
    {
        while (ourCanvasGroup.alpha < 1.0f)
        {
            ourCanvasGroup.alpha += speed * Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator FadeOutUIElement(float speed, CanvasGroup ourCanvasGroup)
    {
        while (ourCanvasGroup.alpha > 0.0f)
        {
            ourCanvasGroup.alpha -= speed * Time.deltaTime;
            yield return null;
        }
    }



}
