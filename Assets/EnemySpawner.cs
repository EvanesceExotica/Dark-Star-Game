using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{

    public List<ParticleSystem> spawnEffect;


    public EnemyGroup enemyGroup;
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

    public List<List<int>> enemiesAndWaves;
    [SerializeField] int maxWavesBeforeRecycle;

    [SerializeField] int indexOfWaveToRecycle;
    [SerializeField] int maxEnemiesPerWave;


    public Action<int> enemySpawnMethod;
    SpaceMonster spaceMonsterPrefab;

    public BlueDwarf blueDwarfPrefab;
    public List<Vector2> spawnLocations;
    public List<GameObject> enemyTypes;
    public List<GameObject> currentEnemies;

    //TODO: Maybe have this be dependent on the particular wave.
    [SerializeField] int initialSpawnNumber;

    [SerializeField] bool atEnemyCapacity;
    [SerializeField] int maxNumberOfEnemies;
    [SerializeField] int currentNumberOfEnemies;

    [SerializeField] int currentWave;

    public Dictionary<Type, List<GameObject>> enemyDirectory = new Dictionary<Type, List<GameObject>>();
    public Dictionary<SpaceMonster, List<GameObject>> enemyDirectory_ = new Dictionary<SpaceMonster, List<GameObject>>();

    public List<int> enemiesThatWerentSpawned;

    public List<Wave> levelWaves;

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
        currentEnemies.Remove(enemyToRemove);
        currentNumberOfEnemies--;
        StartCoroutine(SpawnBackups());
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





    List<GameObject> typesOfMonsters = new List<GameObject>();

    private void Awake()
    {
        enemiesAndWaves = new List<List<int>>();
        foreach (Wave wave in levelWaves)
        {
            enemiesAndWaves.Add(wave.PopulateWaves());
        }
        currentWave = 0;
        enemyGroup = GetComponent<EnemyGroup>();
        Doomclock.StartingNewDoomclockCycle += this.StartNewWave;
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        enemySpawnMethod = SpawnBlueDwarf;
        // enemiesAndWaves = new List<List<int>>();
        // List<int> testList = new List<int>();
        // for (int i = 0; i < 5; i++)
        // {
        //     testList.Add(0);
        // }
        // enemiesAndWaves.Add(testList);
        // UpdateEnemies();



    }

    void StartNewWave(int nextWave)
    {
        //take any remaining and drop them in the last list.
        //TODO: Maybe instead we want to reuse them 
        if (enemiesThatWerentSpawned != null)
        {
            enemiesThatWerentSpawned.AddRange(enemiesAndWaves[currentWave]);
        }
        else
        {
            enemiesThatWerentSpawned = new List<int>();
            enemiesThatWerentSpawned.AddRange(enemiesAndWaves[currentWave]);
        }
        enemiesAndWaves.RemoveAt(currentWave);
        currentWave = nextWave;
        CheckSpawningProgress();

    }

    void CheckSpawningProgress()
    {

        StartCoroutine(SpawnInitialWave());
    }

    public GameObject GetClosestAlly(IGoap allyType, GameObject allySeeker)
    {

        GameObject potentialMate = null;
        //change this
        potentialMate = FindClosest.FindClosestObject(enemyDirectory[allyType.GetType()], allySeeker);

        return potentialMate;

    }


    void Start()
    {
        currentNumberOfEnemies = 0;
        maxNumberOfEnemies = 8;
        StartCoroutine(SpawnInitialWave());
    }

    public IEnumerator SpawnBackups()
    {
        //TODO: MAke this not a magic number v
        yield return new WaitForSeconds(10.0f);
        if (currentNumberOfEnemies != maxNumberOfEnemies && enemiesAndWaves[currentWave].Count > 0)
        {
            //if we're not at max capacity, and the current wave still has enemies to choose from, spawn a new one to make up for the one that died
            Spawn();
        }
        // while (currentNumberOfEnemies != maxNumberOfEnemies)
        // {
        //     Spawn();
        //     yield return new WaitForSeconds(3.0f);
        // }
    }

    public void SpawnFromDumpList(){
        //TODO: The problem with this is that //nevermind, nothing should spawn that hasn't already spawned since it's being dynamically added;
       SpawnFromList(enemiesThatWerentSpawned) ;
        

    }

    public IEnumerator SpawnInitialWave()
    {
        yield return new WaitForSeconds(7.0f);
        int numberSpawnedSoFar = 0;
        while (numberSpawnedSoFar <= initialSpawnNumber)
        {
            //This will spawn until we reach the initial spawn number. It will stop trying to spawn until the player kills an enemy or an enemy otherwise dies
            numberSpawnedSoFar++;
            //while we still have enemies to spawn in our first wave and there is still room 
            if (currentNumberOfEnemies != maxNumberOfEnemies)
            {
                Spawn();
            }
            //spawn a new enemy, and wait four seconds before spawning another
            yield return new WaitForSeconds(3.0f);
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
        // int runningPowerLevelOfGroupToSpawn = 0;
        // //grab a random number from the enum that corresponds to a type
        // EnemyGroup.EnemyTypes potentialType = (EnemyGroup.EnemyTypes)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyGroup.EnemyTypes)).Length);
        // int potentialPowerCap = enemyGroup.correspondingEnemyThreatLevel[potentialType];
        // if (potentialPowerCap + runningPowerLevelOfGroupToSpawn <= spawnRoundPowerCap)
        // {
        //     //if this enemy type's power level + how much our current power level is is less than the power cap we want for this group
        //     //choose this spacemonster to spawn
        //     //            enemyToSpawn = enemiesToAppearOnThisLevel.correspondingSpaceMonster[potentialType].GetPooledInstance<SpaceMonster>();

        //     //add the power cap to the total
        //     runningPowerLevelOfGroupToSpawn += potentialPowerCap;
        //     // newEnemiesToSpawn.Add(newEnemy) ;;
        // }
        // else
        // {

        //     MakeSureUnderPowerCap(spawnRoundPowerCap, currentPowerLevel);

        // }

        return enemyToSpawn;

    }

    void SpawnFromList(List<int> listToSpawnFrom){

        SpaceMonster enemyToSpawn;
        List<int> ourCurrentWaveList = listToSpawnFrom;
        int count = listToSpawnFrom.Count;
        int randomIndex = UnityEngine.Random.Range(0, count);

        int typeWeWant = listToSpawnFrom[randomIndex];
        //this is going to choose a random integer which corresponds to a certain type of enemy
        SpaceMonster ourSpaceMonster = enemyGroup.correspondingSpaceMonster[typeWeWant];

        enemyToSpawn = ourSpaceMonster.GetPooledInstance<SpaceMonster>();
        enemyToSpawn.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);

        Type ourType = ourSpaceMonster.GetType();
        if (!enemyDirectory.ContainsKey(ourType))
        {
            List<GameObject> newGOsOfThisSpaceMonsterTypeList = new List<GameObject>();
            enemyDirectory.Add(ourType, newGOsOfThisSpaceMonsterTypeList);
            enemyDirectory[ourType].Add(enemyToSpawn.gameObject);

        }
        else
        {

            enemyDirectory[ourType].Add(enemyToSpawn.gameObject);
        }
    }
    void Spawn()
    {
        SpaceMonster enemyToSpawn;
        List<int> ourCurrentWaveList = enemiesAndWaves[currentWave];
        int count = ourCurrentWaveList.Count;
        int randomIndex = UnityEngine.Random.Range(0, count);

        int typeWeWant = ourCurrentWaveList[randomIndex];
        //this is going to choose a random integer which corresponds to a certain type of enemy
        List<int> endList = enemiesAndWaves.Last();

        

        //TODO: This doesn't quite work -- you could end up with a wave with a count of the same and then it doesn't add a new list
        if (endList.Count == levelWaves[currentWave].numberPerWave)
        {
            //if our end list is full

            //endlist is now equal to a new list
            endList = new List<int>();

            //add this integer we're using to spawn to the end
            enemiesAndWaves.Add(endList);

            //add the new list to tne end
            endList.Add(typeWeWant);

            //remove the integer we want from the index
            enemiesAndWaves[currentWave].RemoveAt(randomIndex);

        }
        else if (enemiesAndWaves.Last().Count < maxEnemiesPerWave)
        {

            //if the last list is less than max enemies per wave
            endList.Add(typeWeWant);
            //add that old enemy to this new list
            enemiesAndWaves[currentWave].RemoveAt(randomIndex);
        }

        SpaceMonster ourSpaceMonster = enemyGroup.correspondingSpaceMonster[typeWeWant];

        // EnemyGroup.EnemyTypes potentialType = (EnemyGroup.EnemyTypes)(typeWeWant);
        // object spaceMonsterObject = enemyGroup.correspondingSpaceMonster[potentialType];
        // SpaceMonster ourSpaceMonster = spaceMonsterObject as SpaceMonster;
        enemyToSpawn = ourSpaceMonster.GetPooledInstance<SpaceMonster>();
        enemyToSpawn.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);
        //.:w

        Type ourType = ourSpaceMonster.GetType();
        if (!enemyDirectory.ContainsKey(ourType))
        {
            List<GameObject> newGOsOfThisSpaceMonsterTypeList = new List<GameObject>();
            enemyDirectory.Add(ourType, newGOsOfThisSpaceMonsterTypeList);
            enemyDirectory[ourType].Add(enemyToSpawn.gameObject);

        }
        else
        {

            enemyDirectory[ourType].Add(enemyToSpawn.gameObject);
        }
        currentNumberOfEnemies++;

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
            //TODO: FIX THIS number and whole method 
            SpaceMonster genericMonster_ = MakeSureUnderPowerCap(numberSpawnedAtOnce, /*currentPowerLevel*/ 10);//enemiesToAppearOnThisLevel.enemyTypes[UnityEngine.Random.Range(0, enemiesToAppearOnThisLevel.enemyTypes.Count - 1)];

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
        for (int i = 0; i < numberSpawnedAtOnce; i++)
        {
            if (currentNumberOfEnemies == maxNumberOfEnemies)
            {
                atEnemyCapacity = true;
                break;
            }
            SpaceMonster genericMonster_ = enemyGroup.enemyTypes[UnityEngine.Random.Range(0, enemyGroup.enemyTypes.Count - 1)];

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
