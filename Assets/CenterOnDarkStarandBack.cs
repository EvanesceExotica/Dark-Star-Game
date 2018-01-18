using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;

public class CenterOnDarkStarandBack : MonoBehaviour
{
    Camera gameCamera;
    ProCamera2D ourProCamera2D;
    GameObject darkStar;
    GameObject player;
    bool focusedOnDarkStar;

    private void Awake()
    {
        darkStar = GameObject.Find("Dark Star");
        gameCamera = Camera.main;
        ourProCamera2D = gameCamera.GetComponent<ProCamera2D>();

        player = GameObject.Find("Player");
    }
    void CenterOnDarkStar()
    {
        focusedOnDarkStar = true;
        ourProCamera2D.RemoveCameraTarget(player.transform);
        ourProCamera2D.AddCameraTarget(darkStar.transform);
    }
    void CenterOnBothTargets()
    {
        focusedOnDarkStar = true;
        ourProCamera2D.AddCameraTarget(darkStar.transform);

    }
    void ReturnToPlayer()
    {
        ourProCamera2D.RemoveCameraTarget(darkStar.transform);
        ourProCamera2D.AddCameraTarget(player.transform);
        focusedOnDarkStar = false;
    }
    IEnumerator PanBack()
    {

        yield return null;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space) && !focusedOnDarkStar)
        {
            CenterOnDarkStar();
           // CenterOnBothTargets();
            //pan me over to the dark star
        }
        if (Input.GetKeyUp(KeyCode.Space) && focusedOnDarkStar)
        {

            ReturnToPlayer();
        }
    }
}
 