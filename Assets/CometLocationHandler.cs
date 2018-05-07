using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CometLocationHandler : MonoBehaviour
{

    // Use this for initialization

    public LayerMask darkStarLayer;
    public CircleCollider2D ourCollider;

    bool stillBeingBirthedFromStar;

    bool normalFunctionAssumed;
    float colliderRadius;
    public event Action LeftStarAfterBirth;
    void Awake()
    {
        ourCollider = GetComponent<CircleCollider2D>();
        ourCollider.enabled = false;
        colliderRadius = ourCollider.bounds.extents.x;
    }

    void TurnOnCollider2D()
    {
        ourCollider.enabled = true;
		normalFunctionAssumed = true;
    }
    // Update is called once per frame
    void Update()
    {

        if (!normalFunctionAssumed)
        {
            stillBeingBirthedFromStar = Physics2D.OverlapCircle(transform.position, colliderRadius, darkStarLayer);
            if (!stillBeingBirthedFromStar)
            {
				TurnOnCollider2D();
            }
        }

    }
}
