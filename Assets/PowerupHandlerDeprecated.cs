using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerupHandlerDeprecated : MonoBehaviour
{
    Sprite shieldSprite;
    Sprite connectorSprite;
    PlayerReferences playerReferences;
    public List<GameObject> ourIcons = new List<GameObject>();
    public List<Image> ourIconsImages = new List<Image>();

    float speed;
    CanvasGroup ourCanvasGroup;

    private void Awake()
    {
        ourCanvasGroup = GetComponent<CanvasGroup>();
        foreach (Transform child in transform)
        {
            ourIcons.Add(child.gameObject);
        }
        ourCanvasGroup.alpha = 0.0f;

        playerReferences = GetComponentInParent<PlayerReferences>();
        LaunchSoul.SoulPriming += this.DisplayIcons;
        LaunchSoul.SoulNotLaunching += this.HideIcons;
    }

    public void DisplayCentralIcon()
    {


    }

    public IEnumerator FadeInCentralIcon()
    {
        while (ourCanvasGroup.alpha < 1)
        {
            ourCanvasGroup.alpha += (speed * Time.deltaTime);
            yield return null;
        }
    }

    public void DisplayIcons()
    {
        StartCoroutine(FadeInIcons());
    }

    public IEnumerator FadeInIcons()
    {
        while (ourCanvasGroup.alpha < 1)
        {
            ourCanvasGroup.alpha += (speed * Time.deltaTime);
            yield return null;
        }


    }

    public IEnumerator FadeOutIcons()
    {
        while (ourCanvasGroup.alpha > 0)
        {
            ourCanvasGroup.alpha -= (speed * Time.deltaTime);
            yield return null;
        }

    }
    public void HideIcons()
    {
        StartCoroutine(FadeOutIcons());
    }

    bool poweredUp;
    Image ourImage;
    Text ourText;

    public Dictionary<KeyCode, PowerUp> powerUpCollection;


    void Start()
    {
        speed = 3;
    }

    #region 
    //  public class PowerUp
    // {
    //     public KeyCode keyCode;
    //     public Sprite sprite;
    //     public string text;

    //     public PowerUp(KeyCode ourKeyCode, Sprite ourSprite, string ourText)
    //     {
    //         keyCode = ourKeyCode;
    //         sprite = ourSprite;
    //         text = ourText;
    //     }

    // }
    // void YesPoweredUp()
    // {
    //     poweredUp = true;
    // }

    // void NotPoweredUp()
    // {

    //     poweredUp = false;
    // }

    // List<PowerUp> ourPowerUps;

    // // Use this for initialization
    // void Start()
    // {

    //     speed = 3;
    // }

    // void DisplaySprite(Sprite spriteToDisplay, string textToDisplay)
    // {
    //     ourImage.enabled = true;
    //     ourImage.sprite = spriteToDisplay;
    //     ourText.text = textToDisplay;
    // }

    // void HideSprite()
    // {
    //     ourImage.enabled = false;
    //     ourImage.sprite = null;
    //     ourText.text = "";
    // }

    // public IEnumerator SelectPowerUpCoroutine()
    // {
    //     while (poweredUp)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Alpha1))
    //         {
    //             //if "1" is pressed

    //             DisplaySprite(powerUpCollection[KeyCode.Alpha1].sprite, powerUpCollection[KeyCode.Alpha1].text);
    //             // Show the sprite of the powerup corresponding to "1"

    //             if (Input.GetKeyUp(KeyCode.Alpha1))
    //             {
    //                 //if 1 is released, activate the correct function (shield, connector);
    //                 ChargeCorrectPowerUp(powerUpCollection[KeyCode.Alpha1]);
    //                 break;
    //             }
    //         }
    //             if (Input.GetKeyDown(KeyCode.Alpha2))
    //             {
    //                 //if "2" is pressed

    //                 DisplaySprite(powerUpCollection[KeyCode.Alpha2].sprite, powerUpCollection[KeyCode.Alpha2].text);
    //                 // Show the sprite of the powerup corresponding to "1"

    //                 if (Input.GetKeyUp(KeyCode.Alpha2))
    //                 {
    //                     //if 2 is released, activate the correct function (shield, connector);
    //                     ChargeCorrectPowerUp(powerUpCollection[KeyCode.Alpha2]);
    //                     break;
    //                 }
    //             }

    //         if (Input.GetKeyDown(KeyCode.Alpha3))
    //         {
    //             //if "1" is pressed

    //             DisplaySprite(powerUpCollection[KeyCode.Alpha3].sprite, powerUpCollection[KeyCode.Alpha3].text);
    //             // Show the sprite of the powerup corresponding to "1"

    //             if (Input.GetKeyUp(KeyCode.Alpha3))
    //             {
    //                 //if 1 is released, activate the correct function (shield, connector);
    //                 ChargeCorrectPowerUp(powerUpCollection[KeyCode.Alpha3]);
    //                 break;
    //             }
    //         }


    //         HideSprite();

    //         yield return null;
    //     }
    // }
    // Update is called once per frame
    #endregion
    void Update()
    {

    }
}
