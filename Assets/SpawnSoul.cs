using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSoul : MonoBehaviour {

    public SoulBehavior soulPrefab;
    Vector2 darkStarPosition;
    public GameObject darkStar;
    float bufferAmount = 3.0f;

    float radiusOfDarkStar;

    private void Awake() {
        darkStar = GameObject.Find("Dark Star");
        radiusOfDarkStar = darkStar.GetComponent<CircleCollider2D>().bounds.extents.x;
        darkStarPosition = darkStar.transform.position;


    }

    public void SpawnsoulAroundDarkStar()
    {
        SoulBehavior soul = soulPrefab.GetPooledInstance<SoulBehavior>();

        soul.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(darkStar, 3.0f);/* gameObject.transform.position; /////FindLocationAroundStar();*/
    }

    Vector2 FindLocationAroundStar()
    {

        Vector2 primeLocation = new Vector2(0, 0);
        primeLocation = UnityEngine.Random.insideUnitCircle.normalized * radiusOfDarkStar + darkStarPosition;
    
        
        if(Vector2.Distance(primeLocation, darkStarPosition) < radiusOfDarkStar + bufferAmount)
        {
            primeLocation *= Random.Range(3.0f, 4.0f);
        }
 
        return primeLocation;
    }
}
