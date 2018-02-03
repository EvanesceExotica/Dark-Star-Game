using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
public class SoulRotateScript : MonoBehaviour
{

    public static event Action<GameObject> SuckedInASoul;

    public void SuckedInASoulWrapper(GameObject go)
    {
        if (SuckedInASoul != null)
        {
            SuckedInASoul(go);
        }
    }
    public PlayerReferences playerReferences;
    public List<Transform> soulPositions = new List<Transform>();
    List<GameObject> soulsInRotation = new List<GameObject>();
    public Dictionary<string, Transform> soulTransformParentage_ = new Dictionary<string, Transform>();
    public Dictionary<Transform, GameObject> soulTransformParentage = new Dictionary<Transform, GameObject>();

    #region 
    public List<ParticleSystem> chargeParticlesystem;
    public bool rotate;
    public Vector3 rotationSpeed;
    #endregion
    public float drawInSpeed;
    public float drawOutSpeed;

    public int numberOfSouls;

    public bool soulSuckedIn;
    GameObject suckedInSoul;

    public PlayerSoulHandler soulHandler;

    ParticleSystem.Particle[] soulParticles = new ParticleSystem.Particle[40];
    void Awake()
    {
        chargeParticlesystem = GetComponentsInChildren<ParticleSystem>().ToList();
        playerReferences = GetComponentInParent<PlayerReferences>();
        soulHandler = playerReferences.playerSoulHandler;
        foreach (Transform child in transform)
        {
            soulPositions.Add(child);
        }
        soulTransformParentage_.Add("TopPosition", transform.GetChild(0));
        soulTransformParentage_.Add("BottomMiddlePosition", transform.GetChild(1));
        soulTransformParentage_.Add("BottomLeftPosition", transform.GetChild(2));
        soulTransformParentage_.Add("BottomRightPosition", transform.GetChild(3));

        foreach (Transform newParent in soulPositions)
        {
            soulTransformParentage.Add(newParent, null);
        }
    }

    void Start()
    {
        numberOfSouls = 0;
    }


    public void AddNewSoulWrapper(GameObject soulToBeAdded)
    {

        StartCoroutine(AddNewSoul(soulToBeAdded));
    }

    public IEnumerator AddNewSoul(GameObject soul)
    {
        Debug.Log("New soul is being added");
        //NOTE: IF you pick the souls up in quick succession they won't have time to reach their proper locations.
        GameObject previousSoul = null;
        int soulNumber = soulHandler.soulsAttachedToPlayer.IndexOf(soul);

        Transform properTransform = null;
        Transform transformToMakeParentOfLastSoul = null;
        if (soulNumber == 2 /*numberOfSouls == 2*/)
        {
            //this will be the third soul that was grabbed;

            //vv this will be the soul in the second position
            previousSoul = soulHandler.soulsAttachedToPlayer[1] ;

            //previousSoul = soulsInRotation.Last();
        }
        if (soulNumber == 3)
        {
            //max number of souls the player can have
            yield break;
        }
        else if (soulNumber == 0 || soulNumber == 1)
        {
            properTransform = DetermineWhichTransform(soulNumber);
           // soul.transform.parent = properTransform;
            //properPosition = properTransform.position;
            //this method should take the new soul where the player picks it up and have it dragged toward an unclaimed position around the player
            while (Vector2.Distance(soul.transform.position, properTransform.position) > 0.2f)
            {
              //  Debug.Log("WE haven't reached it yet" + soul.name + " We have " + Vector2.Distance(soul.transform.position, properTransform.position) + " units to go ");
                soul.transform.position = Vector2.MoveTowards(soul.transform.position, properTransform.position, 5.0f * Time.deltaTime);
                yield return null;
            }
           // EditorApplication.isPaused = true;
            
        }
        else if (soulNumber == 2)
        {

            properTransform = DetermineWhichTransform(soulNumber);
           // soul.transform.parent = properTransform;
            //properPosition = properTransform.position;
            transformToMakeParentOfLastSoul = DetermineWhichTransform(3)/*(soulTransformParentage_.Last().Value*/;
            //positionToMoveLastSoulTo = transformToMakeParentOfLastSoul.position;
            while (true/*Vector2.Distance(soul.transform.position, properTransform.position) > 0.5f && Vector2.Distance(previousSoul.transform.position, transformToMakeParentOfLastSoul.position) > 0.5f*/)
            {
                float distance = Vector2.Distance(soul.transform.position, properTransform.position);
                float previousSoulDistance = Vector2.Distance(previousSoul.transform.position, transformToMakeParentOfLastSoul.transform.position);
                if(distance < 0.5f && previousSoulDistance < 0.2f){
                    break;
                }
                soul.transform.position = Vector2.MoveTowards(soul.transform.position, properTransform.position, 5.0f * Time.deltaTime);
              //  previousSoul.transform.parent = null;
                previousSoul.transform.position = Vector2.MoveTowards(previousSoul.transform.position, transformToMakeParentOfLastSoul.position, 5.0f * Time.deltaTime);
                yield return null;

            }

        }
        soul.transform.parent = properTransform;
        soul.transform.position = soul.transform.parent.transform.position;
        soul.GetComponent<Rigidbody2D>().isKinematic = true;
        if (soulNumber == 2)
        {
            previousSoul.transform.parent = transformToMakeParentOfLastSoul;
        }
        soulsInRotation.Add(soul);
        numberOfSouls++;

    }

