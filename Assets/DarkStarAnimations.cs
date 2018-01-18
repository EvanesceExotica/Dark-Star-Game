using UnityEngine;
using System.Collections;

public class DarkStarAnimations : MonoBehaviour
{
    _2dxFX_Blood starRejectionAnimation;
    float maxBloodDistortionValue;
    float currentBloodDistortionValue;

    private void Awake()
    {
        starRejectionAnimation = GetComponent<_2dxFX_Blood>();
        maxBloodDistortionValue = 0.207f;
        currentBloodDistortionValue = 0.0f;
    }
    public void AnimateRejectionWrapper()
    {
        StartCoroutine(AnimateRejection());
    }

    public IEnumerator AnimateRejection()
    {
        while(currentBloodDistortionValue < maxBloodDistortionValue)
        {
            currentBloodDistortionValue += 0.075f;
            starRejectionAnimation.TurnToBlood = currentBloodDistortionValue;
            yield return null;
        }

        while(currentBloodDistortionValue >= 0)
        {
            currentBloodDistortionValue -= 0.075f;

           
            starRejectionAnimation.TurnToBlood = currentBloodDistortionValue;
            if(starRejectionAnimation.TurnToBlood < 0)
            {
                starRejectionAnimation.TurnToBlood = 0;
                break;
            }
            yield return null;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Testing distortion");
            StartCoroutine(AnimateRejection());
        }
    }
}
