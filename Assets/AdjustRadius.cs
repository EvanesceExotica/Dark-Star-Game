using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustRadius : MonoBehaviour {

    float defaultRadius;
    float currentRadius;
    float RadiusDuration;

    bool growing;
    CircleCollider2D ourCollider;

    private void Awake()
    {
        ourCollider = GetComponent<CircleCollider2D>();
    }

    void AdjustColliderRadius(float adjustmentValue)
    {
        StartCoroutine(AdjustRadiusCoroutine(adjustmentValue));

    }


    //don't forget to Radius collider
    IEnumerator AdjustRadiusCoroutine(float adjustmentValue)
    {
     //   adjustmentValue *= 0.08f;
        growing = true;
        currentRadius = ourCollider.bounds.extents.x;
     //   ////Debug.Log(currentRadius);
        float elapsedTime = 0;

        float desiredRadius = (currentRadius + adjustmentValue);
     //   ////Debug.Log("Current radius : " + currentRadius + "," + " Desired Radius: " + desiredRadius);

        while (elapsedTime < RadiusDuration)
        {
            ourCollider.radius = Mathf.Lerp(currentRadius, desiredRadius, elapsedTime / RadiusDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        growing = false;


    }


    private void OnEnable()
    {
        DarkStar.AdjustLuminosity += this.AdjustColliderRadius;
    }

    private void OnDisable()
    {
        DarkStar.AdjustLuminosity -= this.AdjustColliderRadius;
    }

    // Use this for initialization
    void Start()
    {
        RadiusDuration = 5.0f;
        defaultRadius = ourCollider.bounds.extents.x;

    }
}