    Transform DetermineWhichTransform(int soulNumber)
    {
        Transform correctTransform = null;
        if (soulNumber == 0)
        {
            correctTransform = soulTransformParentage_["TopPosition"];
        }
        else if (soulNumber == 1)
        {
            correctTransform = soulTransformParentage_["BottomMiddlePosition"];
        }
        else if (soulNumber == 2)
        {
            correctTransform = soulTransformParentage_["BottomLeftPosition"];
        }
        else if (soulNumber == 3)
        {
            correctTransform = soulTransformParentage_["BottomRightPosition"];
        }
        return correctTransform;
    }

    void MoveGameObjectTowardTarget(GameObject ourGameObject, Transform target, float speed)
    {

        ourGameObject.transform.position = Vector2.MoveTowards(ourGameObject.transform.position, target.position, speed * Time.deltaTime);
    }

    public void SuckInSoulWrapper()
    {
        StartCoroutine(SuckInSoul());
    }
    IEnumerator SuckInSoul()
    {
        GameObject soul = null;
        float Distance = 0;
        float secondSoulDistance = 0;
        if (numberOfSouls == 0 || soulSuckedIn)
        {
            yield break;
        }
        else if (numberOfSouls == 3 || numberOfSouls == 2)
        {
            //the soul added secondd
            soul = playerReferences.playerSoulHandler.soulsAttachedToPlayer[1];
            //Distance = Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position);
            //Transform transformToMoveRemainingSoulTo = null;
            if (numberOfSouls == 3)
            {
                GameObject previousSoul = playerReferences.playerSoulHandler.soulsAttachedToPlayer.Last();
                while (true)
                {

                    Distance = Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position);
                    secondSoulDistance = Vector2.Distance(previousSoul.transform.position, DetermineWhichTransform(1).position);

                    if(Distance < 0.2f && secondSoulDistance < 0.2f){
                        break;
                    }
                    Debug.Log(soul + "," + previousSoul);
                    //We need to remove the soul added second for visual effect since it's at the bottom right position, so remove at index 1
                    MoveGameObjectTowardTarget(soul, GameStateHandler.player.transform, (5.0f * Distance));

                    //then we need to  move the soul added last, which is at the bottom left position, to the bottom middle position
                    MoveGameObjectTowardTarget(previousSoul, DetermineWhichTransform(1), 5.0f);
                    yield return null;
                }
            }
            else if (numberOfSouls == 2)
            {

                while (Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position) > 0.2f)
                {
                    Distance = Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position);
                    MoveGameObjectTowardTarget(soul, GameStateHandler.player.transform, (5.0f * Distance));
                    yield return null;
                }
            }
        }
        else if (numberOfSouls == 1)
        {
            soul = playerReferences.playerSoulHandler.soulsAttachedToPlayer[0];
            //  Distance = Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position);
            while (Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position) > 0.2f)
            {
                Distance = Vector2.Distance(soul.transform.position, GameStateHandler.player.transform.position);
                MoveGameObjectTowardTarget(soul, GameStateHandler.player.transform, (5.0f * Distance));
                yield return null;
            }

        }
        if(numberOfSouls == 3)
        {
            soulsInRotation.Last().transform.parent = DetermineWhichTransform(1);
        }
        SuckedInASoulWrapper(soul);
        //No reversal - no going back
        soul.transform.parent = GameStateHandler.player.transform;
        soulsInRotation.Remove(soul);
        suckedInSoul = soul;
        soulSuckedIn = true;

    }

    IEnumerator ReverseSuckIn()
    {
        if (!soulSuckedIn)
        {
            yield break;
        }
        GameObject soul = null;
        float Distance = 0;

        if (numberOfSouls == 1)
        {
            Distance = Vector2.Distance(soul.transform.position, DetermineWhichTransform(0).position);
            while (Distance > 0.5f)
            {
                MoveGameObjectTowardTarget(soul, DetermineWhichTransform(0), 5.0f);
                yield return null;
            }
        }
        else if (numberOfSouls == 2)
        {
            Distance = Vector2.Distance(soul.transform.position, DetermineWhichTransform(0).position);
            while (Distance > 0.5f)
            {
                MoveGameObjectTowardTarget(soul, DetermineWhichTransform(0), 5.0f);
                yield return null;
            }
        }
        else if (numberOfSouls == 3)
        {

        }
    }

    void DeleteSoul()
    {
        //this should trigger upon death and power up being chosen
    }
    void Update()
    {


    }




}
