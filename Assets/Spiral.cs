using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiral : MonoBehaviour {

	public float circleSize;
	public float circleGrowSpeed;
	public float circleSpeed;
	void Start(){
        circleSize = 10.0f;
        circleGrowSpeed = 0.03f;
        circleSpeed = 1.0f;
		StartCoroutine(SpiralAround());
	}
	public IEnumerator SpiralAround()
    {
        circleSize = DarkStar.radius;
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        while (true)
        {
            // circleSpeed = frequency, circleSize = amplitude -- the phase shift is the only thing that's "added" rather than multiplied so to speak
            xPosition = circleSize * Mathf.Sin(Time.time * circleSpeed) ;
            yPosition = circleSize * Mathf.Cos(Time.time * circleSpeed) ;
            //var zPosition += forwardSpeed * Time.deltaTime;
            circleSize += circleGrowSpeed;
            transform.position = new Vector2(xPosition, yPosition);
            yield return null;
        }
	}
}
