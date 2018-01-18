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
            }
        }
        return closestObject;
    }

   
}
