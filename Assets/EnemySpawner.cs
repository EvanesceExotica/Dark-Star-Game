using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{

    public static event Action<GameObject> NewEnemySpawned;

    void SpawnedNewEnemy(GameObject newEnemy){

        if(NewEnemySpawned != null){
            NewEnemySpawned(newEnemy);
        }
    }
    public SpawnEffect spawnEffect;


    public EnemyGroup enemyGroup;
    GameStateHandler gameStateHandler;


    [SerializeField] int maxWavesBeforeRecycle;

    [SerializeField] int indexOfWaveToRecycle;
    [SerializeField] int maxEnemiesPerWave;


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
    [Header("Spawned Enemy Lists")]

    public List<List<GameObject>> enemyDirectoryList = new List<List<GameObject>>();
    public List<GameObject> allEnemies = new List<GameObject>();

    [Header("Wave Lists")]
    public List<List<int>> enemiesAndWaves;
    public List<int> enemiesThatWerentSpawned;

    public List<Wave> levelWaves;






    void RemoveEnemyFromList(GameObject enemyToRemove, SpaceMonster ourType)
    {
        int index = ourType.ID;
        enemyDirectoryList[index].Remove(enemyToRemove);
        allEnemies.Remove(enemyToRemove);
        //currentEnemies.Remove(enemyToRemove);
        currentNumberOfEnemies--;
        StartCoroutine(SpawnBackups());
        UnsubscribeEnemy(ourType);
    }

    void PlaySpawnEffect(GameObject spawnedEnemy)
    {
        SpawnEffect ourEffect = spawnEffect.GetPooledInstance<SpawnEffect>();
        ourEffect.transform.position = spawnedEnemy.transform.position;
        //TODO: make it so that the effect goes back to pool after a while as to not clutter objects

    }

    void SubscribeEnemy(SpaceMonster ourMonster)
    {

        if (ourMonster.GetComponent<Enemy>() != null)
        {
            ourMonster.gameObject.GetComponent<Health>().Died += this.RemoveEnemyFromList;
        }

    }

    void UnsubscribeEnemy(SpaceMonster ourMonster)
    {

        if (ourMonster.GetComponent<Enemy>() != null)
        {
            ourMonster.gameObject.GetComponent<Health>().Died -= this.RemoveEnemyFromList;
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
        enemyDirectoryList = PopulateListAccordingToWave();





    }

    List<List<GameObject>> PopulateListAccordingToWave()
    {
        //TODO: I'm not sure if this method works
        List<List<GameObject>> listToPopulate = new List<List<GameObject>>();

        int highestEnemyNumber = 0;
        for (int i = 0; i < levelWaves.Count - 1; i++)
        {

            //depending on how many are in this index of the ratiosCorrespondedToType, that'll tell us what the highest enemy to appear in this wave is
            int potentialHighest = levelWaves[i].highestEnemyIDToAppearInWave;
            Debug.Log("Potential highest enemy ID is " + potentialHighest);
            if (potentialHighest > highestEnemyNumber)
            {
                highestEnemyNumber = potentialHighest;
            }
        }
        for (int i = 0; i <= highestEnemyNumber; i++)
        {
            listToPopulate.Add(new List<GameObject>());
        }
        return listToPopulate;
    }

    void StartNewWave(int nextWave)
    {
        //take any remaining and drop them in the last list.
        if (enemiesAndWaves[currentWave].Count > 0)
        {
            //if there are still enemies remaining in our list

            if (enemiesThatWerentSpawned != null)
            {
                enemiesThatWerentSpawned.AddRange(enemiesAndWaves[currentWave]);
            }
            else
            {
                enemiesThatWerentSpawned = new List<int>();
                enemiesThatWerentSpawned.AddRange(enemiesAndWaves[currentWave]);
            }
        }
        enemiesAndWaves.RemoveAt(currentWave);
        currentWave = nextWave;
        CheckSpawningProgress();

    }

    void CheckSpawningProgress()
    {

        StartCoroutine(SpawnInitialWave());
    }



    public GameObject GetClosestOfType(SpaceMonster soughtType, GameObject seeker)
    {

        if (allEnemies.Count == 1 && allEnemies[0] == seeker)
        {
            //if there's only one enemy and it's us
            return null;
        }
        GameObject potential = null;
        int indexID = soughtType.ID;
        potential = FindClosest.FindClosestObject(enemyDirectoryList[indexID], seeker);
        return potential;
    }

    public GameObject GetClosestAny(SpaceMonster ourType, GameObject seeker)
    {
        if (allEnemies.Count == 1 && allEnemies[0] == seeker)
        {
            //if there's only one enemy and it's us
            return null;
        }
        GameObject anyEnemy = null;
        anyEnemy = FindClosest.FindClosestObject(allEnemies, seeker);
        return anyEnemy;
    }

    public GameObject GetClosestOther(SpaceMonster ourType, GameObject seeker)
    {
        if (allEnemies.Count == 1 && allEnemies[0] == seeker)
        {
            //if there's only one enemy and it's us
            return null;
        }
        GameObject potentialOther = null;
        List<GameObject> others = new List<GameObject>();
        int ourIndexID = ourType.ID;
        foreach (GameObject go in allEnemies)
        {
            if (go.GetComponent<SpaceMonster>().ID == ourIndexID)
            {
                continue;
            }
            others.Add(go);
        }
        potentialOther = FindClosest.FindClosestObject(others, seeker);
        return potentialOther;
    }






    void Start()
    {
        currentNumberOfEnemies = 0;
        maxNumberOfEnemies = 6;
        StartCoroutine(SpawnInitialWave());
    }

    public IEnumerator SpawnBackups()
    {
        //TODO: MAke this not a magic number v
        yield return new WaitForSeconds(10.0f);
        if (currentNumberOfEnemies != maxNumberOfEnemies)
        {
            if (enemiesAndWaves[currentWave].Count > 0)
            {
                Debug.Log("Current wave count: " + enemiesAndWaves[currentWave].Count);
                //we still have enemies to spawn from the wave
                SpawnFromCurrentWave();
            }
            //if we're not at max capacity, and the current wave still has enemies to choose from, spawn a new one to make up for the one that died
            if (enemiesThatWerentSpawned != null && enemiesThatWerentSpawned.Count > 0)
            {
                //we have enemies from a previous wave who weren't spawned, so use these
                SpawnFromDumpList();
            }
            else if (enemiesAndWaves.Count > Doomclock.numberOfCyclesUntilBarrierBreaks)
            {
                //if an extra list exists on the end of enemiesandwaves, which should be the case
                //spawn from it
                SpawnFromLast();
            }
            else
            {
                Debug.Log("<color=red>Ran out of enemies to spawn for this wave</color>");
            }
        }

    }

    public void SpawnFromLast()
    {
        SpawnFromList(enemiesAndWaves.Last());
    }

    public void SpawnFromCurrentWave()
    {
        SpawnFromList(enemiesAndWaves[currentWave]);
    }

    public void SpawnFromDumpList()
    {
        //TODO: The problem with this is that //nevermind, nothing should spawn that hasn't already spawned since it's being dynamically added;
        SpawnFromList(enemiesThatWerentSpawned);
    }

    public IEnumerator SpawnInitialWave()
    {
        yield return new WaitForSeconds(7.0f);
        int numberSpawnedSoFar = 0;
        List<int> waveWereSpawningFrom = enemiesAndWaves[currentWave];
        int numberInWave = waveWereSpawningFrom.Count;
        while (numberSpawnedSoFar <= maxNumberOfEnemies)
        {
            numberSpawnedSoFar++;
            //while we still have enemies to spawn in our current wave and there is still room 
            if (currentNumberOfEnemies != maxNumberOfEnemies)
            {
                //Spawn();
                SpawnFromCurrentWave();
            }
            //spawn a new enemy, and wait four seconds before spawning another
            yield return new WaitForSeconds(3.0f);
        }

    }
    #region //Deprecated spawn methods
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
    #endregion

    void SpawnFromList(List<int> listToSpawnFrom)
    {

        SpaceMonster enemyToSpawn;
        //this could be either the current wave, the dump wave, or the last wave. It speaks to exactly the enemies we want to spawn this level with their ratios
        List<int> ourCurrentWaveList = listToSpawnFrom;
        int count = listToSpawnFrom.Count;
        int randomIndex = UnityEngine.Random.Range(0, count);
        //spawn randomly from this list of potential enemies
        int intIDOfTypeWeWant = listToSpawnFrom[randomIndex];
        //add an end list to add enemies we want to spawn from if we have no unspawned enemies
        //TODO: This might be able to be changed if we just have the enemies run off at the end of the level, then add them to the final list??
        List<int> endList = enemiesAndWaves.Last();

        if (enemiesAndWaves.Count == Doomclock.numberOfCyclesUntilBarrierBreaks)
        {
            //if the number of cycles is equal to enemiesandWaves.count, which means that a new list hasn't
            //been created to hold the dump 

            //endlist is now equal to a new list
            endList = new List<int>();

            //add this integer we're using to spawn to the end
            enemiesAndWaves.Add(endList);

            //add the new list to tne end
            endList.Add(intIDOfTypeWeWant);

            //remove the integer we want from the index

        }
        else if (enemiesAndWaves.Count > Doomclock.numberOfCyclesUntilBarrierBreaks)
        {
            //if the number of lists we have in enemiesAndWaves is greater than the number of total cycles we have in this level, meaning
            //we added a sort of dump list to the end in case no enemies are ever not spawned
            endList.Add(intIDOfTypeWeWant);
            //add that old enemy to this new list
        }
        //this is going to choose a random integer which corresponds to a certain type of enemy
        SpaceMonster ourSpaceMonster = enemyGroup.correspondingSpaceMonster[intIDOfTypeWeWant];
        enemyToSpawn = ourSpaceMonster.GetPooledInstance<SpaceMonster>();
        enemyToSpawn.transform.position = FindLocationInSafeZone.FindLocationInCircleExclusion(gameStateHandler.darkStar, 3.0f);
        PlaySpawnEffect(enemyToSpawn.gameObject);
        enemyDirectoryList[intIDOfTypeWeWant].Add(enemyToSpawn.gameObject);
        allEnemies.Add(enemyToSpawn.gameObject);


        listToSpawnFrom.RemoveAt(randomIndex);
        //TODO: Fix this update enemies method to also add to a list so we can actively see 
        currentNumberOfEnemies++;
        SubscribeEnemy(ourSpaceMonster);
        SpawnedNewEnemy(ourSpaceMonster.gameObject);
    }
    #region //more deprecated spawn methods
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

    #endregion



    // Update is called once per frame

}
