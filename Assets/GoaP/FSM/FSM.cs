using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

using System;

public class FSM  {

    private Stack<FSMState> stateStack = new Stack<FSMState>();

//so this delegate is cycling between the moveState, performState, and idleState;
    public delegate void FSMState(FSM fsm, GameObject gameObject);


    public void Update(GameObject gameObject)
    {
        if(stateStack.Peek() != null)
        {
            Profiler.BeginSample("Update invocation");
            stateStack.Peek().Invoke(this, gameObject);
            Profiler.EndSample();
        }
    }

    public void pushState(FSMState state){

        stateStack.Push(state);
    }

    public void popState()
    {
        stateStack.Pop();
    }

}
