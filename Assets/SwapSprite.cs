using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSprite : MonoBehaviour {

    public Sprite sprite1;
    public Sprite sprite2;
    SpriteRenderer ourSpriteRenderer;
    public GameObject target;
    public Transform parent;
    public bool facingRight;
    public bool moving;

	// Use this for initialization
	void Start () {

        facingRight = true;
        ourSpriteRenderer = GetComponent<SpriteRenderer>();
        sprite1 = ourSpriteRenderer.sprite;
        parent = transform.parent;
	}
	
    public void Swap()
    {
        ourSpriteRenderer.sprite = sprite2;
    }

    void FlipLeft()
    {
        ourSpriteRenderer.flipX = true;
        facingRight = false;
    }

    void FlipRight()
    {
        ourSpriteRenderer.flipX = false;
        facingRight = true;
    }

	// Update is called once per frame
	void Update () {
        if (moving)
        {
            if(target.transform.position.x > parent.transform.position.x && !facingRight)
            {
                facingRight = true;
            }
            else if(target.transform.position.x < parent.transform.position.x && facingRight)
            {
                facingRight = false;
            }
        }
		
	}
}
