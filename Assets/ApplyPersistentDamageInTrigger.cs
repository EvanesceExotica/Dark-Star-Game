using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ApplyPersistentDamageInTrigger : MonoBehaviour {

    float startTime;
    public float damage;
    public float tickTime;
    public float lifeTime;
    LayerMask enemyLayermask;

    // public event Action<> applyingPersistentDamage;

    //subscribe on the fly?

    List<IDamageable> raycastHitObjects = new List<IDamageable>();

    List<IDamageable> objectsInTrigger = new List<IDamageable>();
    List<IDamageable> alreadyEffected = new List<IDamageable>();


    private void OnEnable()
    {
        startTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        IDamageable damageableObject = hit.GetComponent<IDamageable>();
        if(damageableObject != null && !objectsInTrigger.Contains(damageableObject))
        {
            AddPersistentDamage(damageableObject);            
        }
    }

    void OnTriggerExit2D(Collider2D hit)
    {
        IDamageable damageableObject = hit.GetComponent<IDamageable>();
        if(damageableObject != null && objectsInTrigger.Contains(damageableObject))
        {
            RemovePersistentDamage(damageableObject);
        }
    }


    void AddPersistentDamage(IDamageable damageableObject)
    {
        damageableObject.AddPersistentDamageSource(damage, lifeTime, tickTime, this.gameObject);
        objectsInTrigger.Add(damageableObject);

    }

    void RemovePersistentDamage(IDamageable damageableObject)
    {

        damageableObject.RemovePersistentDamageSource(this.gameObject);
        objectsInTrigger.Remove(damageableObject);
    }


    public IEnumerator CastCircle(Vector2 startPoint, float width, Vector2 endPoint, float distance)
    {
        yield return null;
        Vector2 trans = endPoint - startPoint;
        trans.Normalize();
        RaycastHit2D[] ourRayCastHitArray = Physics2D.CircleCastAll(startPoint, width, trans, distance, enemyLayermask);
        foreach(RaycastHit2D hit in ourRayCastHitArray)
        {
            IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
            if(damageableObject != null)
            {

            }
        }
    }






    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > startTime + lifeTime)
        {
            gameObject.SetActive(false);
        }
		
	}
}
