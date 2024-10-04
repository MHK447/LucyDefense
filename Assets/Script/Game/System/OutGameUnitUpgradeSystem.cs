using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;

public class OutGameUnitUpgradeSystem 
{

    public enum SKiillInfoType
    {
        SwordsFury, ShieldBash, GuardiansStance, ValiantStrike, AttackPowerIncrease, AttackSpeedIncrease, SkillDamageIncrease, SkillCooldownReduction, IncreasedBossDamage , CriticalDamagePecentIncrease , CriticalDamageIncrase
    }
    public void AddUnitData(int unitidx , int unitcount)
    {
        var finddata = FindOutGameUnit(unitidx);

        if (finddata != null)
        {
            finddata.UnitCountProperty.Value += unitcount;
        }
        else
        {
            var newdata = new OutGameUnitUpgradeData(unitidx, 1, unitcount);
            GameRoot.Instance.UserData.CurMode.OutGameUnitUpgradeDatas.Add(newdata);
        }
    }


    public float FindBuffValue(int unitidx , SKiillInfoType infotype)
    {
        float buffvalue = 0f;

        var finddata = FindOutGameUnit(unitidx);

        if(finddata != null)
        {
            var unitlist = Tables.Instance.GetTable<OutGameUnitUpgrade>().DataList.FindAll(x => x.unit_idx == unitidx);

            var findskilldata = unitlist.Find(x => x.skill_idx == (int)infotype && finddata.UnitLevel >= x.level);

            if (findskilldata != null)
            {
                buffvalue = findskilldata.skill_value / 100;
            }
        }

        return buffvalue;
    }


    public OutGameUnitUpgradeData FindOutGameUnit(int unitidx)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.OutGameUnitUpgradeDatas.ToList().Find(x => x.UnitIdx == unitidx);

        return finddata;
    }
}
