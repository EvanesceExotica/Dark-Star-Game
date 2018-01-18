using UnityEngine;
using System.Collections;

public abstract class ConditionResolver : MonoBehaviour
{
    private bool resolved;
    private bool conditionMet;

    public ConditionResolver()
    {
       
    }

    public void Reset()
    {
        this.resolved = false;
        this.conditionMet = false;
        
    }

    public bool IsMet(GoapAgent agent)
    {
        if (!this.resolved)
        {
            this.conditionMet = Resolve(agent);
        }
        return this.conditionMet;
    }

    public abstract bool Resolve(GoapAgent agent);
   

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
