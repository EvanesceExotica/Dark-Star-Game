using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AtomicAction  {


    public enum GoapResult
    {
        SUCCESS,
        FAILED,
        RUNNING
    }
    public virtual void ResetForPlanning(GoapAgent agent)
    {

    }

    public virtual bool CanExecute(GoapAgent agent)
    {
        return true;
    }

    public virtual GoapResult Start(GoapAgent agent)
    {
        return GoapResult.SUCCESS;

    }

    public virtual GoapResult Update(GoapAgent agent)
    {
        return GoapResult.SUCCESS;
    }

    public virtual void OnFail(GoapAgent agent)
    {

    }
	
}
