using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class DarkenStage : MonoBehaviour
{

    public static event Action StageDarkening;

    void BeginningToDarkenStage()
    {

        if(StageDarkening != null)
        {
            StageDarkening(); 
        }
    }
    GameStateHandler gameStateHandler;
    SpriteRenderer playerSpriteRenderer;
    Light starLight;
    SpriteRenderer starSpriteRenderer;
    float defaultLightIntensity;
    ParticleSystem starField;
    List<ParticleSystem> tearBackground = new List<ParticleSystem>();
    List<Light> playerLights;

    public GameObject openedDarkStar;

    // Use this for initialization
    private void Awake()
    {

        starField = GameObject.Find("Starfield").GetComponent<ParticleSystem>();
        tearBackground = GameObject.Find("TearBackground").GetComponentsInChildren<ParticleSystem>().ToList();
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        playerSpriteRenderer = GameStateHandler.player.GetComponent<SpriteRenderer>();
        starSpriteRenderer = gameStateHandler.darkStar.GetComponent<SpriteRenderer>();
        starLight = gameStateHandler.darkStar.GetComponent<Light>();
        defaultLightIntensity = starLight.intensity;
        GameStateHandler.DarkPhaseStarted += this.TurnOffLights;
        playerLights = FindObjectsOfType<Light>().ToList();
        //foreach(Transform child in transform)
        //{

        //    if(child.GetComponentInChildren<Light>() != null)
        //    {
        //        Debug.Log(child.gameObject.name);
        //        playerLights.AddRange(child.GetComponentsInChildren<Light>().ToList());
        //    }
        //    if(child.GetComponent<Light>() != null)
        //    {
        //        playerLights.Add(child.GetComponent<Light>());
        //        Debug.Log(child.gameObject.name);
        //    }
        //}
        //playerLights.AddRange(gameStateHandler.player.GetComponentsInChildren<Light>().ToList());
    }
    void Start()
    {

    }

    void TurnOffLights()
    {
        // gameStateHandler.switchHolder.SetActive(false);
        DimSwitches();
        starField.Stop();
        StartCoroutine(LowerLightIntensity());
        StartCoroutine(FadeDarkStarSprite());
        GameStateHandler.player.GetComponent<HandleChildParticleSystems>().ChangeColorOfAura(this.gameObject, Color.clear);
        ScaleObject.AdjustLightIntensity(this, GameStateHandler.player.GetComponentInChildren<Light>(), 1.0f, 1.0f);
        foreach(Light light in playerLights)
        {
            if (light != null)
            {
                ScaleObject.DimLightOverTime(light, 2, 2);
                light.enabled = false;
            }
        }
        //this will trigger a few seconds after the lights fade to deactivate the dark star and such and instantiate the openedDarkStar object
        StartCoroutine(Wait());

     }
    public void DimSwitches()
    {
        ParticleSystemPlayer.StopChildParticleSystems(tearBackground);

        foreach(Transform child in gameStateHandler.switchHolder.transform)
        {
            child.GetComponent<Switch>().DimSwitchParticles(); 
        }
        
    }
    public IEnumerator LowerLightIntensity()
    {
        while(starLight.intensity > 0)
        {
            starLight.intensity -= Time.deltaTime/1.0f;
            yield return null;
        }
    }

    public IEnumerator BrightenLightIntensity(float amount)
    {
        while(starLight.intensity < amount) { 
            starLight.intensity += Time.deltaTime/1.0f;
            yield return null;

        }
    }

    public IEnumerator FadeDarkStarSprite()
    {
        while(starSpriteRenderer.color.a > 0)
        {
            Color color = starSpriteRenderer.color;
            color.a -= Time.deltaTime / 1.0f;
            starSpriteRenderer.color = color;
            yield return null;
        }
    }

    public IEnumerator FadePlayerSprite()
    {
        while(playerSpriteRenderer.color.a > 0)
        {
            Color color = playerSpriteRenderer.color;
            color.a -= Time.deltaTime / 1.0f;
            playerSpriteRenderer.color = color;
            yield return null;
        }
    }

    public void PowerUpStarOnceOpened()
    {
        StartCoroutine(BrightenLightIntensity(defaultLightIntensity));
        //the light will shine again a bit and you have to get to the star,



        //but it will attract the monster
        //you have to get to the star (Switches provide safe zones?)
        //You fire to find the key, but it draws the enemy to you.

    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(4.0f);
        gameStateHandler.darkStar.SetActive(false);
        GameObject openStar = Instantiate(openedDarkStar, gameStateHandler.darkStar.transform.position, Quaternion.identity);
       // openStar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            TurnOffLights();
        }
    }
}
