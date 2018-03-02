using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
public class ChainSwap : MonoBehaviour
{

    EnemySpawner enemySpawner;
    public GameObject chainEnd;

    GameObject chainedEnemy;
    LineRenderer chainLineRenderer;
    List<ParticleSystem> particleSystem;
    GameObject particleSystemGameObject;
    [SerializeField]
    float duration;

    bool throwingChain;
    bool grabbedEnemy;
    bool canChainEnemy;

    public LayerMask enemyMask;

    void Awake()
    {

        Switch.SwitchEntered += this.SetCanChainEnemy;
        Switch.SwitchExited += this.SetCannotChainEnemy;
        chainLineRenderer = chainEnd.GetComponent<LineRenderer>();
        //   particleSystem = particleSystemGameObject.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    void SetCanChainEnemy(GameObject ourSwitch)
    {
        canChainEnemy = true;
    }

    void SetCannotChainEnemy(GameObject ourSwitch)
    {
        canChainEnemy = false;
    }

    void BeginChainEnemy(GameObject ourSwitch)
    {
        StartCoroutine(ChainEnemy());
    }

    void ZoomOut()
    {

    }

    void ZoomBackToNormal()
    {

    }
    Vector2 pointWeHitEnemy;

    bool holdingEnemy;
    Vector2 originalPlayerPosition;
    public IEnumerator ChainEnemy()
    {
        throwingChain = true;
        //chain through enemies starting with closest -- show  a growing chain
        //want to darken the screen here
        originalPlayerPosition = transform.position;
        //raycast enemies so it's like a drag thing
        //TODO: put back in // ZoomOnPlayer.ZoomOut(-10, 3.0f);
       // FreezeTime.SlowdownTime(0.75f);
        Vector2 mousePosition;
        Vector2 mousePositionScreen;
        Vector2 trans;
        chainedEnemy = null;
        Vector2 hitPoint;
        Vector2 hitNormal;
        Vector2 pointToJumpTo = new Vector2(0, 0);
        float throwSpeed = 14.0f;
        // RaycastHit2D hit = Physics2D.Raycast(transform.position, trans, Mathf.Infinity, enemyMask);
        Rigidbody2D chainRigidbody = chainEnd.GetComponent<Rigidbody2D>();

        //        GameObject chainedEnemy = FindClosest.FindClosestObject(enemySpawner.currentEnemies, this.gameObject);
        chainLineRenderer.enabled = true;
        //hold down button to grow chain, then swap
        float startTime = Time.time;
        chainLineRenderer.SetPosition(0, transform.position);

        float holdOnEnemyTime = 0.0f;

        while (/*Time.time < startTime + 10.0f * 0.75f*/ true)
        {
            if(Input.GetKeyUp(KeyCode.P) && !holdingEnemy){
                yield break;
            }
            //TODO: LINE IS MOVING TOO SLOWLY WITH MOUSE -- FIX MOUSE IS MOVING TOO SLOWLY WITH POINT
            mousePositionScreen = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(mousePositionScreen.x, mousePositionScreen.y));
            trans = (mousePosition - (Vector2)transform.position);
            trans.Normalize();
            //TODO: Make this a variable 
    
            chainRigidbody.velocity = trans * (throwSpeed );

            //float step = duration * Time.deltaTime;
            //step *= 0.75f; //not sure if this is the right thing to do
            // chainEnd.transform.position = Vector2.MoveTowards(transform.position, mousePosition, 4 * Time.deltaTime);
            //alter this to fit the slow

            //shooting out a line that gets longer
            float distance = Vector2.Distance(transform.position, chainEnd.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, trans, distance, enemyMask);
            Debug.DrawRay(transform.position, trans * distance, Color.blue, 30.0f);

            Vector2 endPosition = (Vector2)transform.position + (distance * trans);
            chainLineRenderer.SetPosition(1, endPosition);

            if (hit && hit.collider.GetComponent<Enemy>() != null)
            { 
                if(holdOnEnemyTime == 2.0f){
                    break;
                }
                holdingEnemy = true;
                chainedEnemy.GetComponent<UniversalMovement>().cantMove = true;

                holdOnEnemyTime += Time.deltaTime;

                if(holdOnEnemyTime == 2.0f){
                    break;
                }
                //if our raycast hits an enemy
                hitPoint = hit.point;
                pointWeHitEnemy = hitPoint;
                hitNormal = hit.normal;
                //So the normal will always bounce straight from the wall regardless, so by subtracting the hitpoint by it, you're getting a ray pointing directly opposite
                //TODO: Fix this for more accurate representation, all these numbers are wrong
                //we want to jump to the exact other side of an enemy by finding the opposite point 
                RaycastHit2D newHit = Physics2D.Raycast(hitPoint - hitNormal * 1000, hitNormal, Mathf.Infinity, enemyMask);

                newHit.point = pointToJumpTo * 2;
                chainedEnemy = hit.collider.gameObject;
               
            }
            throwingChain = false;

            //add points to line renderer
            yield return null;
        }

        FreezeTime.StartTimeAgain();
        // TODO: PUT BACK IN//  ZoomOnPlayer.ZoomInOnPlayer(10, 3.0f);
        //   ParticleSystemPlayer.PlayChildParticleSystems(particleSystem);
        //we want them to jump to the end of the chain
        if (chainedEnemy != null)
        {
            grabbedEnemy = true;
            //jump the player behind the enemy and send the eneny flying back toward the original player position
            //add a chain linerenderer effect that looks as if its pulling the player
            Rigidbody2D enemyRigidbody = chainedEnemy.GetComponent<Rigidbody2D>();
            transform.position = pointWeHitEnemy;
            Vector2 trans2 = originalPlayerPosition - (Vector2)chainedEnemy.transform.position;
            enemyRigidbody.velocity = trans2 * 2.0f;
        }
        //   chainedEnemy.GetComponent<Rigidbody2D>().AddForce();
        throwingChain = false;
    }

    // public IEnumerator LockOntoEnemy(){
    //     float startTime = Time.time;
    //     while(Time.time < startTime + 2.0f){
    //         RaycastHit2D hit =  Physics2D.Raycast()
    //     }


    // }
   

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DebugExtension.DrawPoint(pointWeHitEnemy, 1.0f);
    }

    void UpdateLineRenderer(Vector2 startPosition, Vector2 endPosition)
    {
        chainLineRenderer.SetPosition(0, startPosition);
        chainLineRenderer.SetPosition(1, endPosition);
    }
    // Update is called once per frame
    void Update()
    {
        if(grabbedEnemy){
            //if the chain is pulling an enemy
            if(chainedEnemy == null || Vector2.Distance(originalPlayerPosition, chainedEnemy.transform.position) < 1.5f ){
                //if the enemy exists (and hasn't fallen into the star) or the enemy gas reached the player's old position
                grabbedEnemy = false;
                originalPlayerPosition = new Vector2(0, 0);
                chainLineRenderer.enabled = false;
                //perhaps play a particle effect showing the chain breaking
            }
            else{
                //have the chain go from the enemy to the original position to make it look as if it's being pulled
                UpdateLineRenderer(originalPlayerPosition, chainedEnemy.transform.position);
            }

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(ChainEnemy());
        }
        if (throwingChain)
        {
          //  UpdateLineRenderer(transform.position, chainEnd.transform.position);
        }
    }
}
