using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSizeHandler : MonoBehaviour
{


    HandleStarParticles handleStarParticles;
    Transform starTransform;
    Vector2 defaultScale;
    Vector3 currentScale;
    Vector3 currentParticleSystemScale;
    float scaleDuration;

    public ParticleSystemScalingMode scaleMode;
    public GameStateHandler gameStateHandler;
    bool growing;

    public ParticleSystem starParticleSystem;


    void AdjustScale(float adjustmentValue)
    {
        StartCoroutine(AdjustScaleCoroutine(adjustmentValue));

    }



    //don't forget to scale collider
    IEnumerator AdjustScaleCoroutine(float adjustmentValue)
    {
        adjustmentValue *= 0.08f;
        growing = true;
        currentScale = transform.localScale;
        previousScale = currentScale;
        //currentParticleSystemScale = starParticleSystem.transform.localScale;
        float elapsedTime = 0;

        Vector2 desiredScale = new Vector2(currentScale.x + adjustmentValue, currentScale.y + adjustmentValue);

        //I'm not sure if all the particle systems do this, but the Mierza Berg Particle Systems scale on the x and z axis, so the adjustment has to be added to the x  and z rather than x and y
        List<Vector3> desiredParticleScaleList = new List<Vector3>();
        List<Vector3> currentParticleSystemScaleList = new List<Vector3>();
        foreach (GameObject go in handleStarParticles.particleSystemGameObjectsToScale)
        {
            Vector3 currentParticleSystemScale = go.transform.localScale;
            Vector3 desiredParticleScale = new Vector3(currentParticleSystemScale.x + (2 * adjustmentValue), currentParticleSystemScale.y, currentParticleSystemScale.z + (2 * adjustmentValue));
            desiredParticleScaleList.Add(desiredParticleScale);
           currentParticleSystemScaleList.Add(currentParticleSystemScale) ;
        }
        //Vector3 desiredParticleScale = new Vector3(currentScale.x + adjustmentValue, currentScale.y , currentScale.z + adjustmentValue);
        //


        desiredScale = CheckIfWillPassMinimum(desiredScale);
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(currentScale, desiredScale, elapsedTime / scaleDuration);
            for (int i = 0; i < handleStarParticles.particleSystemGameObjectsToScale.Count; i++)
            {
                //you had trouble with this earlier -- make sure that when using Lerp sometimes, the current value isn't being changed in the loop. You had to set the "currentScale" outside of this loop otherwise it would change and make this scale faster than the sprite's scale above.
                GameObject currentParticleSystemGO= handleStarParticles.particleSystemGameObjectsToScale[i];
                //Debug.Log("This is hte particle system we're scaling " + currentParticleSystemGO.name + " from " + currentParticleSystemGO.transform.localScale + " to " + desiredParticleScaleList[i]);
                Vector3 currentParticleSystemScale = currentParticleSystemScaleList[i];

                currentParticleSystemGO.transform.localScale = Vector3.Lerp(currentParticleSystemScale, desiredParticleScaleList[i], elapsedTime / scaleDuration );
            }
            // var main = starParticleSystem.main;
            //main.scalingMode = scaleMode;
            DarkStar.radius = DarkStar.area.bounds.extents.x;
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        growing = false;
        //TODO: You can change this so that the creature gets a radius update while the star is growing, or just let them die 
        // DarkStar.radius = DarkStar.area.bounds.extents.x;
        darkStarComponent.DarkStarStable();
    }

    Vector3 previousScale;

    void ShrinkToZeroWrapper(){
        StartCoroutine(ShrinkStarToZero());
    }
    IEnumerator ShrinkStarToZero(){
        Debug.Log("Shrinking to zero");
        ///this method will be used specifically during the final moments of the star's life in order to make the explosion look more realistic
        float elapsedTime = 0;
        while(elapsedTime < shrinkDuration){
           transform.localScale = Vector3.Lerp(currentScale, Vector3.zero, elapsedTime/shrinkDuration) ;
            elapsedTime += Time.deltaTime;
           yield return null;

        }
    }
    void ResetToPreviousWrapper(){
        StartCoroutine(ResetToPreviousScale());
    }

    IEnumerator ResetToPreviousScale(){
        Debug.Log("Resetting to previous");
        currentScale = transform.localScale;
        float elapsedTime = 0;
        while(elapsedTime < shrinkDuration){
            transform.localScale = Vector3.Lerp(currentScale, previousScale, elapsedTime/shrinkDuration);
            elapsedTime += Time.deltaTime;
           yield return null;
        }
    }

    Vector2 CheckIfWillPassMinimum(Vector2 desiredScale)
    {
        //TODO: CHange this to a minimum value
        Vector2 ourUpdatedDesiredScale = new Vector2(0, 0);
        if (desiredScale.x > 0)
        {//if our desired scale isn't smaller than zero
            ourUpdatedDesiredScale = desiredScale;
        }
        return ourUpdatedDesiredScale;
    }
    DarkStar darkStarComponent;
    float shrinkDuration;
    void Awake()
    {
        starParticleSystem = GetComponentInChildren<ParticleSystem>();
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        darkStarComponent = gameStateHandler.darkStar.GetComponent<DarkStar>();
        handleStarParticles = GetComponent<HandleStarParticles>();
        DarkStar.Overcharged += this.ShrinkToZeroWrapper;
    }
    private void OnEnable()
    {
        DarkStar.AdjustLuminosity += this.AdjustScale;
    }

    private void OnDisable()
    {
        DarkStar.AdjustLuminosity -= this.AdjustScale;
    }

    // Use this for initialization
    void Start()
    {
        shrinkDuration = 1.0f;
        scaleDuration = 5.0f;
        defaultScale = gameObject.transform.localScale;


    }

    // Update is called once per frame
    void Update()
    {

    }
}
