using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoulRotateScript : MonoBehaviour
{

	public PlayerReferences playerReferences;
	List<Transform> soulPositions = new List<Transform>();
	List<GameObject> soulGameObjects = new List<GameObject>();

#region 
    public List<ParticleSystem> chargeParticlesystem;
    public bool rotate;
    bool particlesBreathedIn;
    public ParticleSystem ourParticleSystem;
    public Vector3 rotationSpeed;
#endregion
    public float drawInSpeed;
    public float drawOutSpeed;
    GameObject player;

    ParticleSystem.Particle[] soulParticles = new ParticleSystem.Particle[40];
    void Awake()
    {
        ourParticleSystem = GetComponent<ParticleSystem>();
        chargeParticlesystem = GetComponentsInChildren<ParticleSystem>().ToList();
        player = GameObject.Find("Player");
		playerReferences = GetComponentInParent<PlayerReferences>();
		foreach(Transform child in transform){
			soulPositions.Add(child);
		}
    }
    int maxSoulsInStream;
	#region 
    void AddSoul()
    {
        var emission = ourParticleSystem.emission;
        emission.rateOverTime = emission.rateOverTime.constant + 1.0f;
    }

    void SubtractSoul()
    {

        var emission = ourParticleSystem.emission;
        emission.rateOverTime = emission.rateOverTime.constant - 1.0f;
    }
    // Use this for initialization
    void Start()
    {
        GetParticles();
        drawInSpeed = 8;
    }

    void GetParticles()
    {
        var particleCount = ourParticleSystem.particleCount;
        if (soulParticles.Length < particleCount)
        {
            soulParticles = new ParticleSystem.Particle[particleCount];
        }
        ourParticleSystem.GetParticles(soulParticles);

    }

    public IEnumerator DrawInParticles()
    {
        Debug.Log("Drawing in particles");
        particlesBreathedIn = true;
        ParticleSystem.MainModule psMain = ourParticleSystem.main;
        psMain.loop = false;
        //psMain.startLifetime = 7.0f;
        float startTime = Time.time;
        for (int i = 0; i < soulParticles.Length; i++)
        {
            var particle = soulParticles[i];
            particle.remainingLifetime += 1.0f;
        }
        for (float t = 0f; t < 10f; t += 0.1f)
        {

            GetParticles();
            Debug.Log(" We hsould be darwing them in now" + soulParticles.Length);
            for (int i = 0; i < soulParticles.Length; i++)
            {
                Vector3 targetPosition = transform.InverseTransformPoint(transform.position);
                Debug.Log("inside the for loop");
                float distance = Vector2.Distance(targetPosition, soulParticles[i].position);
                ParticleSystem.Particle particle = soulParticles[i];
                particle.position = Vector3.Lerp(particle.position, targetPosition, t);
                // particle.remainingLifetime = particle.remainingLifetime + 1.0f;
                // particle.startLifetime = particle.startLifetime + 1.0f;
            }
            //	particle.position = transform.InverseTransformPoint(particle.position);
            ourParticleSystem.SetParticles(soulParticles, soulParticles.Length);
            ourParticleSystem.Stop();
            yield return null;
        }

        particlesBreathedIn = false;
    }

    public IEnumerator DrawSingleParticleIn()
    {
        GetParticles();
        Vector3 targetPosition = transform.InverseTransformPoint(transform.position);
        float distance = Vector2.Distance(targetPosition, soulParticles[0].position);
        while (Vector2.Distance(targetPosition, soulParticles[0].position) > 0.2f)
        {
            ParticleSystem.Particle particle = soulParticles[0];
            particle.position = Vector3.Lerp(particle.position, targetPosition, Time.deltaTime * 4/*Time.deltaTime / 1.0f*/);
            soulParticles[0] = particle;
            ourParticleSystem.SetParticles(soulParticles, soulParticles.Length);

			yield return null;
        }

		SubtractSoul();
		ParticleSystemPlayer.PlayChildParticleSystems(chargeParticlesystem);
    }

    bool subtractedSoul = false;
    void DrawInSingleParticle()
    {
        //Todo: make this a couritne or something 
        GetParticles();
        Vector3 targetPosition = transform.InverseTransformPoint(transform.position);
        float distance = Vector2.Distance(targetPosition, soulParticles[0].position);
        if (distance < 0.2f)
        {
            particlesBreathedIn = false;
        }
        ParticleSystem.Particle particle = soulParticles[0];
        particle.position = Vector3.Lerp(particle.position, targetPosition, Time.deltaTime / 1.0f);
        // particle.remainingLifetime = particle.remainingLifetime + 1.0f;
        // particle.startLifetime = particle.startLifetime + 1.0f;
        //	particle.position = transform.InverseTransformPoint(particle.position);
        soulParticles[0] = particle;
        ourParticleSystem.SetParticles(soulParticles, soulParticles.Length);
        if (!subtractedSoul)
        {
            SubtractSoul();
            ParticleSystemPlayer.PlayChildParticleSystems(chargeParticlesystem);
            subtractedSoul = true;
        }
    }
    void DrawParticlesIn()
    {

        GetParticles();
        if (particlesBreathedIn == false)
        {

            for (int i = 0; i < soulParticles.Length; i++)
            {


                soulParticles[i].remainingLifetime += 5.0f;
            }
        }
        //rotate = false;
        particlesBreathedIn = true;
        ParticleSystem.MainModule psMain = ourParticleSystem.main;
        //psMain.startLifetime = 7.0f;
        psMain.loop = false;

        for (int i = 0; i < soulParticles.Length; i++)
        {
            //You had a problem here because this wasn't in localSpace. The particles were heading toward the world space position (0, 0, 0) most likely. You had to transform from world to local.
            Vector3 targetPosition = transform.InverseTransformPoint(transform.position);
            float distance = Vector2.Distance(targetPosition, soulParticles[i].position);
            ParticleSystem.Particle particle = soulParticles[i];
            particle.position = Vector3.Lerp(particle.position, targetPosition, 2*Time.deltaTime / 1.0f);
            // particle.remainingLifetime = particle.remainingLifetime + 1.0f;
            // particle.startLifetime = particle.startLifetime + 1.0f;
            //	particle.position = transform.InverseTransformPoint(particle.position);
            soulParticles[i] = particle;
            // if(distance > 0.2f){
            // 	ParticleSystem.Particle particle = soulParticles[i];
            // 	particle.position = Vector3.Lerp(particle.position, transform.position, Time.deltaTime / 2.0f);
            // //	particle.position = transform.InverseTransformPoint(particle.position);
            // 	soulParticles[i] = particle;
            // }
            //     Vector3 startingPosition = soulParticles[i].position;

            //     Vector3 targetPosition = player.transform.position;

            //    Vector2 trans = targetPosition - startingPosition;
            //     trans.Normalize();
            //     soulParticles[i].velocity = (trans * drawInSpeed);


        }
        ourParticleSystem.SetParticles(soulParticles, soulParticles.Length);
        ourParticleSystem.Stop();
    }

    void ParticlesSpiralInward()
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ourParticleSystem.velocityOverLifetime;

        //	velocityOverLifetimeModule.x
    }



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
		#region 
        // if (rotate)
        // {
        //     RotateParticles();
        // }
        // if (particlesBreathedIn)
        // {
        //    // DrawInSingleParticle();
        //     //DrawParticlesIn();
        // }
        // //RotateParticles();
        // if (!particlesBreathedIn)
        // {
        //     // RotateParticles();
        // }
		#endregion
        if (Input.GetKeyDown(KeyCode.J) && !particlesBreathedIn)
        {
		//	StartCoroutine(DrawSingleParticleIn());
           // particlesBreathedIn = true;
            //DrawParticlesIn();
            // StartCoroutine(DrawInParticles());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddSoul();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SubtractSoul();
        }
    }




}
