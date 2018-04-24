using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SwitchConnection : PooledObject
{

    public InteractableTransformSpot transformSpotPrefab;
    private float distance;

    public float Distance
    {
        get { return distance; }
    }
    public Switch switchA;
    public GameObject switchAGO;
    public Switch switchB;
    public GameObject switchBGO;
    LineRenderer ourLineRenderer;

    private bool lineRendererPositionsSet;

    public List<Vector3> pathPoints = new List<Vector3>();

    public List<Vector3> prunedPathPoints = new List<Vector3>();

    // Use this for initialization
    void Awake()
    {
        ourLineRenderer = GetComponent<LineRenderer>();

    }
    bool transferingPower;
    public bool temporary;

    void TransferingPowerWrapper()
    {
        if (TransferingPower != null)
        {
            TransferingPower(this.gameObject);
        }
    }
    public event Action<GameObject> NotTransferingPower;
    void NotTransferingPowerWrapper()
    {
        if (NotTransferingPower != null)
        {
            NotTransferingPower(this.gameObject);
        }
    }
    public event Action<GameObject> TransferingPower;
    List<GameObject> poweredSwitchesWereConnecting = new List<GameObject>();


    public void BeginTransferPower_(GameObject poweredSwitchGO)
    {
        poweredSwitchesWereConnecting.Add(poweredSwitchGO);
        TransferingPowerWrapper();
        transferingPower = true;
    }

    public void RemoveTransferingPower(GameObject previouslyPoweredSwitchGO)
    {
        poweredSwitchesWereConnecting.Remove(previouslyPoweredSwitchGO);
        if (poweredSwitchesWereConnecting.Count == 0)
        {
            NotTransferingPowerWrapper();
            transferingPower = false;
        }
    }

    void LostPower(GameObject previouslyPoweredSwitch)
    {
        transferingPower = false;
        if (previouslyPoweredSwitch == switchAGO)
        {
            switchB.Depowered(switchAGO);
        }
        if (previouslyPoweredSwitch == switchBGO)
        {
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
        else if (poweredSwitch == switchBGO)
        {
            switchA.Powered(Switch.transferType.switchConnection, switchBGO);
        }

    }

    public void MakeConnection(GameObject a, GameObject b)
    {
        switchAGO = a;
        switchA = switchAGO.GetComponent<Switch>();
        switchBGO = b;
        switchB = switchBGO.GetComponent<Switch>();
        distance = Vector2.Distance(a.transform.position, b.transform.position);
        //have switchB connect back 
        switchB.AddSwitchConnectionAndSubscribe(switchAGO, this);

        if (!lineRendererPositionsSet)
        {
            //HEre we're making sure the positions are only set once;
            ourLineRenderer.SetPosition(0, a.transform.position);
            pathPoints.Add(a.transform.position);
            ourLineRenderer.SetPosition(1, b.transform.position);
            pathPoints.Add(b.transform.position);
            CreatePrunedList();
            lineRendererPositionsSet = true;
        }

    }

    public void CreatePrunedList()
    {
        if (pathPoints == null || pathPoints.Count == 0)
        {
            return;
        }
        if(!temporary){
            int i = 0;
            Vector3 aPosition = switchAGO.transform.position;
            Vector3 bPosition = switchBGO.transform.position;
            
            for(i = 0; i < (int)distance; i+= 3){
                //determine a new point at every 3 units away
               Vector3 newPoint = Vector3.Lerp(aPosition, bPosition, i/distance) ;
            }
        }
        else
        {
            int i = 0;
            for (i = 0; i < pathPoints.Count; i += 2)
            {
                prunedPathPoints.Add(pathPoints[i]);
            }
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
        switchAGO = a;
        switchA = switchAGO.GetComponent<Switch>();
        switchBGO = b;
        switchB = switchBGO.GetComponent<Switch>();
        switchB.AddTemporarySwitchConnectionAndSubscribe(switchAGO, this, plottedPath, passedDuration);
        distance = Vector2.Distance(a.transform.position, b.transform.position);
        if (!lineRendererPositionsSet)
        {
            pathPoints = plottedPath.ToList();
            CreatePrunedList();
            ourLineRenderer.positionCount = pathPoints.Count;
            ourLineRenderer.SetPositions(pathPoints.ToArray());
            //HEre we're making sure the positions are only set once;
            lineRendererPositionsSet = true;

        }
        yield return new WaitForSeconds(duration);
        DestroyUs();
    }

    bool checkIfConnectionExists()
    {
        bool connectionExists = false;


        return connectionExists;
    }

    void DestroyUs()
    {
        this.ReturnToPool();
    }
}
