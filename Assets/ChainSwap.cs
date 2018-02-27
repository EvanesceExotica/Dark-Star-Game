using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChainSwap : MonoBehaviour
{

    EnemySpawner enemySpawner;
    GameObject chainEnd;
    LineRenderer chainLineRenderer;
    List<ParticleSystem> particleSystem;
    GameObject particleSystemGameObject;
    [SerializeField]
    float duration;

    bool chaining;
    bool canChainEnemy;

    void Awake()
    {

        Switch.SwitchEntered += this.SetCanChainEnemy;
        Switch.SwitchExited += this.SetCannotChainEnemy;
        particleSystem = particleSystemGameObject.GetComponentsInChildren<ParticleSystem>().ToList();
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

	void ZoomOut(){

	}

	void ZoomBackToNormal(){

	}
    public IEnumerator ChainEnemy()
    {
		chaining = true;
        //chain through enemies starting with closest -- show  a growing chain
        //want to darken the screen here

		//raycast enemies so it's like a drag thing
        FreezeTime.SlowdownTime(0.75f);
        GameObject chainedEnemy = FindClosest.FindClosestObject(enemySpawner.currentEnemies, this.gameObject);
        chainLineRenderer.enabled = true;
        //hold down button to grow chain, then swap
        float startTime = Time.time;

        while (Time.time < startTime + duration * 0.75f)
        {
            //alter this to fit the slow
            float step = duration * Time.deltaTime;
            step *= 0.75f; //not sure if this is the right thing to do
            chainEnd.transform.position = Vector2.MoveTowards(transform.position, chainedEnemy.transform.position, step);
            yield return null;
        }
        FreezeTime.StartTimeAgain();
        ParticleSystemPlayer.PlayChildParticleSystems(particleSystem);
        //we want them to jump to the end of the chain
        transform.position = chainEnd.transform.position;
		chaining = false;



    }

    void UpdateLineRenderer()
    {
        chainLineRenderer.SetPosition(0, transform.position);
        chainLineRenderer.SetPosition(1, chainEnd.transform.position);
    }
    // Update is called once per frame
    void Update()
    {
        if (chaining)
        {
            UpdateLineRenderer();
        }
    }
}
