using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindLocationInSafeZone : MonoBehaviour {

    float radiusOfDarkStar;
    public GameObject darkStar;
    Vector2 darkStarPosition;
    float bufferAmount = 3.0f;
    private void Awake() {
        darkStar = GameObject.Find("Dark Star");
        radiusOfDarkStar = darkStar.GetComponent<CircleCollider2D>().bounds.extents.x;
        darkStarPosition = darkStar.transform.position;


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
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
