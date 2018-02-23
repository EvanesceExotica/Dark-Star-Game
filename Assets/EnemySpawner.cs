using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{

    public List<ParticleSystem> spawnEffect;


    public EnemyGroup enemiesToAppearOnThisLevel;
    public Transform spawner;
    public Health health;
    //public int spawnCap;
    //public int currentSpawned;
    //public float offSetRange;
    //public int powerCap;
    //public int powerSum;
    GameStateHandler gameStateHandler;

    public GameObject spawnerHolder;
    public event Action noEnemiesLeft;
    public GameObject enemyPrefab;


    public Action<int> enemySpawnMethod;
    SpaceMonster spaceMonsterPrefab;

    public BlueDwarf blueDwarfPrefab;
    public List<Vector2> spawnLocations;
    public List<GameObject> enemyTypes;
    public List<GameObject> currentEnemies;

    bool atEnemyCapacity;
    int maxNumberOfEnemies;
    int currentNumberOfEnemies;

    public Dictionary<Type, List<GameObject>> enemyDirectory = new Dictionary<Type, List<GameObject>>();
    public Dictionary<SpaceMonster, List<GameObject>> enemyDirectory_ = new Dictionary<SpaceMonster, List<GameObject>>();



    //   public List<GameObject> enemiesInLevel = new List<GameObject>();

    enum MonsterType
    {

    }

    void ConvertToType()
    {

    }
    void RemoveEnemyFromList(GameObject enemyToRemove, Type ourType)
    {
        enemyDirectory[ourType].Remove(enemyToRemove);
        // currentEnemies.Remove(enemyToRemove);
        currentNumberOfEnemies--;
        UpdateEnemies(ourType);
    }

    void UpdateEnemies(Type ourType)
    {

        if (enemyDirectory[ourType] != null && enemyDirectory[ourType].Count > 0 /*currentEnemies != null && currentEnemies.Count > 0*/)
        {

            //Desubscribe everything in the enemy list
            foreach (GameObject enemy in enemyDirectory[ourType])
            {
                if (enemy.GetComponent<Enemy>() != null)
                {
                    enemy.GetComponent<Health>().Died -= this.RemoveEnemyFromList;
                }
            }
            //Resubscribe them all again
            foreach (GameObject enemy in enemyDirectory[ourType])
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

        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        enemySpawnMethod = SpawnBlueDwarf;
        // UpdateEnemies();



    }

    public GameObject GetClosestAlly(IGoap allyType, GameObject allySeeker)
    {

        GameObject potentialMate = null;
        //change this
        potentialMate = FindClosest.FindClosestObject(enemyDirectory[allyType.GetType()], allySeeker);

        return potentialMate;

    }
    /** */
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
        maxNumberOfEnemies = 8;
        // StartCoroutine(SpawnOverTime());
        SpawnGeneric(blueDwarfPrefab, 4, 5);
        //SpawnBlueDwarf(8);

    }

    public IEnumerator SpawnOverTime()
    {
        yield return new WaitForSeconds(10.0f);
        SpawnGeneric_(maxNumberOfEnemies);
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
                SpawnGeneric_(maxNumberOfEnemies);
            }
            else if (currentNumberOfEnemies == maxNumberOfEnemies / 2)
            {
                //if half enemies are left, spawn to meet that number
                SpawnGeneric_(maxNumberOfEnemies / 2);
            }
            else if (currentNumberOfEnemies == 1)
            {
                SpawnGeneric_(maxNumberOfEnemies - 1);
            }
        }
    }

    void SpawnRandom(int numberOfEnemiesToSpawn)
    {

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            GameObject potentialEnemy = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count - 1)];
            GameObject newEnemy = Instantiate(potentialEnemy, transform);
            newEnemy.transform.position = FindRandomSpawnPointAroundStar.FindLocationAroundStar(4.0f, gameStateHandler.darkStar.GetComponent<CircleCollider2D>().bounds.extents.x, gameStateHandler.darkStar.transform.position);
            // Debug.Log("We were spawned here! " + FindRandomSpawnPointAroundStar.FindLocationAroundStar(4.0f));
            currentNumberOfEnemies++;
            currentEnemies.Add(newEnemy);
        }
        //TODO: Fix this so that it's updating in the dictionary rather than the list
        // UpdateEnemies();

    }
    // GameObject RandomSpawnFilter()
    // {
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
    // }

    public void SpawnIndependent(SpaceMonster ourType, Vector2 position)
    {
        if (!atEnemyCapacity)
        {
            Debug.Log("We're being spawned!");


            SpaceMonster spaceMonster = ourType.GetPooledInstance<SpaceMonster>();
            spaceMonster.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 5.0f);
            currentNumberOfEnemies++;
            if (currentNumberOfEnemies == maxNumberOfEnemies)
            {
                atEnemyCapacity = true;
            }
        }
    }

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
            newBlueDwarf.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);

            if (!enemyDirectory.ContainsKey(typeof(BlueDwarf)))
            {
                List<GameObject> blueDwarfList = new List<GameObject>();
                enemyDirectory.Add(typeof(BlueDwarf), blueDwarfList);
                enemyDirectory[typeof(BlueDwarf)].Add(newBlueDwarf.gameObject);

            }
            else
            {

                enemyDirectory[typeof(BlueDwarf)].Add(newBlueDwarf.gameObject);
            }
        }
    }

    SpaceMonster MakeSureUnderPowerCap(int spawnRoundPowerCap, int currentPowerLevel)
    {

        //List<SpaceMonster> newEnemiesToSpawn = new List<SpaceMonster>();
        SpaceMonster enemyToSpawn = null;
        int runningPowerLevelOfGroupToSpawn = 0;
        //grab a random number from the enum that corresponds to a type
        EnemyGroup.EnemyTypes potentialType = (EnemyGroup.EnemyTypes)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyGroup.EnemyTypes)).Length);
        int potentialPowerCap = enemiesToAppearOnThisLevel.correspondingEnemyThreatLevel[potentialType];
        if (potentialPowerCap + runningPowerLevelOfGroupToSpawn <= spawnRoundPowerCap)
        {
            //if this enemy type's power level + how much our current power level is is less than the power cap we want for this group
            //choose this spacemonster to spawn
            enemyToSpawn = enemiesToAppearOnThisLevel.correspondingSpaceMonster[potentialType].GetPooledInstance<SpaceMonster>();

            //add the power cap to the total
           runningPowerLevelOfGroupToSpawn += potentialPowerCap; 
            // newEnemiesToSpawn.Add(newEnemy) ;;
        }
        else
        {

            MakeSureUnderPowerCap(spawnRoundPowerCap, currentPowerLevel);

        }

        return enemyToSpawn;

    }

    void SpawnGeneric_(int numberSpawnedAtOnce, int spawnRoundPowerCap)
    {
        for (int i = 0; i < numberSpawnedAtOnce; i++)
        {
            if (currentNumberOfEnemies == maxNumberOfEnemies)
            {
                atEnemyCapacity = true;
                break;
            }

            SpaceMonster genericMonster_ = MakeSureUnderPowerCap(numberSpawnedAtOnce, currentPowerLevel);//enemiesToAppearOnThisLevel.enemyTypes[UnityEngine.Random.Range(0, enemiesToAppearOnThisLevel.enemyTypes.Count - 1)];

            genericMonster_.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);
            Type ourType = genericMonster_.GetType();
            if (!enemyDirectory.ContainsKey(ourType))
            {
                List<GameObject> newGOsOfThisSpaceMonsterTypeList = new List<GameObject>();
                enemyDirectory.Add(ourType, newGOsOfThisSpaceMonsterTypeList);
                enemyDirectory[ourType].Add(genericMonster_.gameObject);

            }
            else
            {

                enemyDirectory[ourType].Add(genericMonster_.gameObject);
            }
            currentNumberOfEnemies++;

        }

    }

    void SpawnGeneric(SpaceMonster genericMonster, int numberSpawnedAtOnce, int spawnRoundPowerCap)
    {
        //TODO: fix this up later 
        for (int i = 0; i < numberSpawnedAtOnce; i++)
        {
            if (currentNumberOfEnemies == maxNumberOfEnemies)
            {
                atEnemyCapacity = true;
                break;
            }
            SpaceMonster genericMonster_ = enemiesToAppearOnThisLevel.enemyTypes[UnityEngine.Random.Range(0, enemiesToAppearOnThisLevel.enemyTypes.Count - 1)];

            SpaceMonster ourNewMonster = genericMonster.GetPooledInstance<SpaceMonster>();
            ourNewMonster.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);
            // BlueDwarf newBlueDwarf = blueDwarfPrefab.GetPooledInstance<BlueDwarf>();
            // newBlueDwarf.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);
            Type ourType = ourNewMonster.GetType();
            if (!enemyDirectory.ContainsKey(ourType))
            {
                List<GameObject> newGOsOfThisSpaceMonsterTypeList = new List<GameObject>();
                enemyDirectory.Add(ourType, newGOsOfThisSpaceMonsterTypeList);
                enemyDirectory[ourType].Add(ourNewMonster.gameObject);
                // List<GameObject> blueDwarfList = new List<GameObject>();
                // enemyDirectory.Add(typeof(BlueDwarf), blueDwarfList);
                // enemyDirectory[typeof(BlueDwarf)].Add(ourNewMonster.gameObject);

            }
            else
            {

                //enemyDirectory[typeof(BlueDwarf)].Add(ourNewMonster.gameObject);
                enemyDirectory[ourType].Add(ourNewMonster.gameObject);
            }
            currentNumberOfEnemies++;

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
