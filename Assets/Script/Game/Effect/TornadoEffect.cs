using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EffectPath("Effect/TornadoEffect", false, true)]
public class TornadoEffect : MonoBehaviour
{
    public void Set(int damage, InGameEnemyBase enemy)
    {
        enemy.Damage(damage);
    }
}
