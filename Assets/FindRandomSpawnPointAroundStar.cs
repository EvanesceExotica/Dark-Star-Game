using UnityEngine;
using System.Collections;

public class FindRandomSpawnPointAroundStar : MonoBehaviour
{

    static float radiusOfDarkStar;
   static Vector2 darkStarPosition;

    static GameStateHandler gameStateHandler;
    private void Awake()
    {
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        darkStarPosition = GameObject.Find("Dark  Star").transform.position;
        radiusOfDarkStar = GameObject.Find("Dark Star").GetComponent<CircleCollider2D>().bounds.extents.x;
    }

    public static Vector2 FindLocationAroundStar(float bufferAmount, float radius, Vector2 position)
    {

        Vector2 primeLocation = new Vector2(0, 0);

        Debug.Log("Radius : " + radius);
        Debug.Log("Dark Star Position " + position);
        //darkStarPosition = gameStateHandler.darkStar.transform.position;
        //radiusOfDarkStar = gameStateHandler.darkStar.GetComponent<CircleCollider2D>().bounds.extents.x;
        primeLocation = UnityEngine.Random.insideUnitCircle.normalized * radius + position;


        if (Vector2.Distance(primeLocation, position) < radius + bufferAmount)
        {
            primeLocation *= Random.Range(3.0f, 4.0f);
        }

        return primeLocation;
    }
}