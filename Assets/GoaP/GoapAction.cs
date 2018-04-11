using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour
{


    public DarkStar darkStar;
    public GameStateHandler gameStateHandler;
    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;
    private List<Condition> _Preconditions;
    private List<Condition> _Effects;

    public PointEffector2D ourPointEffector2D;
    public ThreatTrigger ourThreatTrigger;

    public bool interrupted;
    public bool incapacitated;
    public bool performing;

    public EnemySpawner enemySpawner;
    public SpaceMonster ourType;

    public Rigidbody2D ourRigidbody2D;

    public Collider2D ourCollider2D;


    public bool setPerformancePrereqs = false;
    private bool inRange = false;

    public float cost = 1f;

    public bool hasVectorTarget;
    public GameObject target;
    public Vector2 vectorTarget;

    public GoapAgent ourGoapAgent;

    public void SetAgentTarget(GameObject target)
    {
        if (ourGoapAgent != null)
        {
            ourGoapAgent.currentTarget = target;
        }
    }

    public GameObject GetCurrentTarget(){
        return ourGoapAgent.currentTarget;
    }
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
        incapacitated = false;
        setPerformancePrereqs = false;
        performing = false;
        reset();
    }

    public abstract void reset();

    public abstract bool isDone();

    public abstract bool checkProceduralPrecondition(GameObject agent);

    public virtual void ImportantEventTriggered(GameObject intruder){
        
    }
   
    public virtual bool perform(GameObject agent){
        if(interrupted){
            performing = false;
        }
        if(incapacitated){
            performing = false;
        }
        return performing;
    }

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
        foreach (KeyValuePair<string, object> kvp in preconditions)
        {
            if (kvp.Key.Equals(key))
            {
                remove = kvp;
            }
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
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
    //
    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }

    public virtual void Awake()
    {
        ourGoapAgent = GetComponent<GoapAgent>();
        ourThreatTrigger = GetComponentInChildren<ThreatTrigger>();
        ourThreatTrigger.threatInArea += this.ImportantEventTriggered;
        gameStateHandler = GameObject.Find("Game State Handler").GetComponent<GameStateHandler>();
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        ourType = GetComponent<SpaceMonster>();
        ourPointEffector2D = GetComponentInChildren<PointEffector2D>();
        ourRigidbody2D = GetComponent<Rigidbody2D>();
        ourCollider2D = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (ourPointEffector2D != null)
        {
            ourPointEffector2D.enabled = false;
        }

    }

}
