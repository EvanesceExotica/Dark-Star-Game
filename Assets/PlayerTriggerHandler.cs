using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class PlayerTriggerHandler : MonoBehaviour
{
    Color defaultAuraColor;



    public List<GameObject> objectsHoveredOver = new List<GameObject>();

    HandleChildParticleSystems particleSystemHandler;
    PlayerReferences pReference;

    public static event Action<GameObject, Color> AuraHoveredOverInteractable;

    void VoidNotHoveredOver()
    {
        if(NoLongerHoveringOverVoid != null)
        {
            NoLongerHoveringOverVoid();
        }
    }
    void VoidHoveredOver()
    {
        //Debug.Log("We're hovering over the void!");
        if(HoveringOverVoid != null)
        {
            HoveringOverVoid();
        }
    }
    public static event Action HoveringOverVoid;
    public static event Action NoLongerHoveringOverVoid;

    void HoveredOverInteractable(GameObject obj, Color color)
    {
        //Debug.Log("We should be turning gold");
        if(AuraHoveredOverInteractable != null)
        {
            AuraHoveredOverInteractable(obj, color);

        }
    }

    public static event Action<Color> AuraHoveredOverNothing;

    void NotHovering(Color color)
    {
        if(AuraHoveredOverNothing != null){
            AuraHoveredOverNothing(color);
        }
    }
    private void Awake()
    {
        pReference = GetComponentInParent<PlayerReferences>();
        particleSystemHandler = GetComponent<HandleChildParticleSystems>();
        defaultAuraColor = new Color32(39, 34, 206, 17);
    }
    void DetermineWhatWasHit(GameObject hitObject)
    {
        //Debug.Log("This is being hit for " + hitObject);
        //make sure these are all able to be effected by the bash trigger and on the right layers.
        Key key = hitObject.GetComponent<Key>();
        Enemy enemy = hitObject.GetComponent<Enemy>();
        SoulBehavior soul = hitObject.GetComponent<SoulBehavior>();
        Switch ourSwitch = hitObject.GetComponent<Switch>();
        ProximityToVoidWarning voidBarrier = hitObject.GetComponent<ProximityToVoidWarning>();
        IPullable pullableObject = hitObject.GetComponent<IPullable>();
        InteractableTransformSpot transformSpot = hitObject.GetComponent<InteractableTransformSpot>();

        if(transformSpot != null){
            
        }
        if(key != null)
        {
            Debug.Log("Found key");
            //Debug.Log("Found key!");
            key.KeyGrabbed();
        }
        if(soul != null)
        {
            if(pullableObject != null){
                pullableObject.CancelPull();
            }
            //Debug.Log("Found soul!");
            if (soul.attachmentState != SoulBehavior.Attachments.AttachedToPlayer)
            {
                soul.Attached();
            }

        }
        if(enemy != null)
        {
            if(pullableObject != null){
                pullableObject.CancelPull();
            }
            //Debug.Log("Hovering over enemy!");
        }
        if(ourSwitch != null)
        {

            pReference.switchPuller.switchObject = hitObject;
            pReference.switchPuller.inRangeOfSwitch = true;

            ////Debug.Log("Hovering over switch!" + ourSwitch.gameObject.name);
           
        }
        if(voidBarrier != null)
        {
            VoidHoveredOver();
        }
    }

    void DetermineWhatExited(GameObject hitObject)
    {
        ProximityToVoidWarning voidBarrier = hitObject.GetComponent<ProximityToVoidWarning>();
        Switch ourSwitch = hitObject.GetComponent<Switch>();

        if(voidBarrier != null)
        {

            VoidNotHoveredOver();
        }
        if (ourSwitch != null)
        {
           // //Debug.Log("Leaving switch! " + ourSwitch.name);

            pReference.switchPuller.switchObject = null;
            pReference.switchPuller.inRangeOfSwitch = false;
           
        }
    }
    //fix this up later

    private void OnTriggerEnter2D(Collider2D hit)
    {
        objectsHoveredOver.Add(hit.gameObject);
        HoveredOverInteractable(hit.gameObject, defaultAuraColor);
        DetermineWhatWasHit(hit.gameObject);
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        objectsHoveredOver.Remove(hit.gameObject);
        DetermineWhatExited(hit.gameObject);
        if(objectsHoveredOver.Count == 0)
        {
            NotHovering(Color.yellow); 
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
