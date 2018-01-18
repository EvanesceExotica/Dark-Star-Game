using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BurstBehaviour : MonoBehaviour
{

    Light ourLight;
    public float timeToFade;

    private void Awake()
    {
        ourLight = GetComponent<Light>();
    }
    public static event Action<Vector2> BurstLightGoesOff;

    void LightBurstActivated(Vector2 atLocation)
    {
        Debug.Log("We're fading now");
        FadeBurstLight();
        if(BurstLightGoesOff != null)
        {
            BurstLightGoesOff(atLocation);
        }
    }

    void FadeBurstLight()
    {
        ScaleObject.AdjustLightIntensity(this, ourLight, 0.5f , 3.0f);
    }
    private void OnEnable()
    {
        LightBurstActivated(transform.position);
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
