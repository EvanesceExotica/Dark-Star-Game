using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionHandler {


    public static Vector2 LimitPosition(Vector2 center, float limit, Vector3 ourPosition)
    {
        //this limits the position of the indicator to inside the spell's radius 
        Vector2 centerOfCircle = (Vector2)center;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 offSet = ((Vector2)mousePosition - centerOfCircle);
		ourPosition= new Vector3(0, 0,0);

        float distance = offSet.magnitude;
        
            if (distance >= limit)
            { //if the indicator has passed the radius, place it back where it should be
                Vector2 direction = offSet / distance;
                ourPosition = centerOfCircle + direction * limit;
            }
            else
            {
                ourPosition = mousePosition;
            }
        return ourPosition;
       
    }
	
}
