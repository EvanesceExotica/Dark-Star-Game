using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchSoul : MonoBehaviour
{
	bool launching;
    bool priming  = false;
    float minimumHoldDuration = 1.0f;
    Vector2  mouseStartPosition;
    float maxPullDistance;
    public float elasticity;
	 LineRenderer slingshotLineRenderer;
    public int slingshotCounter = 0;
    public int maxConcurrentSlingshots = 3;
    public bool cantSlingshot = false;
    bool stillHeld = false;
    float holdStartTime;
	Rigidbody2D rb;

    public IEnumerator PrimeSlingshot()
    {

        //Debug.Log("Priming!");
        priming = true;
        mouseStartPosition = Input.mousePosition;
        float distance = 0;
        Vector2 direction = new Vector2(0, 0);


        slingshotLineRenderer.enabled = true;
        FreezeTime.SlowdownTime(0.25f);
        while (true)
        {

            if (Input.GetMouseButtonUp(0))
            {
                break;
            }
            if (Input.GetMouseButtonDown(1))
            {
                //right click to cancel
                stillHeld = true;
                slingshotLineRenderer.enabled = false;
                yield break;
            }
            slingshotLineRenderer.SetPosition(0, transform.position);
            slingshotLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
            yield return null;
        }
        FreezeTime.StartTimeAgain();
        slingshotLineRenderer.enabled = false;

        slingshotCounter++;

        Vector2 mousePos = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        distance = Vector2.Distance(gameObject.transform.position, mousePositionWorld);
        direction = (Vector2)((Vector2)transform.position - mousePositionWorld);


        float velocity = distance * Mathf.Sqrt(elasticity / rb.mass);
        rb.velocity = direction.normalized * velocity;

        //Debug.Log(pReference.rb.velocity);
        priming = false;
        launching = true;
        //   StartCoroutine(PlotPath());
    }
    void Update(){
		if( Input.GetMouseButton(1) && !cantSlingshot && !stillHeld)
        {
			holdStartTime = Time.time;
            StartCoroutine(PrimeSlingshot());

        }
	}
    // Use this for initialization

}
