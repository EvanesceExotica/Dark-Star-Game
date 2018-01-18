using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBullet : PooledObject {

    public float speed;
    Rigidbody2D rb;
    public float maxDistance;
    GameObject player;
    Vector2 playerPosition;
    Light ourLight;
    bool collided;

    private void Awake()
    {
        speed = 10.0f;
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        ourLight = gameObject.GetComponent<Light>();
    }
    // Use this for initialization

    // Use this for initialization
    void Start()
    {
        maxDistance = 3.0f;
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


        while (Mathf.Abs((Vector2.Distance(playerPosition, transform.position))) <= maxDistance)
        {
            if (collided)
            {
                break;
            }

            rb.velocity = trans * speed;
            var dir = (Vector3)mousePositionWorld - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


            yield return null;

        }

        StartCoroutine(Illuminate()); 

    }

    // Update is called once per frame
    public IEnumerator Illuminate()
    {
        ourLight.enabled = true;
        yield return new WaitForSeconds(5.0f);
        ReturnToPool();

    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        collided = true;
        StartCoroutine(Illuminate());
    }
}
