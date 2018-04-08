using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FSMState  {


    void Enter(FSM fsm, GameObject gameObject);
    // Update is called once per frame
    void Update(FSM fsm, GameObject gameObject); 

    void Exit(FSM fSM, GameObject gameObject)	;
	
}
