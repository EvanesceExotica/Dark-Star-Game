using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : MonoBehaviour {

    CircleCollider2D bashRadius;
    public PlayerReferences pReference;
    public bool bashing;
    public bool canBash;
    float bashStartTime;
    float bashMaxDuration;
    public float bashForce;
    float minimumBashForce;
    public GameObject objectToBash;
    public GameObject energyBallPrefab;

    KeyCode bashKeyCode;
    public List<GameObject> bashableObjectsInRange = new List<GameObject>(); 

	// Use this for initialization
	void Start () {

        bashKeyCode = KeyCode.L;
        pReference = gameObject.GetComponentInParent<PlayerReferences>(); 
        bashMaxDuration = 5.0f;
        minimumBashForce = 2000;
        bashForce = minimumBashForce;
	}

    // Update is called once per frame
    void Update() {

        if (canBash)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                StartBash(objectToBash);
            }
        }
        if(bashableObjectsInRange.Count > 0)
        {
            canBash = true;
        }
        else if(bashableObjectsInRange.Count == 0)
        {
            canBash = false;
        }
        if(bashableObjectsInRange.Count == 1)
        {
            objectToBash = bashableObjectsInRange[0];
        }
		else if(bashableObjectsInRange.Count > 1)
        {

            objectToBash = FindClosest.FindClosestObject(bashableObjectsInRange, this.gameObject);
            
        }
        
	}

    void StartBash(GameObject bashObject)
    {
        StartCoroutine(BashOut(bashObject));
    }

    public IEnumerator BashOut(GameObject bashObject)
    {
        Rigidbody2D bashRigidbody = bashObject.GetComponent<Rigidbody2D>();
        Enemy ourEnemy = bashObject.GetComponent<Enemy>();
        bashing = true;
        bashStartTime = Time.time;

        //pReference.rb.bodyType = RigidbodyType2D.Kinematic;
        pReference.rb.velocity = new Vector2(0.0f, 0.0f);


        //bashRigidbody.bodyType = RigidbodyType2D.Kinematic;
        bashRigidbody.velocity = new Vector2(0.0f, 0.0f);


        GameObject bashEnergyBall = Instantiate(energyBallPrefab);
        bashEnergyBall.transform.position = transform.position;

        while (Input.GetKey(KeyCode.B) && (Time.time - bashStartTime) < bashMaxDuration)
        {
            //fix this
            float timePassed = Time.time - bashStartTime;
            if (Mathf.Approximately((timePassed), (int)timePassed)){
                bashForce *= 1.2f;
            }
             
            yield return null;
        }

        Vector2 origin = transform.position;
        Vector2 mousePos = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        Vector2 trans = mousePositionWorld - origin;
        trans.Normalize();

       //k pReference.rb.gravityScale = 1.0f; 
       //k pReference.rb.bodyType = RigidbodyType2D.Dynamic;
        //kpReference.rb.AddForce(-trans * bashForce);
        StartCoroutine(BeingPushed(ourEnemy));
        ourEnemy.BeingManipulated(ShowDarkStarPhase.DarkStarPhases.pushPhase);
        // pReference.rb.AddForce(new Vector3(-bashForce, 0, 0));



        bashRigidbody.bodyType = RigidbodyType2D.Dynamic;
        bashRigidbody.AddForce(trans * bashForce, ForceMode2D.Force);

        Destroy(bashEnergyBall);

        bashForce = minimumBashForce;

        bashing = false;
       // yield return new WaitForSeconds(3.0f); //charge time


    }
    public IEnumerator BeingPushed(Enemy ourEnemy)
    {
        ourEnemy.beingPushed = true;
        yield return new WaitForSeconds(5.0f);
        ourEnemy.beingPushed = false;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        IBashable bashableObject = hit.GetComponent<IBashable>();
        if(bashableObject != null && !bashableObjectsInRange.Contains(hit.gameObject))
        {
            bashableObjectsInRange.Add(hit.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D hit)
    {

        IBashable bashableObject = hit.GetComponent<IBashable>();
        if (bashableObject != null && bashableObjectsInRange.Contains(hit.gameObject))
        {
            bashableObjectsInRange.Remove(hit.gameObject);
        }
    }

}
