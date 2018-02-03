using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class RideConnections : MonoBehaviour
{

    PlayerReferences pReference;
    bool riding;

    GameObject currentSwitch;
    GameObject destinationSwitch;
    GameObject trackSparksGO;
    List<ParticleSystem> trackSparksSystems = new List<ParticleSystem>();


    //let's just have this travel clockwise for now 

    void TestMethod(){

    }
    bool CheckForConnection()
    {
        bool connectionExists = false;
        if (currentSwitch.GetComponent<Switch>().connectedSwitches.Contains(destinationSwitch))
        {
            connectionExists = true;
        }
        return connectionExists;
    }

    private void Awake()
    {
        ChoosePowerUp.connectorChosen += this.TestMethod;
        if(trackSparksGO != null)
            trackSparksSystems = trackSparksGO.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    IEnumerator FindConnectorToRide(GameObject switch1, GameObject switch2){
        while(true){
          //  RaycastHit2D raycastHit = Physics2D.Raycast()
            yield return null;
        }
    }


    // Use this for initialization
    void Start()
    {
        pReference = GetComponentInParent<PlayerReferences>();
    }
    void PlayRideSparks()
    {
        ParticleSystemPlayer.PlayChildParticleSystems(trackSparksSystems);
    }

    void StopRideSparks()
    {

        ParticleSystemPlayer.StopChildParticleSystems(trackSparksSystems);
    }

    IEnumerator RideSwitch(GameObject switchStart, GameObject switchEnd)
    {
        riding = true;
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / 2.0f;
            if (Input.GetKeyUp(KeyCode.R)) {
                //Debug.Log("We're no longer holding on!");
                break;
            }
            transform.position = Vector2.Lerp(currentSwitch.transform.position, destinationSwitch.transform.position, Mathf.SmoothStep(0.0f, 1.0f, time));
            yield return null;
        }
        riding = false;
    }

    void DetermineDirection()
    {
        List<GameObject> connectedSwitches =   pReference.locationHandler.currentSwitch.GetComponent<Switch>().connectedSwitches;


        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(pReference.locationHandler.currentSwitch != null)
            {
                
            }
        }

    }
}
