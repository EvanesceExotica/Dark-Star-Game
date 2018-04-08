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

//this is invoking the delegate at the top of the stack which would change depending on if it were idleState, PerformState or moveToState; Invoke is a delegate method that calls the delegate, so it's basically saying do this method depending on which state is on top 
            stateStack.Peek().Invoke(this, gameObject);
            
            Profiler.EndSample();
        }
    }

    public void pushState(FSMState state){
        //here we're putting a new state on top
        stateStack.Push(state);
    }

    public void popState()
    {

        //here we're taking a state off of the top
        stateStack.Pop();
    }

}
