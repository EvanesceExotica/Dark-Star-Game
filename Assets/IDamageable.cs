using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable 
{
    void RemovePersistentDamageSource(GameObject source);
    void AddPersistentDamageSource(float amount, float duration, float tickTime, GameObject source);
    IEnumerator TakeDamageOverTime(float amount, float duration, float tickTime, GameObject source);
    void adjustCurrentHealth(int adj, GameObject source);

}