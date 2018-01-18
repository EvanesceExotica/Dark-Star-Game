using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPullable  {

    void BeginPull(Transform target);

    void CancelPull(); 
    

}
