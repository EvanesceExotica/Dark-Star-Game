using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
public class ChoosePowerUp : MonoBehaviour
{
    //
    CanvasGroup ourCentralPowerUpCanvasGroup;
    Image chosenPowerUpImage;
    LocationHandler playerLocationHandler;
    bool acceptingCollision;
    Collider2D ourCollider;
    PowerupHandlerDeprecated powerupHandler;

    GameObject soulBeingLaunched;
    public PowerUpTypes ourPowerUpType;

    Image ourImage;


    public enum PowerUpTypes
    {
        laser,
        connector,
        chain
    }

    public Sprite laserOnSwitch;
    public Sprite laserOffSwitch;

    public Sprite connectorOnSwitch;
    public Sprite connectorOffSwitch;

    public Sprite chainOnSwitch;
    public Sprite chainOffSwitch;

    float speed = 3.0f;
    public void DisplayCentralIcon()
    {

        StartCoroutine(FadeInCentralIcon());
    }

    public IEnumerator FadeInCentralIcon()
    {
        while (ourCentralPowerUpCanvasGroup.alpha < 1)
        {
            ourCentralPowerUpCanvasGroup.alpha += (speed * Time.deltaTime);
            yield return null;
        }
    }
    public void HideCentralIcon()
    {
        StartCoroutine(FadeOutCentralIcon());
    }
    public IEnumerator FadeOutCentralIcon()
    {
        while (ourCentralPowerUpCanvasGroup.alpha > 0)
        {
            ourCentralPowerUpCanvasGroup.alpha -= (speed * Time.deltaTime);
            yield return null;
        }

    }

    void ChangeIconToSwitchVersion(GameObject irrelevant)
    {

        if (ourPowerUpType == PowerUpTypes.laser)
        {
            ourImage.sprite = laserOnSwitch;
        }
        else if (ourPowerUpType == PowerUpTypes.connector)
        {
            ourImage.sprite = connectorOnSwitch;
        }
        else if (ourPowerUpType == PowerUpTypes.chain)
        {
            ourImage.sprite = chainOnSwitch;
        }
    }

    void ChangeIconToOffSwitchVersion(GameObject irrelevant)
    {
        if (ourPowerUpType == PowerUpTypes.laser)
        {
            ourImage.sprite = laserOffSwitch;
        }
        else if (ourPowerUpType == PowerUpTypes.connector)
        {
            ourImage.sprite = connectorOffSwitch;
        }
        else if (ourPowerUpType == PowerUpTypes.chain)
        {
            ourImage.sprite = chainOffSwitch;
        }
    }

    public static event Action powerupChosen;
    public static event Action<PowerUpTypes> SpecificPowerUpChosen;

    void ChoseSpecificPowerUp(PowerUpTypes chosen)
    {
        if (SpecificPowerUpChosen != null)
        {
            SpecificPowerUpChosen(chosen);
        }
    }

    void ChoseAnyPowerUp()
    {
        chosenPowerUpImage.sprite = ourImage.sprite;
        DisplayCentralIcon();

        if (powerupChosen != null)
        {
            powerupChosen();
        }
    }

    public static event Action chainChosen;

    void ChoseChain()
    {
        if (chainChosen != null)
        {
            chainChosen();
        }
    }
    public static event Action connectorChosen;

    void ChoseConnector()
    {

        if (connectorChosen != null)
        {
            connectorChosen();
        }
    }

    public static event Action laserChosen;


    void ChoseLaser()
    {

        if (laserChosen != null)
        {
            laserChosen();
        }
    }

    [SerializeField] ParticleSystem shatterParticles;
    void Awake()
    {
        PowerUp.StoppedUsingPowerUp += HideCentralIcon;
        Transform chosenPowerUpCanvas = transform.parent.parent.Find("ChosenPowerUpCanvas");
        ourCentralPowerUpCanvasGroup = chosenPowerUpCanvas.GetComponent<CanvasGroup>();
        chosenPowerUpImage = chosenPowerUpCanvas.GetComponentInChildren<Image>();
        ourCollider = GetComponent<Collider2D>();
        ourCollider.enabled = false;
        LaunchSoul.SoulToBeLaunched += this.AcceptingCollision;
        LaunchSoul.SoulNotLaunching += this.NotAcceptingCollision;

        shatterParticles = GetComponentInChildren<ParticleSystem>();
        powerupHandler = GetComponentInParent<PowerupHandlerDeprecated>();

        Switch.SwitchEntered += this.ChangeIconToSwitchVersion;
        Switch.SwitchExited += this.ChangeIconToOffSwitchVersion;
        ourImage = GetComponent<Image>();
    }

    void Start()
    {
        ChangeIconToOffSwitchVersion(null);
    }

    void AcceptingCollision(GameObject launchedSoul)
    {
        soulBeingLaunched = launchedSoul;
        acceptingCollision = true;
        ourCollider.enabled = true;

    }

    void NotAcceptingCollision()
    {
        acceptingCollision = false;
        ourCollider.enabled = false;
    }
    public string powerUpType;

    void OnTriggerEnter2D(Collider2D hit)
    {

        SoulBehavior soulBehavior = hit.GetComponent<SoulBehavior>();
        if (hit.gameObject == soulBeingLaunched && soulBehavior != null)
        {
            powerupHandler.HideIcons();
            ChoseAnyPowerUp();
            shatterParticles.Play();
            DeterminePowerup();
            soulBehavior.ReturnToPool();
        }
    }

    void DeterminePowerup()
    {
        if (ourPowerUpType == PowerUpTypes.connector)
        {
            Debug.Log("connector was chosen");
            ChoseConnector();
            ChoseSpecificPowerUp(PowerUpTypes.connector);
        }
        else if (ourPowerUpType == PowerUpTypes.laser)
        {
            Debug.Log("laser was chosen");
            ChoseLaser();
            ChoseSpecificPowerUp(PowerUpTypes.laser);
        }
        else if (ourPowerUpType == PowerUpTypes.chain)
        {
            Debug.Log("chain was chosen");
            ChoseChain();
            ChoseSpecificPowerUp(PowerUpTypes.chain);
        }
    }

}
