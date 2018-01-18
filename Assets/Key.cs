using UnityEngine;
using System.Collections;
using System;
public class Key : MonoBehaviour, IPullable, iWillFollow
{
    Rigidbody2D rb;
    CircleCollider2D ourCircleCollider;
    public bool hookBroken;
    public static event Action KeyGrabbedByPlayer;
      float snapSpeed;
    float stopDistance;
    bool Attached = false;
    public ParticleSystem appearanceParticleSystem;

  public void FollowAlong(GameObject ourTarget)
    {
        ourCircleCollider.enabled = false;
        Vector3 ourPosition = transform.position;

        Vector3 ourTargetsPosition = ourTarget.transform.position;
        float ourTargetsPositionX = ourTargetsPosition.x;
        float ourPositionX = ourPosition.x;
        if (ourTargetsPositionX < ourPositionX)
        {
            if (Vector3.Distance(ourTargetsPosition, ourPosition) < 2.0f)
            {
                 rb.velocity = Vector2.zero;
            }
            else
            {
                Vector2 targetDirection = (Vector2)Vector3.Normalize(ourTargetsPosition - ourPosition);
                 rb.velocity = new Vector2(targetDirection.x * snapSpeed, targetDirection.y * snapSpeed);
            }
            //FlipLeft();
        }
        else if (ourTargetsPositionX > ourPositionX)
        {


            if (Vector3.Distance(ourTargetsPosition, ourPosition) < 2.0f)
            {
                 rb.velocity = Vector2.zero;
            }
            else
            {
                Vector2 targetDirection = (Vector2)Vector3.Normalize(ourTargetsPosition - ourPosition);
                 rb.velocity = new Vector2(targetDirection.x * snapSpeed, targetDirection.y * snapSpeed);
            }
            //FlipRight();
        }

    }
    public void KeyGrabbed()
    {
        Attached = true;
        if(KeyGrabbedByPlayer != null)
        {
            Debug.Log("The key's been grabbed");
            KeyGrabbedByPlayer();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D hit)
    {
       if(hit.gameObject.tag == "Player")
        {
            //Debug.Log("Player grabbed the key!");
            KeyGrabbed();
        }
    }
    public void CancelPull()
    {
        hookBroken = true;
    }


    public void BeginPull(Transform target)
    {
        StartCoroutine(PullMeForward(target));
    }

    public IEnumerator PullMeForward(Transform target)
    {
        Debug.Log("Key is being pulled toward " + target);

        Vector2 zipLocation = target.position;
        Vector2 trans = zipLocation - (Vector2)transform.position;
        trans.Normalize();
        rb.bodyType = RigidbodyType2D.Kinematic;
        while (Mathf.Abs(Vector2.Distance(transform.position, target.position)) > stopDistance)
        {
           
            if (hookBroken)
            {
                 break;
            }
            rb.velocity = trans * snapSpeed;

            yield return null;
        }
        ourCircleCollider.enabled = false;
        rb.velocity = new Vector2(0.0f, 0.0f);
        Attached = true; 
       // rb.bodyType = RigidbodyType2D.Dynamic;
            
    }
    private void Awake()
    {
        ourCircleCollider = GetComponent<CircleCollider2D>();
         stopDistance = 3.0f;
        snapSpeed = 10.0f; 
        rb = GetComponent<Rigidbody2D>();
        appearanceParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        appearanceParticleSystem.Play(); 
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Attached)
        {
            FollowAlong(GameStateHandler.player);
        }

    }
}
