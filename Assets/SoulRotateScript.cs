using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoulRotateScript : MonoBehaviour
{

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

    ParticleSystem.Particle[] soulParticles = new ParticleSystem.Particle[40];
    void Awake()
    {
        chargeParticlesystem = GetComponentsInChildren<ParticleSystem>().ToList();
        playerReferences = GetComponentInParent<PlayerReferences>();
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
		GameObject previousSoul = null;

		Transform properTransform = null;
		Transform transformToMakeParentOfLastSoul = null;
		Vector2 properPosition = new Vector2(0, 0);
		Vector2 positionToMoveLastSoulTo = new Vector2(0, 0);
		if(numberOfSouls == 1 || numberOfSouls == 2){
			previousSoul = soulsInRotation.Last(); 
		}
        if (numberOfSouls == 3)
        {
            //max number of souls the player can have
            yield break;
        }
        else if (numberOfSouls == 0 || numberOfSouls == 1)
        {
			properTransform = DetermineWhichTransform();
			properPosition = properTransform.position;
            //this method should take the new soul where the player picks it up and have it dragged toward an unclaimed position around the player
            while (Vector2.Distance(soul.transform.position, properPosition) > 0.1f)
            {
                soul.transform.position = Vector2.MoveTowards(soul.transform.position, properPosition, 5.0f * Time.deltaTime);
                yield return null;
            }
        }
		else if(numberOfSouls == 2){

			properTransform = DetermineWhichTransform();
			properPosition = properTransform.position;
			transformToMakeParentOfLastSoul = soulTransformParentage_.Last().Value;
			positionToMoveLastSoulTo = transformToMakeParentOfLastSoul.position;
            while (Vector2.Distance(soul.transform.position, properPosition) > 0.1f)
            {
                soul.transform.position = Vector2.MoveTowards(soul.transform.position, properPosition, 5.0f * Time.deltaTime);
				previousSoul.transform.parent = null;
				previousSoul.transform.position = Vector2.MoveTowards(soul.transform.position, positionToMoveLastSoulTo, 5.0f * Time.deltaTime);
                yield return null;

			}

		}
       	soul.transform.parent = properTransform;
		   if(numberOfSouls == 2){
			   previousSoul.transform.parent = transformToMakeParentOfLastSoul;
		   }
		soulsInRotation.Add(soul);
        numberOfSouls++;

    }

    Transform DetermineWhichTransform()
    {
        Transform correctTransform = null;
        if (numberOfSouls == 0)
        {
            correctTransform = soulTransformParentage_["TopPosition"];
        }
        else if (numberOfSouls == 1)
        {
            correctTransform = soulTransformParentage_["BottomMiddlePosition"];
        }
		else if(numberOfSouls == 2){
			correctTransform = soulTransformParentage_["BottomLeftPosition"];
		}
        return correctTransform;
    }

    int maxSoulsInStream;
    #region 


    void SendParticlesOut()
    {

    }

    void RotateParticles()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
    #endregion
    void TravelTowardCenter()
    {

    }

    void TravelBackOut()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }




}
