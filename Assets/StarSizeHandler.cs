using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSizeHandler : MonoBehaviour {



    Transform starTransform;
    Vector2 defaultScale;
    Vector3 currentScale;
    float scaleDuration;
public GameStateHandler gameStateHandler;
    bool growing;

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
        float elapsedTime = 0;

        Vector2 desiredScale = new Vector2(currentScale.x + adjustmentValue, currentScale.y + adjustmentValue);

        desiredScale = CheckIfWillPassMinimum(desiredScale);
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(currentScale, desiredScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        growing = false;
        //TODO: You can change this so that the creature gets a radius update while the star is growing, or just let them die 
        DarkStar.radius = DarkStar.area.bounds.extents.x;
        darkStarComponent.DarkStarStable();
    }

    Vector2 CheckIfWillPassMinimum(Vector2 desiredScale)
    {
        //TODO: CHange this to a minimum value
        Vector2 ourUpdatedDesiredScale = new Vector2(0, 0);
        if(desiredScale.x > 0)
        {//if our desired scale isn't smaller than zero
            ourUpdatedDesiredScale = desiredScale; 
        }
        return ourUpdatedDesiredScale;
    }
DarkStar darkStarComponent;
void Awake(){
    gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
    darkStarComponent = gameStateHandler.darkStar.GetComponent<DarkStar>();
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
    void Start () {
        scaleDuration = 5.0f;
        defaultScale = gameObject.transform.localScale;
        
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
