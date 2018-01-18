using UnityEngine;
using System.Collections;

public class FadePointLightOnPS : MonoBehaviour
{

    Light ourLight;
    ParticleSystem ourSystem;
    public float fadeTime;

    private void Awake()
    {
        ourLight = GetComponentInChildren<Light>();
        ourSystem = GetComponent<ParticleSystem>();
    }
    // Use this for initialization
    void Start()
    {

    }


    void FadePointLight()
    {
        ScaleObject.DimLightOverTime(ourLight, 0, 2);
    }
    // Update is called once per frame
    void Update()
    {
        if (!ourSystem.isPlaying)
        {

            Debug.Log("Fading our point ourLight");
            FadePointLight();
        }
    }
}
