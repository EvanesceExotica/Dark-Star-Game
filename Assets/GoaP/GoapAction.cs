using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour {

public GameStateHandler gameStateHandler;
    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;
    private List<Condition> _Preconditions;
    private List<Condition> _Effects;

    ThreatTrigger ourThreatTrigger;

    public bool interrupted;
    public bool performing; 

public EnemySpawner enemySpawner;
public IGoap ourType;


public bool setPerformancePrereqs = false;
    private bool inRange = false;

    public float cost = 1f;

    public bool hasVectorTarget;
    public GameObject target;
    public Vector2 vectorTarget;

   public GoapAgent ourGoapAgent;

    public GoapAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
        _Preconditions = new List<Condition>();
        _Effects = new List<Condition>();

    }

    public void doReset()
    {
        inRange = false;
        target = null;
        interrupted = false;
        setPerformancePrereqs = false;
        reset();
    }

    public abstract void reset();

    public abstract bool isDone();

    public abstract bool checkProceduralPrecondition(GameObject agent);

    public virtual void importantEventTriggered(GameObject intruder)
    {
       // Debug.Log(ourGoapAgent.name);
        //Debug.Log(ourGoapAgent.currentAction.name + " equals " + this.name + "?");
        //if (ourGoapAgent.currentAction.name != this.name)
        //{
        //    return;
        //}
        interrupted = true;
    }

    public abstract bool perform(GameObject agent);

    public abstract bool requiresInRange();

    public bool isInRange()
    {
        return inRange;
    }

    public void setInRange(bool inRange)
    {
        this.inRange = inRange;
    }

    public void addPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void AddPrecondition(Condition condition)
    {
        _Preconditions.Add(condition);
    }

    public void AddEffect(Condition condition)
    {
        _Effects.Add(condition);
    }
    
    void RemovePrecondition()
    {
        
    }

    void RemoveEffect()
    {

    }

    public void removePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach(KeyValuePair<string, object> kvp in preconditions)
        {
            if (kvp.Key.Equals(key))
            {
                remove = kvp;
            }
        }
        if(!default(KeyValuePair<string, object>).Equals(remove))
        {
            preconditions.Remove(remove);
        }
    }

    public void addEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }


    public void removeEffect(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in effects)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            effects.Remove(remove);
    }
    public List<Condition> _preconditions
    {
        get
        {
            return _Preconditions;
        }
    }
    public List<Condition> _effects
    {
        get
        {
            return _Effects;
        }
    }

    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }

    public virtual void Awake()
    {
        ourThreatTrigger = GetComponentInChildren<ThreatTrigger>();
        ourThreatTrigger.threatInArea += this.importantEventTriggered;
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        ourType = GetComponent<IGoap>();
    }

    private void OnEnable()
    {
        if(ourThreatTrigger == null)
        {
        }
       

    }

    private void OnDisable()
    {

     //   ourThreatTrigger.threatInArea -= this.importantEventTriggered;
    }
    // Use this for initialization
    void Start () {
        ourGoapAgent = GetComponent<GoapAgent>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
