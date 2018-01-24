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
            primeLocation *= Random.Range(3.0f, 4.0f);
        }
 
        return primeLocation;
    }
}
