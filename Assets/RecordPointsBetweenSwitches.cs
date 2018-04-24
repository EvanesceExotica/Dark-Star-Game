using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPointsBetweenSwitches : MonoBehaviour
{

    public GameObject currentSwitch;
    public bool inSwitchTrigger;

    public bool recording;

    bool hasTouchedTwoSwitches;

    List<GameObject> switchesTouched = new List<GameObject>();

    bool cancelled;

    List<Vector3> points = new List<Vector3>();
    void Awake()
    {
        Switch.AnythingEnteredSwitch += AddSwitchWeTouched;
        Switch.AnythingExitedSwitch += SwitchExited;

    }
    public void Cancel(){
        cancelled = true;
        Reset();
    }

    void Reset(){
        switchesTouched.Clear();
        points.Clear();
        hasTouchedTwoSwitches = false;
        recording = false;

    }
    public void AddSwitchWeTouched(GameObject objectThatEnteredSwitch, GameObject switchTouched)
    {

        if (switchTouched.GetComponent<Switch>().GetType() != typeof(Core))
        {
            //we don't want the core of a planet to count here
            if (objectThatEnteredSwitch == this.gameObject)
            {
                inSwitchTrigger = true;
                currentSwitch = switchTouched;
                StartCoroutine(CheckIfCloseEnough(switchTouched));

            }
        }
    }
    IEnumerator RecordPoints()
    {
        recording = true;
        while (!hasTouchedTwoSwitches && !cancelled)
        {
            //if we've only touched one switch so far
            Debug.Log("Adding points");
            points.Add(transform.position);
            yield return null;
            //yield return new WaitForSeconds(0.5f);
        }
        if (hasTouchedTwoSwitches)
        {
            Switch ourSwitch = switchesTouched[0].GetComponent<Switch>();
            //change this number back for duration
            ourSwitch.AddTemporarySwitchConnectionAndSubscribe(switchesTouched[1], ourSwitch.CreateNewSwitchConnection(), points, 150.0f);
        }
    }
    public void SwitchExited(GameObject switchWeWereIn, GameObject objectThatExitedSwitch)
    {
        if (objectThatExitedSwitch == this.gameObject && switchWeWereIn == currentSwitch)
        {
            //We're no longer moving through a switch
            currentSwitch = null;
            inSwitchTrigger = false;
        }
    }
    bool PassedCloseEnoughToSwitchCenter(GameObject touchedSwitch)
    {
        //TODO: This is the distance when the switch is entered. 
        bool passedCloseEnough = false;
        if (Vector2.Distance(this.gameObject.transform.position, touchedSwitch.transform.position) <= 1.5f)
        {
            //if the GO is in the switch and passes close enough to its center, then return true 
            //takes care of objects that enter the switch but just graze it 
            passedCloseEnough = true;
        }
        return passedCloseEnough;

    }
    public IEnumerator CheckIfCloseEnough(GameObject touchedSwitch)
    {
        while (inSwitchTrigger)
        {
            //while we are currently passing through a switch
            if (PassedCloseEnoughToSwitchCenter(touchedSwitch))
            {
                //if at any point we pass close enough to its center
                if (!switchesTouched.Contains(touchedSwitch))
                {
                    //if this isn't a switch we've already touched
                    switchesTouched.Add(touchedSwitch);
                    if (!recording)
                    {
                        //if we're not already recording points (meaning if this is the first switch we've touched close to the center)
                        StartCoroutine(RecordPoints());
                        break;
                    }
                }
            }
            yield return null;
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
