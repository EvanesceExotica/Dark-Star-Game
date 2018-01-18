using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExitInstantiator : MonoBehaviour {

    float radius;
    int difficulty;
    Vector2 stageCenter;

    float systemRadius;

    float darkStarRadius;
    Vector2 darkStarPosition;

    GameObject darkStar;
    float buffer;

   public Vector2 keyLocation;

    public GameObject keyPrefab;

    bool KeyGenerated;

    public static event Action HideKey;


    enum ExitLocationType
    {
        Space,
        Planet,
        Wreck,
        Enemy 
    }

    ExitLocationType defaultLocationType;
    
    void BeginHidingOfKey()
    {
        if(HideKey != null)
        {
            HideKey(); 
        }
    }

    private void Awake()
    {
        GameStateHandler.DarkPhaseStarted+= this.GenerateKey;
    }
    // Use nthis for initialization
	// Use this for initialization
	void Start () {
        buffer = 0.5f;
        systemRadius = GameObject.Find("InnerCircle").GetComponent<CircleCollider2D>().radius;
        darkStar = GameObject.Find("Dark Star");
        darkStarRadius = darkStar.GetComponent<CircleCollider2D>().radius;
        darkStarPosition = darkStar.transform.position;

        defaultLocationType = ExitLocationType.Space;



        //if(defaultLocationType == ExitLocationType.Space)
        //{
        //    HideKey = GenerateKey; 
        //}

        //BeginHidingOfKey();

    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(keyLocation);
    }

   public void GenerateKey()
    {
        if (!KeyGenerated)
        {
            KeyGenerated = true;
            bool outsideStar = false;
            Vector2 location = new Vector2(0, 0);
            while (outsideStar == false)
            {
                location = UnityEngine.Random.insideUnitCircle * systemRadius + darkStarPosition;
                if (Vector2.Distance(location, darkStarPosition) > (darkStarRadius + buffer))
                {

                    outsideStar = true;
                }
            }

            keyLocation = location;
            GameObject instantiatedKey = Instantiate(keyPrefab);
            instantiatedKey.transform.position = keyLocation;
        }
        BeginHidingOfKey();



    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.K))
        {
            GenerateKey();
        }
	}
}
