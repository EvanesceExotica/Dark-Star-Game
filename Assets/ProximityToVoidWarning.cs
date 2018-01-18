using UnityEngine;
using System.Collections;

public class ProximityToVoidWarning : MonoBehaviour
{
    float timeToFadeIn;
    float timeToFadeOut;
    SpriteRenderer spriteRenderer;
    Color spriteRendererColor = Color.white;
    Color transparentSpriteRendererColor;
    private void Awake()
    {
        timeToFadeIn = 1.0f;
        timeToFadeOut = 0.5f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayerTriggerHandler.HoveringOverVoid += this.FadeIn;
        PlayerTriggerHandler.NoLongerHoveringOverVoid += this.FadeOut;
        GameStateHandler.DarkPhaseStarted += this.DisableMyself;

    }

    void DisableMyself()
    {
       // Debug.Log("PROXIMITYTOVOID IS DISABLING ITSELF");
        //Debug.Log("This ain't really happening");
        StartCoroutine(StartDisabling());
        //TODO: Also turn off pulse! 
    }

    public IEnumerator StartDisabling()
    {
        //Debug.Log("Did this happen?");
        PlayerTriggerHandler.HoveringOverVoid -= this.FadeIn;
        PlayerTriggerHandler.NoLongerHoveringOverVoid -= this.FadeOut;
        yield return StartCoroutine(FadeSpriteOut());
        this.enabled = false;
    }


    void FadeIn()
    {
        //Debug.Log("Fading sprite in!");
        StartCoroutine(FadeSpriteIn());
    }

    void FadeOut()
    {
        //Debug.Log("Fading sprite out!");
        StartCoroutine(FadeSpriteOut());
    }

    IEnumerator FadeSpriteIn()
    {
        float fade = 0f;
        float startTime;
            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / timeToFadeIn);
                spriteRendererColor.a = fade;
                spriteRenderer.color = spriteRendererColor;
                yield return null;
            }
            //Make sure it's set to exactly 1f
            fade = 1f;
            spriteRendererColor.a = fade;
            spriteRenderer.color = spriteRendererColor;
    }

    IEnumerator FadeSpriteToColor(Color color)
    {
        float fade = 0f;
        float startTime;
        startTime = Time.time;
        while (fade < 1f)
        {
            fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / timeToFadeIn);
            spriteRendererColor.a = fade;
            spriteRenderer.color = spriteRendererColor;
            yield return null;
        }
        //Make sure it's set to exactly 1f
        fade = 1f;
        spriteRendererColor.a = fade;
        spriteRenderer.color = spriteRendererColor;
    }

    IEnumerator FadeSpriteOut()
    {

       
        float fade = 1f;
        float startTime;
            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / timeToFadeOut);
                spriteRendererColor.a = fade;
                spriteRenderer.color = spriteRendererColor;
                yield return null;
            }
            //Make sure it's set to exactly 1f
            fade = 0f;
            spriteRendererColor.a = fade;
            spriteRenderer.color = spriteRendererColor;
    }

  
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
