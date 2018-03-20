using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DarkStar : MonoBehaviour
{

    public GameObject standardStarParticlesGO;

    public GameObject starTooBigGO;

    public GameObject starUltraNovaGO;

    public List<GameObject> particleSystemGameObjectsToScale;
    public static Color warningColor;
    public static Color doomColor;
    public static event Action DarkStarIsGrowing;

    _2dxFX_Distortion_Additive distortionEffect;

    void StartDistortionEffect(){
        distortionEffect.enabled = true;
    }

    void StopDistortionEffect(){

        distortionEffect.enabled = false;
    }
    public void DarkStarGrowing()
    {
        if (DarkStarIsGrowing != null)
        {
            DarkStarIsGrowing();
        }
    }

    public static event Action DarkStarIsStable;

    public void DarkStarStable()
    {
        if (DarkStarIsStable != null)
        {
            DarkStarIsStable();
        }
    }
    #region //Dark Star Variables

    public Sprite redGiantSprite;
    float maxStarRadius;

    public static Vector2 position;
    public static float radius;
    public static CircleCollider2D area;

    public float minimumIllumination;
    _2dxFX_Hologram3 ourDarkStarHurtEffect;

    bool dangerMode;
    int dangerThreshold;

    public EatenBurst blastPrefab;
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
    
    List<ParticleSystem> burstParticleSystem = new List<ParticleSystem>();
    _2dxFX_BlackHole blackHoleEffect;
    PointEffector2D blackHoleForce;

    DarkStarAnimations ourAnimations;

    public static event Action<float> AugmentDoomTimer;

    Sprite DarkStarSprite;

    public GameObject openStarGO;
    List<ParticleSystem> openStarParticles = new List<ParticleSystem>();

    bool playerDiedDuringOvercharge;

    bool overcharged;
    #endregion


    public static void AugmentTimer(float penalty)
    {
        if (AugmentDoomTimer != null)
        {
            AugmentDoomTimer(penalty);
        }
    }

    bool extinguishing;

    
    private void Awake()
    {
        distortionEffect = GetComponent<_2dxFX_Distortion_Additive>();
        AdjustRadius();
        warningColor = new Color32(255, 0, 63, 255);
       particleSystemGameObjectsToScale.Add(standardStarParticlesGO) ;
       particleSystemGameObjectsToScale.Add(starTooBigGO);
       particleSystemGameObjectsToScale.Add(starUltraNovaGO);

        doomColor = Color.red;
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        illuminationLossLap = 30.0f;
        holdMaxIlluminationDuration = 30.0f;
        blackHoleEffect = GetComponent<_2dxFX_BlackHole>();
        blackHoleForce = GetComponent<PointEffector2D>();
        area = GetComponent<CircleCollider2D>();
        DarkStarSprite = GetComponent<SpriteRenderer>().sprite;
        //       openStarParticles = openStarGO.GetComponentsInChildren<ParticleSystem>().ToList();
        ourAnimations = GetComponent<DarkStarAnimations>();
        // maxIlluminationParticles = maxIlluminationGO.GetComponentsInChildren<ParticleSystem>().ToList();

        //here we're connecting it so that when it touches the rim of being too big, it starts a countdown
        DarkStarTooBig.DarkStarReachedTooLargeBounds += BeginOvercharge;
        DarkStarTooBig.DarkStarReceeded += this.CancelOvercharge;
        DarkStarTooBig.DarkStarReachedTooLargeBounds += StartDistortionEffect;
        DarkStarTooBig.DarkStarReceeded += this.StopDistortionEffect;
    }



    int Illumination
    {
        get
        {
            return illumination;
        }

        set
        {
            illumination = value;
        }
    }

    public static event Action<float> AdjustLuminosity;
    public static event Action<float> IlluminationAtMax;
    public static event Action IlluminationAtZero;
    public static event Action LostMaxIllumination;
    public static event Action Overcharged;

    bool overcharging = false;

    void BeginOvercharge()
    {
        overcharged = true;
        StartCoroutine(SuperNova());
    }

    void CancelOvercharge()
    {
        overcharged = false;
        playerDiedDuringOvercharge = false;
    }

    void MaxIlluminationLost()
    {
        if (LostMaxIllumination != null)
        {
            LostMaxIllumination();
        }
    }

    private void Update()
    {

        showIllumination = illumination;
    }

    void AdjustRadius()
    {
        radius = GetComponent<CircleCollider2D>().bounds.extents.x;
    }

    public GameObject maxIlluminationGO;
    List<ParticleSystem> maxIlluminationParticles = new List<ParticleSystem>();

    void PlayMaxIlluminationParticleSystem()
    {
        ParticleSystemPlayer.PlayChildParticleSystems(maxIlluminationParticles);
    }

    public IEnumerator CountdownUntilStarBurst()
    {
        victoryCounterStarted = true;
        while (Time.time < illuminationAtMaxStartTime + 15.0f/*holdMaxIlluminationDuration*/)
        {
            //Debug.Log("We're starting to win");
            if (illumination < maxIllumination)
            {
                //Debug.Log("Does " + illumination + " equal " + maxIllumination);
                MaxIlluminationLost();
                yield break;
            }

            if (illumination == overchargeIlluminationMaxValue && overcharged)
            {
               // TODO: Figure this out. This causes the dark star to burst when you suck in too many enemies at once.
                //DarkStarOvercharged();
            }

            yield return null;
        }
        DarkStarOvercharged();
    }


    void DarkStarOvercharged()
    {
        if (Overcharged != null)
        {
            Overcharged();
        }

    }

    IEnumerator SuperNova()
    {
        float startTime = Time.time;
        while(Time.time < startTime + 15.0f){
            if(playerDiedDuringOvercharge){
                //if we were overcharged, and its signalled that the player died, the level should immediately end
                //this is to keep the player from suiciding in order to drop the star's size
                break;
            }
            yield return null;
        }
        DarkStarOvercharged();

    }

    IEnumerator ExtinguishStar()
    {
        //Debug.Log("WOOSH");
        //Play a bunch of animations, etc. Game over.

        yield return new WaitForSeconds(2.0f);

    }


    public void AdjustLuminosityAndHandleDeath(float adjustmentValue)
    {
        if(overcharged){
            playerDiedDuringOvercharge = true;
        }
        AdjustIllumination((int)adjustmentValue);
        if (AdjustLuminosity != null)
        {
            AdjustLuminosity(adjustmentValue);
        }
    }

    void MaxIllumination()
    {
        Debug.Log(" MAX ILLUMINATION STARTING COUNTDOWN TOWARD DOOM");
        if (IlluminationAtMax != null)
        {
            if (!victoryCounterStarted)
            {
                StartCoroutine(CountdownUntilStarBurst());
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
        EatenBurst blast = blastPrefab.GetPooledInstance<EatenBurst>();
        blast.transform.position = blastLocation;

    }



    public void AdjustIllumination(int illuminationAdjustmentValue)
    {
        //maybe add a limiter here for if it adds or removes health?
        if (illuminationAdjustmentValue > 0)
        {
            DarkStarGrowing();
        }
        illumination += illuminationAdjustmentValue;
        if (illumination < 0)
        {
            illumination = 0;
        }
        if (illumination <= 1)
        {

            MinIllumination();
        }
        if (illumination == maxIllumination)
        {

           // MaxIllumination();
        }

    }


    private void OnTriggerEnter2D(Collider2D hit)
    {

        IDigestible digestibleObject = hit.GetComponent<IDigestible>();
        Enemy enemy = hit.GetComponent<Enemy>();
        if (digestibleObject != null)
        {
            //  //Debug.Log(gameObject.name + " ate " + digestibleObject.ToString());
            Vector2 digestibleObjectPostion = hit.transform.position;
            digestibleObject.Deconstruct();
            DisplayBlast(digestibleObjectPostion);

            if (DoPhasesMatch(enemy.phaseWeNeedToMatch))
            {
                //Debug.Log("<color=green> THEY MATCH </color>");
            }
            else
            {
                //  ourAnimations.AnimateRejectionWrapper();
                //Debug.Log("<color=red> THEY DON'T MATCH! BAD STUFF! </color>");

                // ProCamera2DShake.Instance.Shake("PlayerHit");
                AugmentTimer(-15.0f);
                //trigger red flash and screenshake here for bad bad stuff
            }
            //   if(ShowDarkStarPhase.GetPhase() == ShowDarkStarPhase.DarkStarPhases.)
            AdjustLuminosityAndHandleDeath(digestibleObject.illuminationAdjustmentValue);
        }

    }



    bool DoPhasesMatch(ShowDarkStarPhase.DarkStarPhases enemyPhaseToMatch)
    {
        if (ShowDarkStarPhase.GetPhase() == enemyPhaseToMatch)
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
        PlayerHealth.PlayerDied += this.AdjustLuminosityAndHandleDeath;
    }

    private void OnDisable()
    {
        PlayerHealth.PlayerDied -= this.AdjustLuminosityAndHandleDeath;
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
