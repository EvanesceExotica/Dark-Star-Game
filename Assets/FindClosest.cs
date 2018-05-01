using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindClosest  {

    public static GameObject FindClosestObject(List<GameObject> goList, GameObject ourObject)
    {
        GameObject closestObject = null;
        float minDistance = Mathf.Infinity;
        foreach (GameObject go in goList)
        {
            float distance = Vector2.Distance(go.transform.position, ourObject.transform.position);
            if(distance < minDistance){
                closestObject = go;
                minDistance = distance;
            }
        }
        return closestObject;
    }

    public static Vector3 FindClosestVector(List<Vector3> vectorList, GameObject ourObject){
        Vector3 closest = new Vector3(0, 0, 0);
        float smallestDistance = Mathf.Infinity;
       
        foreach(Vector3 point in vectorList){
            float distance = Vector3.Distance(point, ourObject.transform.position);
            if(distance < smallestDistance){
                smallestDistance = distance;
                closest = point;
            }
        }
        return closest;

    }

    public static GameObject ClosestOfTwo(GameObject a, GameObject b, GameObject source){
        GameObject closest = null;
        float distanceFromA = Vector2.Distance(a.transform.position, source.transform.position);
        float distanceFromB = Vector2.Distance(b.transform.position, source.transform.position);
        if(distanceFromA < distanceFromB){
            closest = a;
        }
        else if(distanceFromB < distanceFromA){
            closest = b;
        }
        return closest;
    }

   
}
