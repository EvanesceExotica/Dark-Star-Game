using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMovement : PlayerMovement  {

    TargetJoint2D targetJoint;
   public bool pointingTowardNearestPlanet;

    public override void Awake()
    {
        TheVoid.PlayerEnteredVoid += IncraseDrag;
        base.Awake();
        pReference = gameObject.GetComponent<PlayerReferences>();
        snapSpeed = 5.0f;
       // moveSpeed = pReference.speed;
    }

    private void OnEnable()
    {
        //make sure this works
        //we're doing this to ensure the player is being rotated back to their normal rotation after they're shot out of or leave a planet
      //  StartCoroutine(AdjustPlayerUpright());
    }

    public IEnumerator RotateFeetTowardPlanet(RaycastHit2D rcast)
    {
        //THIS IS NOT DISABLING WHEN THE SCRIPT IS BEING DISABLED.

        //Note, this stuff is still outside of the while loop, but the while loop is still going, so it won't be triggered, but stuff in the while loop will *I Think*
       
        //This method rotates the feet of the player toward the planet when in range of it, but it might be able to be done in an animation later
        float startTime = Time.time;
        float duration = 4.0f;
        //  //Debug.Log("Rcast" + rcast + " " + rcast.collider);
        // //Debug.Log("Preference" + " " + pReference);
        pointingTowardNearestPlanet = true;
        if (rcast.collider.gameObject == pReference.locationHandler.closestPlanet)
        {

            while (Time.time < startTime + duration)
            {
                if (pReference.locationHandler.onPlanet)
                {
                    break;
                }
                Quaternion newTargetRotation = Quaternion.FromToRotation(Vector3.up, rcast.normal);
                Quaternion newFinalRotation = Quaternion.RotateTowards(transform.rotation, newTargetRotation, 30f * (Time.time - startTime) / duration);
                transform.rotation = Quaternion.Euler(0, 0, newFinalRotation.eulerAngles.z);
                yield return null;
            }
        }
       // pointingTowardNearestPlanet = true;

    }

    public IEnumerator RotateFeetBackToNormal(RaycastHit2D rcast)
    {
        yield return null;
    }


    public IEnumerator AdjustPlayerUpright()
    {
        Debug.Log("We're adjusting back to normal");
        //this method should rotate the player to their normal rotation
        //Play animation here
        float startTime = Time.time;
        float duration = 4.0f; //this should be the time it takes to play the animation 
        while(Time.time < startTime + duration)
        {

            Quaternion newTargetRotation = Quaternion.identity;
            Quaternion newFinalRotation = Quaternion.RotateTowards(transform.rotation, newTargetRotation, 15f * ((Time.time - startTime) / duration));
            transform.rotation = Quaternion.Euler(0, 0, newFinalRotation.eulerAngles.z);
            yield return null;
        }
        pointingTowardNearestPlanet = false;
    }


    
        

    void Update()
    {
        //Debug.Log("Our speed is: " + pReference.speed); 
        if (!this.enabled)
        {
            return;
        }

        if (moving && !incapacitated)
        {
           // //Debug.Log("We pressed a move key. THIS TRIGGERED.");
            pReference.rb.AddForce(movement * pReference.speed);
         //   Debug.Log("Force is " + movement + " * " + pReference.speed);
        }

  



        
        if (/*pReference.locationHandler.inOrbit && !pReference.locationHandler.onPlanet*/ pReference.locationHandler.closeToPlanet && pReference.locationHandler.closestPlanet != null)
        {
            Vector3 direction = pReference.locationHandler.closestPlanet.transform.position - transform.position;
            direction.Normalize();
            RaycastHit2D ourRayCastHit = Physics2D.Raycast(transform.position, direction, 50.0f, pReference.locationHandler.whatIsPlanet);
            Debug.DrawRay(transform.position, direction * 10f, Color.magenta);
            //if (pReference.starBash.outOfPlanet && !pReference.starBash.nesting && !pReference.starBash.bashing)
            //{
            //    pReference.locationHandler.closestPlanet.GetComponentInChildren<PointEffector2D>().enabled = true;
            //}
            // pReference.locationHandler.planetMove.falling = true;
            if (!pointingTowardNearestPlanet)
            {
                StartCoroutine(RotateFeetTowardPlanet(ourRayCastHit));
            }
        }
        if (!pReference.locationHandler.closeToPlanet && pointingTowardNearestPlanet)
        {
            //this should turn the player back to their normal orientation.sd
            StartCoroutine(AdjustPlayerUpright());
        }

    }
   
}
