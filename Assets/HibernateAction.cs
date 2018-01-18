using UnityEngine;
using System.Collections;

public class HibernateAction : GoapAction
{
    public HibernateAction()
    {
        AddPrecondition(new Condition("charge", true));
        AddEffect(new Condition("charge", false));
        AddEffect(new Condition("sleep", true));
        AddEffect(new Condition("reproduce", false));
        cost = 400f;
    }


  
    bool rested = false;



    public override void reset()
    {
        rested = false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //the blue dwarf must find a mate here 

        hasVectorTarget = false;


        target = GameObject.Find("Dark Star");

        if (target != null)
        {
            return true;
        }
        else
        {

            return false;
        }
    }


    void FindNewArea()
    {

    }





    public override bool perform(GameObject agent)
    {



        return true;

    }

    public override bool requiresInRange()
    {

        return false;
    }

    public override bool isDone()
    {
        return rested;

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
