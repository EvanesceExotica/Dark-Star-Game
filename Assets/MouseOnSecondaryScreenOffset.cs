using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MouseOnSecondaryScreenOffset : MonoBehaviour {

    public GameObject tearBackground;

    public GameObject ghostMouse;
    Transform ghostMouseChildAffectors;
    List<MonoBehaviour> ghostMouseChildAffectorComponents = new List<MonoBehaviour>();


    public ParticleSystem tearBackgroundParticleSystemBox;

    float tearBoxX;
    float tearBoxY;

    public Camera tearViewCamera;

    [SerializeField]
    Vector3 testPosition;

    [SerializeField]
    Vector3 testPosition2;

	// Use this for initialization
	void Start () {

        ghostMouseChildAffectors = ghostMouse.transform.GetChild(0);
        ghostMouseChildAffectorComponents = ghostMouseChildAffectors.GetComponents<MonoBehaviour>().ToList();

		
	}

    private void OnDrawGizmos()
    {
        DebugExtension.DrawPoint(testPosition, Color.green, 3.0f);
        DebugExtension.DrawPoint(testPosition2, Color.cyan, 4.0f);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void TurnEffectorsOn()
    {
        foreach(MonoBehaviour mb in ghostMouseChildAffectorComponents)
        {
            mb.enabled = true;
        }
    }

    void TurnEffectorsOff()
    {
        foreach(MonoBehaviour mb in ghostMouseChildAffectorComponents)
        {
            mb.enabled = false;

        }
    }

    private void OnMouseOver()
    {
        TrasmitMouseToGhostMouse();
        TurnEffectorsOn();
    }


    private void OnMouseExit()
    {
        TurnEffectorsOff();
    }
    void TrasmitMouseToGhostMouse()
    {
       // Debug.Log("Transmitting");
       Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
           // Debug.Log("First success!");
            var tearSpaceLocalPoint = hit.textureCoord;
            // Debug.Log(tearSpaceLocalPoint);

            Ray tearRay = tearViewCamera.ViewportPointToRay(new Vector3(tearSpaceLocalPoint.x/* * tearViewCamera.pixelWidth*/, tearSpaceLocalPoint.y /* * tearViewCamera.pixelHeight*/));
          //  RaycastHit tearHit;
            Debug.DrawRay(tearRay.origin, tearRay.direction * 30f, Color.blue);
            testPosition2 = tearRay.origin;
            ghostMouse.transform.position = tearRay.origin;

            testPosition = new Vector2(hit.textureCoord.x, hit.textureCoord.y);


            //you don't need this second one because you're not looking for a collider
            //if (Physics.Raycast(tearRay, out tearHit))
            //{
            //    Debug.Log("Second success!");
            //    Debug.Log(tearHit.point);
            //    testPosition = tearHit.point;
            //    //ghostMouse.transform.position = tearHit.point;
            //}
        }
//     tearViewcamera.ScreenToWorldPoint  
    }
}
