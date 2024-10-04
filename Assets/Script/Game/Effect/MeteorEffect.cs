using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EffectPath("Effect/MeteorEffect", false, true)]
public class MeteorEffect : Effect
{
    public void Set(int damage)
    {
        float bombrange = 3f;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, bombrange);
        HashSet<GameObject> processedObjects = new HashSet<GameObject>();

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (!processedObjects.Contains(hitCollider.gameObject))
            {
                var getobj = hitCollider.gameObject.GetComponent<InGameEnemyBase>();
                if (getobj != null)
                {
                    getobj.Damage(damage);
                }
            }
        }
    }
}
