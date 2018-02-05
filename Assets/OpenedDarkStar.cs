using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class OpenedDarkStar : MonoBehaviour
{
    public GameObject OpenExplosionPSGO;
    public List<ParticleSystem> OpenExplosionParticleSystem;
    SpriteRenderer ourSpriteRenderer;
   _2dxFX_BlackHole  ourBlackHoleEffect;
    Light ourLight;
    public float idealLightIntensity;

    public List<ParticleSystem> OpenDoorParticles = new List<ParticleSystem>();

    public GameObject OpenDoorGO;

    private void Awake()
    {
        //when the door opens, this will have a burst particle effect, and fade in the sprite renderer in the same amount of time
        GameStateHandler.DoorOpened += this.BurstAndCreate;
        OpenExplosionPSGO = GameObject.Find("OpeningExplosion");
        ourSpriteRenderer = GetComponent<SpriteRenderer>();
        OpenExplosionParticleSystem = OpenExplosionPSGO.GetComponentsInChildren<ParticleSystem>().ToList();
        OpenDoorParticles = OpenDoorGO.GetComponentsInChildren<ParticleSystem>().ToList();
        ourBlackHoleEffect = GetComponent<_2dxFX_BlackHole>();
        ourLight = GetComponent<Light>();
        ourLight.intensity = 0.0f;
    } 

    void BurstAndCreate()
    {

        Debug.Log("Are we bursting and creating?");
        ParticleSystemPlayer.PlayChildParticleSystems(OpenExplosionParticleSystem);
        ParticleSystemPlayer.PlayChildParticleSystems(OpenDoorParticles);
 //       ourBlackHoleEffect._Alpha = 0.0f;
//        ourSpriteRenderer.enabled = true;

      //  StartCoroutine(FadeInBlackHoleEffect());
    }

    private void OnEnable()
    {
        //TODO:perhaps have it enable in the darkness without fanfare since it's going to be dark?
        //do some sort of birst particleEffect, then enable the sprite renderer with its black hole effect
        
        //ParticleSystemPlayer.PlayChildParticleSystems(OpenExplosionParticleSystem);
        //ourSpriteRenderer.enabled = true;
    }

    public IEnumerator FadeInBlackHoleEffect()
    {
        Debug.Log("Are we fading in the black hole");
        float startTime = Time.time;
        while(Time.time <= startTime + 5.00f)
        {
            Debug.Log("We're increasing the alpha");
            ourBlackHoleEffect._Alpha = Mathf.Lerp(ourBlackHoleEffect._Alpha, 1, Time.deltaTime * 4.00f); //(Time.deltaTime-startTime/1.00f); //TODO: FADE THIS SHIT 
            Debug.Log("Light is also increasing");

            ourLight.intensity = Mathf.Lerp(ourLight.intensity, 7, Time.deltaTime * 4.00f); //(Time.deltaTime-startTime/1.00f); //TODO: FADE THIS SHIT 
            yield return null; 
        }
        
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
