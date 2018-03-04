using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeamHandler : PowerUp {
    public LineRenderer ourLineRenderer;
    public PlayerReferences playerReferences;
    public float laserDamage;
//tyopw
    public GameObject start;
    public GameObject end;
    private List<ParticleSystem> laserStartParticles;
    public List<ParticleSystem> laserEndParticles;

    public LayerMask enemyLayermask;

    float minWidth;
    float maxWidth;

    float duration;
    float startTime;

    GameObject player;

    float radius = 20.0f;

    Action<Vector2, float, Vector2, float> BeamShooting;

    bool shootingLaser;

    public List<GameObject> objectsBeingDamagedByLaser = new List<GameObject>();
    public List<ParticleSystem> LaserStartParticles; 
   
    void Awake()
    {
        ourLineRenderer = gameObject.GetComponent<LineRenderer>();

        ChoosePowerUp.laserChosen += this.SetPoweredUp;

        ourLineRenderer.enabled = false;
        player = GameObject.FindWithTag("Player");
        playerReferences = player.GetComponent<PlayerReferences>();
        Switch.SwitchEntered += this.SetOnSwitch;
        Switch.SwitchExited += this.SetOffSwitch;
        autoActivated = false;
        ourRequirement = Requirement.OnlyUseOnSwitch;

        LaserStartParticles = start.GetComponentsInChildren<ParticleSystem>().ToList();
        laserEndParticles = end.GetComponentsInChildren<ParticleSystem>().ToList();
    }
    // Use this for initialization



    public override void StartPowerUp()
    {
        shootingLaser = true;
        ourLineRenderer.enabled = true;
        ParticleSystemPlayer.PlayChildParticleSystems(LaserStartParticles);
        ParticleSystemPlayer.PlayChildParticleSystems(laserEndParticles);
        ourLineRenderer.SetPosition(0, player.transform.position);
        ourLineRenderer.SetPosition(1, new Vector2(player.transform.position.x + 10.0f, player.transform.position.y));
        ourLineRenderer.startWidth = 0.0f;
        ourLineRenderer.endWidth = 0.0f;
        minWidth = 7.5f;
        maxWidth = 8.0f;
        duration = 5.0f;
        start = transform.Find("LaserStart").gameObject;
        end = transform.Find("LaserEnd").gameObject;

        StartCoroutine(FlickerLaser());
    }
    private void OnEnable()
    {
        laserDamage = -1;
        //ourLineRenderer.SetPosition(0, player.transform.position);
        //ourLineRenderer.SetPosition(1, new Vector2(player.transform.position.x + 10.0f, player.transform.position.y));
        //ourLineRenderer.startWidth = 0.0f;
        //ourLineRenderer.endWidth = 0.0f;
        //minWidth = 7.5f;
        //maxWidth = 8.0f;
        //duration = 5.0f;
        //start = transform.Find("LaserStart").gameObject;
        //end = transform.Find("LaserEnd").gameObject;

        //StartCoroutine(FlickerLaser());
    }


	public IEnumerator GrowLaser()
    {
        
        while (ourLineRenderer.startWidth < maxWidth)
        {
            ourLineRenderer.startWidth = Mathf.Lerp(ourLineRenderer.startWidth, maxWidth, 5.0f * Time.deltaTime);
            if (ourLineRenderer.startWidth >= maxWidth - 0.1f)
            {
                break;
            }
            ourLineRenderer.endWidth = Mathf.Lerp(ourLineRenderer.endWidth, maxWidth, 5.0f * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator FlickerLaser()
    {
        yield return StartCoroutine(GrowLaser());
        Debug.Log("Now we've started lasering");
        startTime = Time.time;
        StartCoroutine(CastCircle());
        while (Time.time < startTime + duration)
        {
            float randomWidth = UnityEngine.Random.Range(minWidth, maxWidth);
            ourLineRenderer.startWidth = randomWidth;
            ourLineRenderer.endWidth = randomWidth;
            yield return null;
        }
        StartCoroutine(ShrinkLaser());
    }

    public IEnumerator ShrinkLaser()
    {
        while (ourLineRenderer.startWidth > 0)
        {
            ourLineRenderer.startWidth = Mathf.Lerp(ourLineRenderer.startWidth, 0.0f, 2.0f * Time.deltaTime);
            if (ourLineRenderer.startWidth <= 0 + 0.1f)
            {
                break;
            }
            ourLineRenderer.endWidth = Mathf.Lerp(ourLineRenderer.endWidth, 0.0f, 2.0f * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
        playerReferences.playerSoulHandler.Depowered();
        
    }

    Vector2 LimitPosition(Vector2 center, float limit)
    {
        //this limits the position of the indicator to inside the spell's radius 
        Vector2 centerOfCircle = (Vector2)center;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = (Vector2)((Vector2)transform.position - centerOfCircle);
        Vector2 ourPosition = ourLineRenderer.GetPosition(0);

        Vector2 offSet = ((Vector2)mousePosition - centerOfCircle);

        float distance = offSet.magnitude;
        
            if (limit < distance)
            { //if the indicator has passed the radius, place it back where it should be
                Vector2 direction = offSet / distance;
                ourPosition = centerOfCircle + direction * radius;
            }
            else if (distance < limit)
            { //if the distance is less than the inner radius, place it back where it should be
                Vector2 direction = offSet / distance;
                ourPosition = centerOfCircle + direction * limit;
            
            }

            else
            {
                ourPosition = mousePosition;
            }
        return ourPosition;
       
    }

    public IEnumerator CastCircle()
    {
        while (Time.time < startTime + duration)
        {
            Vector2 startPoint = player.transform.position;
            Vector2 endPoint = ourLineRenderer.GetPosition(1);
            float width = ourLineRenderer.startWidth;
            float distance = radius;

            Vector2 trans = endPoint - startPoint;
            trans.Normalize();
            RaycastHit2D[] ourRayCastHitArray = Physics2D.CircleCastAll(startPoint, width, trans, distance, enemyLayermask);
            Debug.DrawRay(startPoint, trans * distance, Color.blue, 10.0f);
            foreach (RaycastHit2D hit in ourRayCastHitArray)
            {
                if(hit){
                    Debug.Log(hit.collider.gameObject.name + " was hit by laser");
                }
                IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
                if (damageableObject != null)
                {
                    
                    Debug.Log(hit.collider.gameObject.name + " was damaged by laser");
                   // damageableObject.AddPersistentDamageSource((int)laserDamage, duration, 1.0f, transform.parent.gameObject);
                    damageableObject.adjustCurrentHealth((int)laserDamage, this.gameObject);
                }
            }
            yield return new WaitForSeconds(0.5f);

        }
    }

    private void Reset()
    {
        ourLineRenderer.startWidth = 0f;
        ourLineRenderer.endWidth = 0f;
        startTime = 0f;
        laserDamage = -1.0f;
    }


    // void BeginFiring(){
    //     if(onSwitch){
    //         StartShootingLaser();
    //     }
    // }
    // Update is called once per frame
    public override void Update () {
        base.Update();

        

        if (shootingLaser)
        {
            ourLineRenderer.SetPosition(1, LimitPosition(player.transform.position, radius));
            ourLineRenderer.SetPosition(0, player.transform.position);

            start.transform.position = player.transform.position;
            end.transform.position = ourLineRenderer.GetPosition(1);
        }
		
	}
}
