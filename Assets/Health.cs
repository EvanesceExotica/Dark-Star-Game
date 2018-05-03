using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{

    public EatenBurst eatenBurstPrefab;
    public int maxHealth;
    public int currentHealth;

    public event Action<GameObject, SpaceMonster> Died;
    public SpawnSoul soulSpawn;
    public bool isEnemy;

    Enemy enemy;
    public PooledObject ourPooledObject;

    public List<GameObject> persistentDamageSources= new List<GameObject>();

    public void AddDamageSource(GameObject source){
        persistentDamageSources.Add(source);
    }



    public void AddPersistentDamageSource(float amount, float duration, float tickTime, GameObject source)
    {
        persistentDamageSources.Add(source);
        StartCoroutine(TakeDamageOverTime(amount, duration, tickTime, source));
    }

    public void RemovePersistentDamageSource(GameObject source)
    {
        persistentDamageSources.Remove(source);
    }

    // Use this for initialization
    void Start()
    {
        maxHealth = 3;
        currentHealth = maxHealth;
        soulSpawn = gameObject.GetComponent<SpawnSoul>();
        if (gameObject.tag == "Player")
        {
            isEnemy = false;
        }
        else
        {
            ourPooledObject = GetComponent<PooledObject>();
            enemy = GetComponent<Enemy>();
            isEnemy = true;
        }
    }

    // Update is called once per frame


    public IEnumerator TakeDamageOverTime(float amount, float duration, float tickTime, GameObject source)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            if (!persistentDamageSources.Contains(source))
            {
                yield break;
            }
            adjustCurrentHealth((int)amount, source);
            yield return new WaitForSeconds(tickTime);
        }
    }

    public void adjustCurrentHealth(int adj, GameObject source)
    {
        currentHealth += adj;
        if (adj < 0)
        {
            Debug.Log(gameObject.name + " is taking damage from " + source);
        }
        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " Our health is zero");
            if (source == GameStateHandler.player)
            {

                Debug.Log(gameObject.name + " died due to player");
            }
            Die(enemy.ourEnemyType, source);
            //Die(typeof(BlueDwarf));
        }
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }



    public virtual void BeingDevoured(GameObject source)
    {
        //TODO: MAke sure to have the Blue Dwarves panic and try to flee from the event horizons
        adjustCurrentHealth(-1, source);
        Debug.Log("I'm being devoured! Health is " + currentHealth);
        //TODO: Add disentagration effect
    }




    public virtual void Die(SpaceMonster ourEnemyType, GameObject source)
    {
        Debug.Log(gameObject.name + " We died regardless ");
        if (Died != null)
        {
            Died(this.gameObject, ourEnemyType);
        }
        if (isEnemy)
        {
            Debug.Log(gameObject.name + " We died and we're an enemy");
            if (source == GameStateHandler.DarkStarGO || source == GameStateHandler.player)
            {
                soulSpawn.SpawnsoulAroundDarkStar();
            }
            //here we're returning it to the pool rather than destoying it
            if (source == GameStateHandler.DarkStarGO || source.GetComponent<EventHorizon>() != null || source == GameStateHandler.player)
            {
                //TODO: Perhpas have a different explosion for different enemy types and player
                EatenBurst ourBurst = eatenBurstPrefab.GetPooledInstance<EatenBurst>();
                ourBurst.transform.position = transform.position;
            }
            ourPooledObject.ReturnToPool();
            //Destroy(this.gameObject);
            //gameObject.SetActive(false);
        }
        //add burst effect here

    }
    public virtual void Die()
    {

    }
}
