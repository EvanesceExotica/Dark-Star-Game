using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchConnection : PooledObject
{


    public Switch switchA;
    public GameObject switchAGO;
    public Switch switchB;
    public GameObject switchBGO;
    LineRenderer ourLineRenderer;
    // Use this for initialization
    void Awake(){
        ourLineRenderer = GetComponent<LineRenderer>();
    }
    bool transferingPower;
    bool temporary;

	void TransferingPowerWrapper(){
		if(TransferingPower != null){
			TransferingPower(this.gameObject);
		}
	}
	public event Action<GameObject> NotTransferingPower;
	void NotTransferingPowerWrapper(){
		if(NotTransferingPower != null){
			NotTransferingPower(this.gameObject);
		}	
	}
	public event Action<GameObject> TransferingPower;
	List<GameObject> poweredSwitchesWereConnecting = new List<GameObject>();


	public void BeginTransferPower_(GameObject poweredSwitchGO){
		poweredSwitchesWereConnecting.Add(poweredSwitchGO);
		TransferingPowerWrapper();
		transferingPower = true;
	}

	public void RemoveTransferingPower(GameObject previouslyPoweredSwitchGO){
		poweredSwitchesWereConnecting.Remove(previouslyPoweredSwitchGO);
		if(poweredSwitchesWereConnecting.Count == 0){
			NotTransferingPowerWrapper();
			transferingPower = false;
		}
	}

	void LostPower(GameObject previouslyPoweredSwitch){
		transferingPower = false;
		if(previouslyPoweredSwitch == switchAGO){
			switchB.Depowered(switchAGO);
		}
		if(previouslyPoweredSwitch == switchBGO){
			switchA.Depowered(switchBGO);
		}
	}

    void TransferPower(GameObject poweredSwitch)
    {
        transferingPower = true;
        if (poweredSwitch == switchAGO)
        {
            switchB.Powered(Switch.transferType.switchConnection, switchAGO);
        }
        else if(poweredSwitch == switchBGO)
        {
            switchA.Powered(Switch.transferType.switchConnection, switchBGO);
        }

    }

    public void MakeConnection(GameObject a, GameObject b, bool temporary)
    {
        switchAGO = a;
        switchA = switchAGO.GetComponent<Switch>();
        switchBGO = b;
        switchB = switchBGO.GetComponent<Switch>();

        if (!temporary)
        {
            ourLineRenderer.SetPosition(0, a.transform.position);
            ourLineRenderer.SetPosition(1, b.transform.position);
        }


    }

    public void MakeTemporaryConnectionWrapper(GameObject a, GameObject b, float duration, List<Vector3> plottedPath)
    {
        StartCoroutine(MakeTemporaryConnection(a, b, duration, plottedPath));
    }

    public IEnumerator MakeTemporaryConnection(GameObject a, GameObject b, float passedDuration, List<Vector3> plottedPath)
    {
        float startTime = Time.time;
        float duration = passedDuration;
        ourLineRenderer.SetPositions(plottedPath.ToArray());
		yield return new WaitForSeconds(duration);
        DestroyUs();
    }

	bool checkIfConnectionExists(){
		bool connectionExists = false;


		return connectionExists;
	}

    void DestroyUs()
    {
        this.ReturnToPool();
    }
}
