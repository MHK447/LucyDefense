using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EffectPath("Effect/WaterRiseEffect", false, true)]
public class WaterRiseEffect : Effect
{
    public void Set(int damage , InGameEnemyBase enemy)
    {
        enemy.Damage(damage);
    }
}
