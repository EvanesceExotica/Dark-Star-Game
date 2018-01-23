﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{


    public Transform spawner;
    public Health health;
    //public int spawnCap;
    //public int currentSpawned;
    //public float offSetRange;
    //public int powerCap;
    //public int powerSum;
    GameStateHandler handler;


    public GameObject spawnerHolder;
    public event Action noEnemiesLeft;
    public GameObject enemyPrefab;



    BlueDwarf blueDwarfPrefab;
    public List<Vector2> spawnLocations;
    public List<GameObject> enemyTypes;
    public List<GameObject> currentEnemies;

    int maxNumberOfEnemies;
    int currentNumberOfEnemies;


    //   public List<GameObject> enemiesInLevel = new List<GameObject>();

    enum MonsterType
    {

    }

    void RemoveEnemyFromList(GameObject enemyToRemove)
    {
        currentEnemies.Remove(enemyToRemove);
        currentNumberOfEnemies--;
        UpdateEnemies();
    }

    void UpdateEnemies()
    {

        if (currentEnemies != null && currentEnemies.Count > 0)
        {

            //Desubscribe everything in the enemy list
            foreach (GameObject enemy in currentEnemies)
            {
                if (enemy.GetComponent<Enemy>() != null)
                {
                    enemy.GetComponent<Health>().Died -= this.RemoveEnemyFromList;
                }
            }
            //Resubscribe them all again
            foreach (GameObject enemy in currentEnemies)
            {
                if (enemy.GetComponent<Enemy>() != null)
                {
                    enemy.GetComponent<Health>().Died += this.RemoveEnemyFromList;
                }
            }
        }

    }

    void NoEnemiesLeft()
    {
        if (noEnemiesLeft != null)
        {
            noEnemiesLeft();
        }
    }

    List<GameObject> typesOfMonsters = new List<GameObject>();

    private void Awake()
    {
        handler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        UpdateEnemies();



    }

    //spawnerHolder = this.gameObject;
    //foreach (Transform child in spawnerHolder.transform)
    //{
    //    spawnLocations.Add(child.transform.position);

    //}
    //maxNumberOfEnemies = 1;


    //for (int i = 0; i < spawnLocations.Count; i++)
    //{
    //    GameObject go = Instantiate(enemyPrefab) as GameObject;
    //    go.transform.position = spawnLocations[i];
    //    handler.enemiesInLevel.Add(go);
    //    handler.numberOfEnemiesLeft++;
    //    handler.enemyHealths.Add(go.GetComponent<Health>());

    //}

    void Start()
    {
        currentNumberOfEnemies = 0;
        maxNumberOfEnemies = 4;
       // StartCoroutine(SpawnOverTime());
       SpawnBlueDwarf(6);

    }

    public IEnumerator SpawnOverTime()
    {
        SpawnRandom(maxNumberOfEnemies);
        while (true)
        {
            if (GameStateHandler.currentGameState == GameStateHandler.GameState.dark && GameStateHandler.currentGameState == GameStateHandler.GameState.starOpened)
            {
                //We don't want to spawn enemies when the star is opened or we're in the dark stage
                break;
            }
            yield return new WaitForSeconds(10.0f); //doomclock 60 seconds
            if (currentNumberOfEnemies == 0)
            {
                //if there are no enemies left on the screen, spawn new ones at this point.
                SpawnRandom(maxNumberOfEnemies);
            }
            else if (currentNumberOfEnemies == maxNumberOfEnemies / 2)
            {
                SpawnRandom(maxNumberOfEnemies / 2);
            }
            else if (currentNumberOfEnemies == 1)
            {
                SpawnRandom(2);
            }
        }
    }

    void SpawnRandom(int numberOfEnemiesToSpawn)
    {

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            GameObject potentialEnemy = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count - 1)];
            GameObject newEnemy = Instantiate(potentialEnemy, transform);
            newEnemy.transform.position = FindRandomSpawnPointAroundStar.FindLocationAroundStar(4.0f, handler.darkStar.GetComponent<CircleCollider2D>().bounds.extents.x, handler.darkStar.transform.position);
            // Debug.Log("We were spawned here! " + FindRandomSpawnPointAroundStar.FindLocationAroundStar(4.0f));
            currentNumberOfEnemies++;
            currentEnemies.Add(newEnemy);
        }
        UpdateEnemies();

    }
    //GameObject RandomSpawnFilter()
    //{
    //    GameObject newSpawn;
    //    for (int i = 0; i < spawnCap; i++)
    //    {
    //        newSpawn = fh.UnitTypes_[UnityEngine.Random.Range(0, (fh.UnitTypes_.Count))];
    //        Unit thisUnit = newSpawn.GetComponent<Unit>();
    //        int power = thisUnit.powerLevel;
    //        //For example, power level is 2
    //        if ((power + powerSum) > powerCap)
    //        { // if the power level in addition to other power levels goes over the power cap
    //            newSpawn = RandomSpawnFilter();
    //            //respawn it to get one that fits within the power level
    //        }
    //        else
    //        { //add power to power sum;
    //            powerSum += power;
    //            thisUnit.fh = fh;
    //            counter++;
    //        }
    //    }
    //}

    GameObject RandomSpawnFilter()
    {
        GameObject newSpawn = null;
        for (int i = 0; i < maxNumberOfEnemies; i++)
        {
            Debug.Log("Spawning enemy");
            if (enemyTypes.Count > 0)
            {
                newSpawn = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count - 1)];
            }
        }
        return newSpawn;
    }


    void SpawnBlueDwarf(int numberSpawnedAtOnce)
    {
        for (int i = 0; i < numberSpawnedAtOnce; i++)
        {
            BlueDwarf newBlueDwarf = blueDwarfPrefab.GetPooledInstance<BlueDwarf>();
            newBlueDwarf.transform.position = 
        }


    }

    void SpawnEventHorizon(int numberSpawnedAtOnce)
    {

    }

    // Use this for initialization





    // Update is called once per frame
    void Update()
    {

    }
}
