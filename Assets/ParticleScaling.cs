using UnityEngine;
using System.Collections;

// add this to a particle system which has a parent game object, to see how each scaling mode works
public class ParticleScaling : MonoBehaviour
{
    private ParticleSystem ps;
    public float sliderValue = 1.0F;
    public float parentSliderValue = 1.0F;
    public ParticleSystemScalingMode scaleMode;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // void Update()
    // {
    //     // ps.transform.localScale = new Vector3(sliderValue, sliderValue, sliderValue);
    //     // if (ps.transform.parent != null)
    //     //     ps.transform.parent.localScale = new Vector3(parentSliderValue, parentSliderValue, parentSliderValue);

    //     // var main = ps.main;
    //     // main.scalingMode = scaleMode;
    // }

   
}