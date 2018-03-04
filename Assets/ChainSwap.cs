using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
public class ChainSwap : PowerUp
{

    EnemySpawner enemySpawner;
    public GameObject chainEnd;

    [SerializeField] GameObject chainedEnemy;
    LineRenderer chainLineRenderer;
    List<ParticleSystem> particleSystem;
    GameObject particleSystemGameObject;

    [SerializeField]
    float duration;

    bool throwingChain;
    bool grabbedEnemy;
    bool canChainEnemy;

    public LayerMask enemyMask;

    public override void Awake()
    {
        base.Awake();
        Switch.SwitchEntered += this.SetOnSwitch;
        Switch.SwitchExited += this.SetOffSwitch;
        chainLineRenderer = chainEnd.GetComponent<LineRenderer>();

        ChoosePowerUp.chainChosen +=  this.SetPoweredUp;
        ourRequirement = Requirement.OnlyUseOnSwitch;
        autoActivated = false;
        //   particleSystem = particleSystemGameObject.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    // void SetPoweredUp(){
    //     PoweredUp = true;
    //     if(onSwitch){
    //         //if we're also on a switch, we can chain enemy now
    //         canChainEnemy = true;
    //     }
    // }

    // void RemovePoweredUp(){
    //     //both powered up and on switch hae to be true to chain enemy, so set canChainEnemy to false
    //     PoweredUp = false;
    //     canChainEnemy = false;
    // }

    
    // void SetOnSwitch(GameObject ourSwitch)
    // {
    //     onSwitch = true;
    //     if(PoweredUp){
    //         //if we're also powered up, we can chain enemy now
    //         canChainEnemy = true;
    //     }
    // }

    // void SetOffSwitch(GameObject ourSwitch)
    // {
    //     onSwitch = false;
    //     //both powered up and on switch hae to be true to chain enemy, so set canChainEnemy to false
    //     canChainEnemy = false;
    // }

    public override void StartPowerUp(){
        base.StartPowerUp();
        StartCoroutine(ChainEnemy());
    }

    void BeginChainEnemy(GameObject ourSwitch)
    {
        StartCoroutine(ChainEnemy());
    }

   
    Vector2 pointWeHitEnemy;

    [SerializeField] bool holdingEnemy;
    Vector2 originalPlayerPosition;
    public IEnumerator ChainEnemy()
    {
        throwingChain = true;
        //chain through enemies starting with closest -- show  a growing chain
        //want to darken the screen here
        originalPlayerPosition = transform.position;
        //raycast enemies so it's like a drag thing
        //TODO: put back in // 
        ZoomOnPlayer.ZoomOut(15, 3.0f, GameStateHandler.ourProCamera2D);
        FreezeTime.SlowdownTime(0.75f);
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
        HighlightSprite spriteHighlighter = null;
        UniversalMovement ourEnemyMovement = null;

        //        GameObject chainedEnemy = FindClosest.FindClosestObject(enemySpawner.currentEnemies, this.gameObject);
        chainLineRenderer.enabled = true;
        //hold down button to grow chain, then swap
        float startTime = Time.time;
        chainLineRenderer.SetPosition(0, transform.position);

        float holdOnEnemyTime = 0.0f;
        float distance = 0;

        while (Time.time < startTime + 10.0f * 0.75f)
        {

            if (Input.GetKeyUp(KeyCode.P) && !holdingEnemy)
            {
                //yield break;
            }
            mousePositionScreen = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(mousePositionScreen.x, mousePositionScreen.y));
            trans = (mousePosition - (Vector2)transform.position);
            trans.Normalize();

            chainRigidbody.velocity = trans * (throwSpeed);

            //float step = duration * Time.deltaTime;
            //step *= 0.75f; //not sure if this is the right thing to do
            // chainEnd.transform.position = Vector2.MoveTowards(transform.position, mousePosition, 4 * Time.deltaTime);
            //alter this to fit the slow

            //shooting out a line that gets longer
            distance = Vector2.Distance(transform.position, chainEnd.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, trans, distance, enemyMask);
            Debug.DrawRay(transform.position, trans * distance, Color.blue, 30.0f);

            Vector2 endPosition = (Vector2)transform.position + (distance * trans);
            chainLineRenderer.SetPosition(1, endPosition);

            if (hit && hit.collider.GetComponent<Enemy>() != null)
            {
                //if our raycast is currently hitting an enemy
                if (!holdingEnemy)
                {
                    //if we haven't set the enemy values yet, sedt all the values below and display the frozen effect
                    chainedEnemy = hit.collider.gameObject;
                    chainedEnemy.GetComponent<UniversalMovement>().cantMove = true;
                    spriteHighlighter = chainedEnemy.GetComponent<HighlightSprite>();
                    ourEnemyMovement = chainedEnemy.GetComponent<UniversalMovement>();
                    spriteHighlighter.DisplayFrozenEffect();
                }

                //set that we're holding the enemy now so the above values aren't constantly set
                holdingEnemy = true;

                //increase the time that we're holding the enemy
                holdOnEnemyTime += Time.deltaTime;
                Debug.Log("How long we've been holding on enemy " + (int)holdOnEnemyTime);

                if (holdOnEnemyTime >= 2.0f || Input.GetKeyUp(KeyCode.P))
                {
                    //if we've been holding the enemy for 2 seconds or more

                    hitPoint = hit.point;
                    pointWeHitEnemy = hitPoint;
                    hitNormal = hit.normal;

                    //So the normal will always bounce straight from the wall regardless, so by subtracting the hitpoint by it, you're getting a ray pointing directly opposite
                    //we want to jump to the exact other side of an enemy by finding the opposite point 
                    //shoot a new raycast through the enemys backside and set it as the point to jump to, then pull us out of this loop so we can jump
                    RaycastHit2D newHit = Physics2D.Raycast(hitPoint - hitNormal * 1000, hitNormal, Mathf.Infinity, enemyMask);

                    pointToJumpTo = newHit.point;
                    break;
                }
                
                //if our raycast hits an enemy


            }
            else
            {
                //if we aren't hitting an enemy right now

                if (chainedEnemy != null && holdingEnemy)
                {
                    //but if we WERE hitting an enemy at some point
                    ourEnemyMovement.StartedMovingAgain();
                    //free that enemy from being frozen 
                    //TODO: MAke sure these frozen effects are stackable so that if something else is restricting the movement we aren't cancelling it out

                    //below, we reset all of the variables since we're not holding on to that enemy anymore
                    holdOnEnemyTime = 0.0f;
                    chainedEnemy = null;
                    ourEnemyMovement = null;
                    spriteHighlighter = null;
                    holdingEnemy = false;
                }
                //this should be resetting the holdonenemytime if the player is not holding the line over the enemy; 

            }

            //add points to line renderer
            yield return null;
        }

        FreezeTime.StartTimeAgain();
        // TODO: PUT BACK IN//  
        ZoomOnPlayer.ZoomInOnPlayer(-10, 3.0f, GameStateHandler.ourProCamera2D);
        //   ParticleSystemPlayer.PlayChildParticleSystems(particleSystem);
        //we want them to jump to the end of the chain
        if (chainedEnemy != null)
        {
            grabbedEnemy = true;
            //jump the player behind the enemy and send the enemy flying back toward the original player position
            //add a chain linerenderer effect that looks as if its pulling the player
            Rigidbody2D enemyRigidbody = chainedEnemy.GetComponent<Rigidbody2D>();
            transform.position = pointToJumpTo;
            Vector2 trans2 = originalPlayerPosition - (Vector2)chainedEnemy.transform.position;
            enemyRigidbody.velocity = trans2 * 2.0f;
        }
        //   chainedEnemy.GetComponent<Rigidbody2D>().AddForce();
        throwingChain = false;
    }



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
    public override void Update()
    {
        base.Update();
        if (grabbedEnemy)
        {
            //if the chain is pulling an enemy
            if (chainedEnemy == null || Vector2.Distance(originalPlayerPosition, chainedEnemy.transform.position) < 1.5f)
            {
                //if the enemy exists (and hasn't fallen into the star) or the enemy gas reached the player's old position
                grabbedEnemy = false;
                originalPlayerPosition = new Vector2(0, 0);
                chainLineRenderer.enabled = false;
                //perhaps play a particle effect showing the chain breaking
            }
            else
            {
                //have the chain go from the enemy to the original position to make it look as if it's being pulled
                UpdateLineRenderer(originalPlayerPosition, chainedEnemy.transform.position);
            }

        }
        // else
        // {
        // }
        // if (Input.GetKeyDown(KeyCode.P) && canChainEnemy)
        // {
        //     StartCoroutine(ChainEnemy());
        // }
        if (throwingChain)
        {
            UpdateLineRenderer(transform.position, chainEnd.transform.position);
        }
    }
}
