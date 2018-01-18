using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamDamage : MonoBehaviour {

    public LayerMask enemyLayermask;

  

    public IEnumerator CastCircle(Vector2 startPoint, float width, Vector2 endPoint, float distance)
    {
        yield return null;
        Vector2 trans = endPoint - startPoint;
        trans.Normalize();
        RaycastHit2D[] ourRayCastHitArray = Physics2D.CircleCastAll(startPoint, width, trans, distance, enemyLayermask);
        foreach (RaycastHit2D hit in ourRayCastHitArray)
        {
            IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
            if (damageableObject != null)
            {

            }
        }
    }


}
