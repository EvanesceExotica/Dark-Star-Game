using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowPulse : MonoBehaviour {

    CircleCollider2D ourPulseCircle;
    float maxRadius;
    float minRadius;
    float growTime;
    float growDuration;
    float growStartTime;

	// Use this for initialization
	void Awake () {

        ourPulseCircle = GetComponent<CircleCollider2D>();
        minRadius = GameObject.Find("Dark Star").GetComponent<CircleCollider2D>().radius;
        maxRadius = GameObject.Find("InnerCircle").GetComponent<CircleCollider2D>().radius;
       // ourPulseCircle.radius = minRadius;
        //StartCoroutine(GrowCollider2D(2.0f));
	}

    private void OnEnable()
    {
        growTime = 10.0f;

        ourPulseCircle.radius = minRadius;
        StartCoroutine(GrowCollider2D(growTime));
    }

    IEnumerator GrowCollider2D(float time)
    {
        float originalRadius = ourPulseCircle.radius;

        float currentTime = 0.0f;
        do
        {
            ourPulseCircle.radius = Mathf.Lerp(ourPulseCircle.radius, maxRadius, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        while (currentTime <= time);
        

        //while(ourPulseCircle.radius < maxRadius)
        //{

        //    ourPulseCircle.radius += Time.deltaTime * growTime;
        //    yield return null;
        //}
    }

    private void OnDisable()
    {
        ourPulseCircle.radius = minRadius;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
