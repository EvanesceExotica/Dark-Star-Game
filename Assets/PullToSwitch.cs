using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PullToSwitch : MonoBehaviour
{
    PlayerMovement movement;
    public GameObject switchObject;
    public bool inRangeOfSwitch;
    public List<GameObject> switchesWereInRangeOf;
    PlayerReferences pReference;
   // public bool movingTowardSwitchCenter;

    private void Awake()
    {
        pReference = GetComponentInParent<PlayerReferences>();
        movement = pReference.playerMovement;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        //GameObject hitObject = hit.gameObject;
        //if(hitObject.GetComponent<Switch>()!= null)
        //{
        //    switchObject = hitObject;
        //    inRangeOfSwitch = true; 
        //}

    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        //GameObject hitObject = hit.gameObject;
        //if(hit.GetComponent<Switch>() != null)
        //{
        //    switchObject = null;
        //    inRangeOfSwitch = false;
        //}
    }

    void PullMeTowardSwitch(Vector2 switchPosition, GameObject ourSwitch)
    {
        //Debug.Log("We're attaching to switch");
        pReference.rb.velocity = Vector2.zero;
//        pReference.rb.angularVelocity = 0;
        movement.MoveToObject(switchPosition);
    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inRangeOfSwitch)
        {
            //Debug.Log(switchObject.name);
        }
        if(inRangeOfSwitch && Input.GetKeyDown(KeyCode.M))
        {
            PullMeTowardSwitch(switchObject.transform.position, switchObject);
           // movingTowardSwitchCenter = true;
        }

    }
}
