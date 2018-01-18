using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRaycast : MonoBehaviour {

    GameObject player;
    float radius;
    List<Vector2> positions;

    LineRenderer mapLineRenderer;

    Vector2 LimitPosition(Vector2 center, float limit)
    {
        //this limits the position of the indicator to inside the spell's radius 
        Vector2 centerOfCircle = (Vector2)center;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = (Vector2)((Vector2)transform.position - centerOfCircle);
        Vector2 ourPosition = mapLineRenderer.GetPosition(0);

        Vector2 offSet = ((Vector2)mousePosition - centerOfCircle);

        float distance = offSet.magnitude;

        if (limit < distance)
        { //if the indicator has passed the radius, place it back where it should be
            Vector2 direction = offSet / distance;
            ourPosition = centerOfCircle + direction * radius;
        }
        else if (distance < limit)
        { //if the distance is less than the inner radius, place it back where it should be
            Vector2 direction = offSet / distance;
            ourPosition = centerOfCircle + direction * limit;

        }

        else
        {
            ourPosition = mousePosition;
        }
        return ourPosition;

    }

    void Start () {
        mapLineRenderer = gameObject.GetComponent<LineRenderer>();
            
        player = GameObject.Find("Player");
        radius = GameObject.Find("Void-OuterCircle").GetComponent<CircleCollider2D>().bounds.extents.x;
        mapLineRenderer.SetPosition(0, player.transform.position);
      //  mapLineRenderer.SetPosition(1, LimitPosition(player.transform.position, radius));


    }

    void Update () {

        mapLineRenderer.SetPosition(0, player.transform.position);
        mapLineRenderer.SetPosition(1, LimitPosition(player.transform.position, radius));

    }
}
