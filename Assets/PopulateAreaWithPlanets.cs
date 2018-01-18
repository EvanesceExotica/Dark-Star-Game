using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateAreaWithPlanets : MonoBehaviour {

    float systemRadius;

    float darkStarRadius;
    Vector2 darkStarPosition;

    GameObject darkStar;
    float buffer;


    public GameObject planetPrefab;

    int maxRadius;
    int minRadius; 

    List<GameObject> planetList = new List<GameObject>();
    Dictionary<GameObject, Vector2> planets = new Dictionary<GameObject, Vector2>();

    int numberOfPlanets;

	// Use this for initialization
	void Start () {

        buffer = 5.0f;
        numberOfPlanets = 5;
        minRadius = 2;
        maxRadius = 5;

        systemRadius = GameObject.Find("InnerCircle").GetComponent<CircleCollider2D>().radius;
        darkStar = GameObject.Find("Dark Star");
        darkStarRadius = darkStar.GetComponent<CircleCollider2D>().radius;
        darkStarPosition = darkStar.transform.position;

        for (int i = 0; i < numberOfPlanets; i++)
        {
            InstantiateRandomPlanets();
        }

    }



    void InstantiateRandomPlanets()
    {
        float radius = Random.Range(minRadius, maxRadius);
        Vector2 location = Random.insideUnitCircle * systemRadius;

        bool outsideStar = false;

        while (outsideStar == false)
        {
            location = UnityEngine.Random.insideUnitCircle * systemRadius + darkStarPosition;
            if (Vector2.Distance(location, darkStarPosition) > (darkStarRadius + radius + buffer))
            {

                outsideStar = true;
            }
        }

        GameObject newPlanet = Instantiate(planetPrefab);
        newPlanet.transform.position = location;
        CircleCollider2D radialCollider = newPlanet.AddComponent<CircleCollider2D>();
        radialCollider.radius = radius;
        planets.Add(newPlanet, location);


       
    }
	
	// Update is called once per frame
	
}
