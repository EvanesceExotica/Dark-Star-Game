using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class FadeIntoMap : MonoBehaviour {


    CanvasGroup ourCanvasGroup;
	// Use this for initialization


    bool scrollingUp;
    bool scrollingDown;
    float zoomDistance;
    Camera gameCamera;
    Camera mapCamera;
    GameObject player;
    GameObject darkStar;
    ProCamera2D ourProCamera;

    bool zoomedInToGame;
    float sizeOfMapCamera;// = mapCamera.orthographicSize;
    float defaultSizeOfGameCamera;// = gameCamera.orthographicSize;



void Start()
    {
        player = GameObject.Find("Player");
        darkStar = GameObject.Find("Dark Star");
        mapCamera = GameObject.Find("MinimapCamera").GetComponent<Camera>();
        gameCamera = Camera.main;
        ourProCamera = gameCamera.GetComponent<ProCamera2D>();
         sizeOfMapCamera = mapCamera.orthographicSize;
         defaultSizeOfGameCamera = gameCamera.orthographicSize;
   
        zoomedInToGame = true;
        zoomDistance = sizeOfMapCamera - defaultSizeOfGameCamera;
        ourCanvasGroup = GetComponent<CanvasGroup>();

    }

    public IEnumerator FadeInMap(float speed)
    {
        while (ourCanvasGroup.alpha < 1f)
        {
            ourCanvasGroup.alpha += speed * Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeOutMap(float speed)
    {
        while (ourCanvasGroup.alpha > 0)
        {
            ourCanvasGroup.alpha -= speed * Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ZoomOutToMapView()
    {
       // ProCamera2D.Zoom(sizeOfMapCamera - defaultSizeOfGameCamera, 0, EaseType.)
       
        float time = 0.0f;

        ourProCamera.RemoveCameraTarget(player.transform);
        ourProCamera.AddCameraTarget(darkStar.transform);

        while (gameCamera.orthographicSize < sizeOfMapCamera)
        {
            if(gameCamera.orthographicSize > sizeOfMapCamera - 0.8f)
            {
               // StartCoroutine(FadeInMap(10f));
                break;
            }
            gameCamera.orthographicSize = Mathf.SmoothStep(gameCamera.orthographicSize, sizeOfMapCamera, time);
            time += Time.deltaTime * 0.5f;

            yield return null;
        }
        StartCoroutine(FadeInMap(10.0f));
        zoomedInToGame = false;

    }




    public IEnumerator ZoomOutToMapViewTEST()
    {
        TestMoveBetweenPlayerAndSun();
        StopFollowingPlayer();
        ourProCamera.Zoom(zoomDistance, 1.0f, EaseType.EaseOut);
        yield return new WaitForSeconds(0.5f);    
        StartCoroutine(FadeInMap(10.0f));
        zoomedInToGame = false;
    }

    public IEnumerator ZoomInToGameView()
    {
        yield return StartCoroutine(FadeOutMap(10.0f));

        float time = 0.0f;

      

        while (gameCamera.orthographicSize > defaultSizeOfGameCamera)
        {
            time += Time.deltaTime * 0.5f;


            //Debug.Log(time);

            gameCamera.orthographicSize = Mathf.Lerp(gameCamera.orthographicSize, defaultSizeOfGameCamera, time);
            yield return null;
        }
        zoomedInToGame = true;

    }

    public void ZoomInToGameViewTEST()
    {
        StartCoroutine(FadeOutMap(10f));
        ourProCamera.AddCameraTarget(player.transform, 1, 1, 0.5f);
        ourProCamera.RemoveCameraTarget(darkStar.transform, 0.5f);
        ourProCamera.Zoom(-zoomDistance, 1.0f, EaseType.EaseIn);
        zoomedInToGame = true;

    }

    // Use this for initialization

        void TestMoveBetweenPlayerAndSun()
    {
        ourProCamera.AddCameraTarget(darkStar.transform, 1, 1, 0.5f);

    }

    void StopFollowingPlayer()
    {
        ourProCamera.RemoveCameraTarget(player.transform, 0.5f);
    }

    void StartFollowingPlayer()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float delta = Input.GetAxis("Mouse ScrollWheel");

        if (delta > 0f)
        {
            //ZoomInToGameViewTEST();
        }
        else if (delta < 0f)
        {
           // StartCoroutine(ZoomOutToMapViewTEST());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TestMoveBetweenPlayerAndSun();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StopFollowingPlayer();
        }
    }
}
