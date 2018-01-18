using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

struct ParticleSystemInfo
{
    GameObject psGameObject;
    List<ParticleSystem> childParticleSystems;
    Color defaultMainColor;
    Color activatedMainColor;
    HandleChildParticleSystems.ParticleSystemObjectTypes psObjectType;


}

public class HandleChildParticleSystems : MonoBehaviour
{
public class ParticleSystemInfo
{
   public GameObject psGameObject;
    public List<ParticleSystem> childParticleSystems;
    public Color defaultMainColor;
    public Color activatedMainColor;
    public ParticleSystemObjectTypes psObjectType;

       public ParticleSystemInfo(GameObject ourObject, List<ParticleSystem> ourChildParticleSystems, Color ourDefaultColor, Color ourActivatedColor, ParticleSystemObjectTypes ourType)
        {
            ourObject = psGameObject;
            ourChildParticleSystems = childParticleSystems;
            ourDefaultColor = defaultMainColor;
            ourActivatedColor = activatedMainColor;
            ourType = psObjectType;
            
        }


}
    //this is attached to the player;
    GameObject playerAura; 
    LocationHandler ourLocationHandler;
    bool playingPlayerAura;
    
    public enum ParticleSystemObjectTypes{

        shield,
        aura






    }


    ParticleSystemInfo DetermineType(ParticleSystemInfo psInfo, ChildParticleSystem childParticleSystem)
    {
        
        if(childParticleSystem.ourPSObjectType == ParticleSystemObjectTypes.aura)
        {

        }
        return psInfo;
    }
    List<ParticleSystemInfo> ourPlayerParticleSystems = new List<ParticleSystemInfo>();

    Dictionary<GameObject, List<ParticleSystem>> childAndParticleSystems = new Dictionary<GameObject, List<ParticleSystem>>();

    private void Awake()
    {
        PlayerTriggerHandler.AuraHoveredOverInteractable += ChangeColorOfAura;
        PlayerTriggerHandler.AuraHoveredOverNothing += ChangeColorOfAuraBack;
        ourLocationHandler = GetComponent<PlayerReferences>().locationHandler;

        List<ChildParticleSystem> childSystems = GetComponentsInChildren<ChildParticleSystem>().ToList();

        foreach(ChildParticleSystem system in childSystems)
        {
            //ParticleSystem.MainModule settings = system.GetParticleSystems()[0].main;
            //Color oldColor = settings.startColor.color;
            //ParticleSystemInfo newPSInfo;
            //newPSInfo.defaultMainColor = oldColor;
            //newPSInfo.childParticleSystems = system.GetParticleSystems();
            //newPSInfo.psGameObject = system.gameObject;
            //DetermineType(system);
            //newPSInfo.activatedMainColor = Color.yellow;

            childAndParticleSystems.Add(system.gameObject, system.GetParticleSystems());
        }
        foreach(GameObject go in childAndParticleSystems.Keys)
        {
          //  //Debug.Log("This is in the child particle systems " + go.name);
            foreach(ParticleSystem ps in childAndParticleSystems[go])
            {
                ////Debug.Log("Here are the sub-systems " + ps.gameObject.name);
            }
        }
    }
    //this needs to be updated to cancel ouf of time stuff early just in case. 

     IEnumerator ChangeSystemColor(ParticleSystem ps, Color color)
    {
        ParticleSystem.MainModule settings = ps.main;
        Color oldColor = settings.startColor.color;
        float elapsedTime = 0.0f;
        float totalTime = 1.0f;
        while(elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            settings.startColor = new ParticleSystem.MinMaxGradient(color);
            yield return null;
        }
    }

    IEnumerator ChangeAuraColor(Color newcolor)
    {
        //Debug.Log("Changing color!");
        ParticleSystem.MainModule settings = childAndParticleSystems[playerAura][1].main;
        Color oldColor = settings.startColor.color;
        float elapsedTime = 0.0f;
        float totalTime = 1.0f;
        while(elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            settings.startColor = new ParticleSystem.MinMaxGradient(newcolor);
            yield return null;
        }
    }
    
   public void ChangeColorOfAuraBack(Color color)
    {
        StartCoroutine(ChangeAuraColor(color));
    }
    public void ChangeColorOfAura(GameObject go, Color color)
    {
        StartCoroutine(ChangeAuraColor(color));
    }
    IEnumerator PlaySystemThenStopAfterTime(float playTime, List<ParticleSystem> ps1)
    {
        ParticleSystemPlayer.PlayChildParticleSystems(ps1);
        yield return new WaitForSeconds(playTime);
        ParticleSystemPlayer.StopChildParticleSystems(ps1);
    }

    IEnumerator PlayLoopingStopThenPlayOneshot(float playTime, List<ParticleSystem> ps1, List<ParticleSystem> ps2)
    {
        ParticleSystemPlayer.PlayChildParticleSystems(ps1);
        yield return new WaitForSeconds(playTime);
        ParticleSystemPlayer.StopChildParticleSystems(ps1);
        ParticleSystemPlayer.PlayChildParticleSystems(ps2);
    }

    void PlaySystem(float playTime, List<ParticleSystem> ps1)
    {
        playingPlayerAura = true;
        ParticleSystemPlayer.PlayChildParticleSystems(ps1);
    }

    void StopSystem(List<ParticleSystem> ps1)
    {

        playingPlayerAura = false;
        ParticleSystemPlayer.StopChildParticleSystems(ps1);
    }


    // Use this for initialization
    void Start()
    {
        //test
        playerAura = GameObject.Find("PlayerAura");

    }

    // Update is called once per frame
    void Update()
    {
        if (ourLocationHandler.floatingFree && !playingPlayerAura)
        {
            PlaySystem(10.0f, childAndParticleSystems[playerAura]);
        }
        else if(ourLocationHandler.anchored && playingPlayerAura)
        {
            StopSystem(childAndParticleSystems[playerAura]);
        }
    }
}
