using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMouseFollow : MonoBehaviour {

    float minX;
    float maxX;

    float minY;
    float maxY;


    float tearBoxX;
    float tearBoxY;

    ParticleSystem tearBackgroundParticleSystemBox;

    public void LimitPositionInsideBounds()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }


    void DetermineMinAndMax()
    {
         
        if (tearBackgroundParticleSystemBox.shape.shapeType == ParticleSystemShapeType.Box)
        {
            tearBoxX = tearBackgroundParticleSystemBox.shape.box.x;
            tearBoxY = tearBackgroundParticleSystemBox.shape.box.y;
        }

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        LimitPositionInsideBounds(); 
	}
}
