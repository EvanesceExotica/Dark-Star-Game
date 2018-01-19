using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateHandler : MonoBehaviour {

    public static GameStateHandler instance = null; 


    int score;
    int soulValue;

    int lives;

    public List<Health> enemyHealths = new List<Health>();

    public EnemySpawner spawner;

    public int numberOfEnemiesLeft;

    public bool noEnemiesLeft;

    public List<GameObject> enemiesInLevel = new List<GameObject>();

    public List<GameObject> soulsAttachedToPlayer = new List<GameObject>();

    public static GameObject player;
    public GameObject darkStar;
    public DarkStar darkStarComponent;
    public GameObject switchHolder;

    public Rigidbody2D playerRigidbody;

    public GameObject voidCreature; 
    public enum GameState
    {
        normal,
        dark,
        starOpened
    }

    public static GameState currentGameState;

    public static event Action DarkPhaseStarted;

    void BeginDarkPhase()
    {
        currentGameState = GameState.dark;
        if(DarkPhaseStarted != null)
        {
            DarkPhaseStarted();
        }
    }

   
    public static event Action DoorOpened;

    void OpenDoor()
    {
        currentGameState = GameState.starOpened;
        if (DoorOpened != null)
        {
            DoorOpened();
        }
    }

    void RemoveEnemyFromListOnEnemyDeath(GameObject enemyToRemove)
    {
        //enemiesInLevel.Remove(enemyToRemove);
        //numberOfEnemiesLeft--;
        //if(numberOfEnemiesLeft < 0)
        //{
        //    numberOfEnemiesLeft = 0;
        //}
        //if(numberOfEnemiesLeft == 0)
        //{
        //    noEnemiesLeft = true;
        //    OpenDoor();
        //}
    }

    void AddsoulToList(GameObject soulToAdd)
    {
        soulsAttachedToPlayer.Add(soulToAdd);
    }

    void RemovesoulFromList(GameObject soulToRemove)
    {
        soulsAttachedToPlayer.Remove(soulToRemove);
    }



    public static event Action levelCompleted;


    public static void CompleteLevel()
    {
        if(levelCompleted != null)
        {
            levelCompleted();
        }
    }

    public static event Action levelFailed;

    public void FailLevel()
    {
        Debug.Log("<color=red>LEVEL FAILED</color>");
        if(levelFailed != null)
        {
            levelFailed();
        }
    }




    void AggregatePointsandLoadNextLevel()
    {
        //Debug.Log("Level complete!");
        //score = (score + (soulsAttachedToPlayer.Count * soulValue));
        //Debug.Log("Here's the score!" + score);
        CompleteLevel();
    }
  
    void SpawnDemon()
    {

    }

    private void Awake()
    {
        voidCreature = GameObject.Find("VoidBeast");
        soulValue = 15;
        noEnemiesLeft = false;
        if(enemyHealths.Count == 0)
        {
        }
        foreach(Health health in enemyHealths)
        {
            health.Died += this.RemoveEnemyFromListOnEnemyDeath;
        }
        SoulBehavior.AttachToPlayer += this.AddsoulToList;
        SoulBehavior.DetatchFromPlayer += this.RemovesoulFromList;
        Key.KeyGrabbedByPlayer += this.OpenDoor;
       // spawner = GameObject.Find("SpawnerHolder").GetComponent<EnemySpawner>();
        Exit.DoorEntered += this.AggregatePointsandLoadNextLevel;
        DarkStar.IlluminationAtZero += this.FailLevel;

        Conduit.AllSwitchesPowered += this.BeginDarkPhase;

        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        darkStar = GameObject.Find("Dark Star");
        darkStarComponent = darkStar.GetComponent<DarkStar>();
        player = GameObject.Find("Player");
        switchHolder = GameObject.Find("Switch Holder");
        currentGameState = GameState.normal;
        //if the star hits zero illumination, fail level
        DarkStar.IlluminationAtZero += this.FailLevel;

    }

    private void OnDisable()
    {
        foreach (Health health in enemyHealths)
        {
            health.Died -= this.RemoveEnemyFromListOnEnemyDeath;
        }
        SoulBehavior.AttachToPlayer -= this.AddsoulToList;
        SoulBehavior.DetatchFromPlayer-= this.RemovesoulFromList;
    }

    void Start () {
        player = GameObject.Find("Player");
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            BeginDarkPhase();

        }
    
	}
}
