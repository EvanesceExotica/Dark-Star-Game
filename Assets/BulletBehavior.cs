using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BulletBehavior : PooledObject {

    public float speed;
    Rigidbody2D rb;
    public float maxDistance;
    GameObject player;
    Vector2 playerPosition;
    public GameObject burstGameObject;
    List<ParticleSystem> burstParticles;

    bool collided;

    private void Awake()
    {
        speed = 10.0f;
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        burstParticles = burstGameObject.GetComponentsInChildren<ParticleSystem>().ToList();
    }
    // Use this for initialization
    
	// Use this for initialization
	void Start () {
        maxDistance = 10.0f;
	}

    void OnEnable()
    {
    }

    public void Fly(Vector2 mousePosition)
    {
        StartCoroutine(FlyCoroutine(mousePosition));
    }

    public IEnumerator FlyCoroutine(Vector2 mousePos)
    {
        playerPosition = player.transform.position;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        Vector2 trans = mousePositionWorld - playerPosition;
        trans.Normalize();


        while ((Vector2.Distance(playerPosition, transform.position)) <= maxDistance)
        {
            //if (collided)
            //{
            //    break;
            //}
            
            rb.velocity = trans * speed;
            var dir = (Vector3)mousePositionWorld - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


            yield return null;

        }
        rb.velocity = new Vector2(0, 0);
        Burst();
        ReturnToPool();

    }

    void Burst()
    {
        //TODO: Make this a pool too silly billy
        GameObject burst = Instantiate(burstGameObject, transform.position, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
       // rb.MovePosition(transform.position + transform.right * Time.deltaTime * speed);

    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        Enemy enemy = hit.GetComponent<Enemy>();
        if(hit.GetComponent<Enemy>() != null)
        {
            collided = true;
            enemy.ourMovement.Stun(player);
            Burst();
            ReturnToPool();
        }
        
    }
}
