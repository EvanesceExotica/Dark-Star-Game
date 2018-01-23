using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable {

    public int maxHealth;
    public int currentHealth;
    public event Action<GameObject> Died;
    public SpawnSoul soulSpawn;
    bool isEnemy;
    PooledObject ourPooledObject;

    List<GameObject> persistentDamageSources;


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
    void Start() {
        maxHealth = 10;
        currentHealth = maxHealth;
        soulSpawn = gameObject.GetComponent<SpawnSoul>();
        if(gameObject.tag == "Player")
        {
            isEnemy = false;
        }
        else
        {
            ourPooledObject = GetComponent<PooledObject>();
            isEnemy = true;
        }
    }

    // Update is called once per frame
    void Update() {

    }


    public IEnumerator TakeDamageOverTime(float amount, float duration, float tickTime, GameObject source)
    {
        float startTime = Time.time;
        while(Time.time < startTime + duration)
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

        if(currentHealth <= 0)
        {
            Die();
        }
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
    }


    public virtual void Die()
    {
        if (Died != null)
        {
            Died(this.gameObject);
        }
        if (isEnemy)
        {
            soulSpawn.SpawnsoulAroundDarkStar();
            //here we're returning it to the pool rather than destoying it
            ourPooledObject.ReturnToPool();
            //Destroy(this.gameObject);
            //gameObject.SetActive(false);
        }
        else
        {
        }
        //add burst effect here
        
    }
}
