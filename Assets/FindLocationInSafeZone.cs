using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FindLocationInSafeZone {

   public  static Vector2 FindLocationInCircleExclusion(GameObject excludedCircleObject, float bufferAmount)
    {
		float innerHoleRadius = excludedCircleObject.GetComponent<CircleCollider2D>().bounds.extents.x;
		Vector2 innerHolePosition = excludedCircleObject.transform.position;
        Vector2 primeLocation = new Vector2(0, 0);
        primeLocation = UnityEngine.Random.insideUnitCircle.normalized * innerHoleRadius + innerHolePosition;
    
        
        if(Vector2.Distance(primeLocation, innerHolePosition) < innerHoleRadius + bufferAmount)
        {
            primeLocation *= Random.Range(bufferAmount, bufferAmount -  1);
        }
 
        return primeLocation;
    }

    public static Vector2 FindLocationInCircle(GameObject circleObject){
	    float innerHoleRadius = circleObject.GetComponent<CircleCollider2D>().bounds.extents.x;
		Vector2 innerHolePosition = circleObject.transform.position;
        Vector2 primeLocation = Vector2.zero;
        primeLocation = UnityEngine.Random.insideUnitCircle.normalized * innerHoleRadius + innerHolePosition;
        return primeLocation;
    }

    // public static Vector2 FindLocationAroundPoint(Vector3 point, float bufferAmount){
    //     //TODO: MAke sure it spawns within this range
    //     Vector2 primeLocation = Vector2.zero;
    // }
}
