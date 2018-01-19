using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DarkStar : MonoBehaviour {

public static event Action DarkStarIsGrowing;

public void DarkStarGrowing(){
    Debug.Log("The Dark star is growing :o");
    if(DarkStarIsGrowing != null){
        DarkStarIsGrowing();
    }
}

public static event Action DarkStarIsStable;

public void DarkStarStable(){
    Debug.Log("The Dark star is stable. No worries.");
    if(DarkStarIsStable != null){
        DarkStarIsStable();
    }
}
    public static Vector2 position;
    public static float radius;
    public static CircleCollider2D area;

    public float minimumIllumination;
    _2dxFX_Hologram3 ourDarkStarHurtEffect;

    bool dangerMode;
    int dangerThreshold;

    public GameObject blastPrefab;
    PlayerHealth playerHealth;

    public static int illumination;

    [SerializeField]
    int showIllumination;

    [SerializeField]
    int maxIllumination;

    int overchargeIlluminationMaxValue;

    int startingIllumination;

    float illuminationAtMaxStartTime;
    float holdMaxIlluminationDuration;

    [SerializeField]
    bool victoryCounterStarted;


    float illuminationLossLap;
    int illuminationToLose;

    _2dxFX_BlackHole blackHoleEffect;
    PointEffector2D blackHoleForce;

    DarkStarAnimations ourAnimations;

    public static event Action<float> AugmentDoomTimer;

    Sprite DarkStarSprite;

    void OpenStar()
    {
        
        blackHoleEffect.enabled = true;
    }

    public static void AugmentTimer(float penalty)
    {
        if(AugmentDoomTimer != null)
        {
            AugmentDoomTimer(penalty);
        }
    }

    bool extinguishing; 

    private void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        illuminationLossLap = 30.0f;
        holdMaxIlluminationDuration = 30.0f;
        blackHoleEffect = GetComponent<_2dxFX_BlackHole>();
        blackHoleForce = GetComponent<PointEffector2D>();
        area = GetComponent<CircleCollider2D>();
        DarkStarSprite = GetComponent<SpriteRenderer>().sprite;

        ourAnimations = GetComponent<DarkStarAnimations>();
    }



    int Illumination
    {
        get { return illumination;
        }

        set {
            illumination = value; 
        }
    }

    public static event Action<float> AdjustLuminosity;
    public static event Action<float> IlluminationAtMax;
    public static event Action IlluminationAtZero;
    public static event Action LostMaxIllumination;
    public static event Action Overcharged;

    bool overcharging = false;

    void DarkStarOvercharged()
    {
        if (!overcharging)
        {
            //Debug.Log("Starting supernova");
            StartCoroutine(SuperNova());
            if (Overcharged != null)
            {
                Overcharged();
            }
        }
    }

    void MaxIlluminationLost()
    {
        if(LostMaxIllumination != null)
        {
            LostMaxIllumination();
        }
    }

    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    MaxIllumination();
        //}
        showIllumination = illumination;
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    AdjustLuminosityWrapper(5.0f);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    AdjustLuminosityWrapper(-5.0f);
        //}
    }

    void AdjustRadius()
    {
        radius = GetComponent<CircleCollider2D>().bounds.extents.x;
    }

    public IEnumerator StartVictoryCounter()
    {
        victoryCounterStarted = true;
        while(Time.time < illuminationAtMaxStartTime + holdMaxIlluminationDuration)
        {
            //Debug.Log("We're starting to win");
            if(illumination < maxIllumination)
            {
                //Debug.Log("Does " + illumination + " equal " + maxIllumination);
                MaxIlluminationLost(); 
                yield break;
            }

            if(illumination == overchargeIlluminationMaxValue)
            {
                DarkStarOvercharged();
            }

            yield return null;
        }
        victoryCounterStarted = false;
        GameStateHandler.CompleteLevel();
    }


    

    IEnumerator SuperNova()
    {
        overcharging = true; //fix these booleans
        //Debug.Log("BOOOOM");
        yield return new  WaitForSeconds(2.0f);
        blackHoleForce.enabled = true;
        blackHoleEffect.enabled = true;
        
    }

    IEnumerator ExtinguishStar()
    {
        //Debug.Log("WOOSH");
        //Play a bunch of animations, etc. Game over.

        yield return new WaitForSeconds(2.0f);

    }

    
    public void AdjustLuminosityWrapper(float adjustmentValue)
    {
        AdjustIllumination((int)adjustmentValue);
        if(AdjustLuminosity != null)
        {
            AdjustLuminosity(adjustmentValue); 
        }
    }

    void MaxIllumination()
    {
        if(IlluminationAtMax != null)
        {
            if (!victoryCounterStarted)
            {
                StartCoroutine(StartVictoryCounter());
            }

            IlluminationAtMax(holdMaxIlluminationDuration);
        }
    }

    void MinIllumination()
    {

        StartCoroutine(ExtinguishStar());
        if (IlluminationAtZero != null)
        {
       
            
            IlluminationAtZero();


        }
    }

    void DisplayBlast(Vector2 blastLocation)
    {
        GameObject blast = Instantiate(blastPrefab) as GameObject;
        blast.transform.position = blastLocation;
    }



    public  void AdjustIllumination(int illuminationAdjustmentValue)
    {
        //maybe add a limiter here for if it adds or removes health?
        if(illuminationAdjustmentValue > 0){
            DarkStarGrowing();
        }
        illumination += illuminationAdjustmentValue;
        if (illumination < 0)
        {
            illumination = 0;
        }
        if (illumination <= 1) {

            MinIllumination(); 
        }
        if (illumination == maxIllumination)
        {
           
            MaxIllumination();
        }
        
    }


    private void OnTriggerEnter2D(Collider2D hit)
    {
        
        IDigestible digestibleObject = hit.GetComponent<IDigestible>();
        Enemy enemy = hit.GetComponent<Enemy>();
        if(digestibleObject != null)
        {
          //  //Debug.Log(gameObject.name + " ate " + digestibleObject.ToString());
            Vector2 digestibleObjectPostion = hit.transform.position;
            //TODO: FIX THIS VERY QUICKLY
            digestibleObject.Deconstruct();
            DisplayBlast(digestibleObjectPostion);

            if (DoPhasesMatch(enemy.phaseWeNeedToMatch))
            {
                //Debug.Log("<color=green> THEY MATCH </color>");
            }
            else
            {
                ourAnimations.AnimateRejectionWrapper();
                //Debug.Log("<color=red> THEY DON'T MATCH! BAD STUFF! </color>");

               // ProCamera2DShake.Instance.Shake("PlayerHit");
                AugmentTimer(-15.0f);
                //trigger red flash and screenshake here for bad bad stuff
            }
         //   if(ShowDarkStarPhase.GetPhase() == ShowDarkStarPhase.DarkStarPhases.)
            AdjustLuminosityWrapper(digestibleObject.illuminationAdjustmentValue); 
        }
        
    }



    bool DoPhasesMatch(ShowDarkStarPhase.DarkStarPhases enemyPhaseToMatch)
    {
        if(ShowDarkStarPhase.GetPhase() == enemyPhaseToMatch)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator LoseIlluminationOverTime()
    {
        yield return new WaitForSeconds(illuminationLossLap);
        while (true)
        {
           // AdjustLuminosityWrapper(illuminationToLose);
            yield return new WaitForSeconds(illuminationLossLap);
        }
    }


    private void OnEnable()
    {
        playerHealth.PlayerDied += this.AdjustLuminosityWrapper; 
    }

    private void OnDisable()
    {
        playerHealth.PlayerDied -= this.AdjustLuminosityWrapper;
    }

    private void Start()
    {
        maxIllumination = 10;
        startingIllumination = 5;
        illumination = startingIllumination;
        illuminationToLose = -2;
        overchargeIlluminationMaxValue = 15;
       // StartCoroutine(LoseIlluminationOverTime());

    }
}
