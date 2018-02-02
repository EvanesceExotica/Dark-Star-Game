using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullMe : MonoBehaviour, IPullable
{
    public float stopDistance = 3.0f;
    public float snapSpeed = 10.0f; //scale with distance? 

    public bool beingPulled;

    public bool hookBroken;
    public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void BeginPull(Transform target)
    {
        StartCoroutine(PullMeForward(target));

    }
    public IEnumerator PullMeForward(Transform target)
    {
        Vector2 zipLocation = target.position;
        Vector2 trans = zipLocation - (Vector2)transform.position;
        trans.Normalize();
        rb.bodyType = RigidbodyType2D.Kinematic;
        beingPulled = true;
        while (Mathf.Abs(Vector2.Distance(transform.position, target.position)) > stopDistance)
        {
            if (hookBroken)
            {
                break;
            }
            rb.velocity = trans * snapSpeed;

            yield return null;
        }
        beingPulled = false;
        rb.velocity = new Vector2(0.0f, 0.0f);
        // rb.bodyType = RigidbodyType2D.Dynamic;

    }
    public void CancelPull()
    {
        hookBroken = true;
    }
    // Use this for initialization

}
