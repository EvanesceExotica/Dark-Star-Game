using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class ConnectionHolder : MonoBehaviour{

    SwitchConnection connectionPrefab;
    
    public List<GameObject> connectionGOs = new List<GameObject>();
    public List<SwitchConnection> switchConnectionList = new List<SwitchConnection>();

    public SwitchConnection CreateNewSwitchConnection()
    {
        SwitchConnection newSwitchConnection = connectionPrefab.GetPooledInstance<SwitchConnection>();
        return newSwitchConnection;
    }
//     public void AddSwitchConnectionAndSubscribe(GameObject firstSwitch, GameObject secondSwitch, SwitchConnection newSwitchConnection)
//     {
//         //SwitchConnection newSwitchConnection = connectionPrefab.GetPooledInstance<SwitchConnection>();
//         //if(newSwitchConnection.switchB.gameObject != this.gameObject)
//         switchConnectionList.Add(newSwitchConnection);
//         newSwitchConnection.TransferingPower += this.SomethingPoweringMeUp;
//         newSwitchConnection.NotTransferingPower += this.SomethingStoppedPoweringMeUp;
//         if (newSwitchConnection.switchAGO != otherSwitch)
//         {
//             //if our connection doesn't already exist and we're the "anchor" switch in this connection
//             newSwitchConnection.MakeConnection(this.gameObject, otherSwitch);
//         }
//     }
//    public void AddTemporarySwitchConnectionAndSubscribe(GameObject otherSwitch, SwitchConnection newSwitchConnection, List<Vector3> plottedPath, float duration)
//     {
//         //SwitchConnection newSwitchConnection = connectionPrefab.GetPooledInstance<SwitchConnection>();
//         switchConnectionList.Add(newSwitchConnection);
//         newSwitchConnection.TransferingPower += this.SomethingPoweringMeUp;
//         newSwitchConnection.NotTransferingPower += this.SomethingStoppedPoweringMeUp;
//         newSwitchConnection.temporary = true;
//         //TODO: Fix this so that the duration is passed through as well
//         if (newSwitchConnection.switchAGO != otherSwitch)
//         {
//             newSwitchConnection.MakeTemporaryConnectionWrapper(this.gameObject, otherSwitch, duration, plottedPath);
//         }
//     }
    
}