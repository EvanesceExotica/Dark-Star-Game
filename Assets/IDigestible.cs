using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public enum DigestibleEntityType
{
    Star,
    EventHorizon,


}

public interface IDigestible  {


    DigestibleEntityType entityType { get;  }
    int illuminationAdjustmentValue { get; }

    void Deconstruct();
	
}
