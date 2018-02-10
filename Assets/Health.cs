﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable {

    public int maxHealth;
    public int currentHealth;
    public event Action<GameObject, Type> Died;
    public SpawnSoul soulSpawn;
    public bool isEnemy;

    Enemy enemy;
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
        maxHealth = 3;
        currentHealth = maxHealth;
        soulSpawn = gameObject.GetComponent<SpawnSoul>();
        if(gameObject.tag == "Player")
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
        if(adj < 0){
            Debug.Log(gameObject.name + " is taking damage from " + source);
        }
        if(currentHealth <= 0)
        {
            if(source == GameStateHandler.player){

                Debug.Log(gameObject.name + " died due to player");
            }
            //TODO: fix this later 
            Die(enemy.ourEnemyType.GetType());
           //Die(typeof(BlueDwarf));
        }
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
    }


    public virtual void Die(Type ourEnemyType)
    {
        if (Died != null)
        {
            Died(this.gameObject, ourEnemyType);
        }
        if (isEnemy)
        {
            soulSpawn.SpawnsoulAroundDarkStar();
            //here we're returning it to the pool rather than destoying it
            ourPooledObject.ReturnToPool();
            //Destroy(this.gameObject);
            //gameObject.SetActive(false);
        }
        //add burst effect here
        
    }
    public virtual void Die(){

    }
}
