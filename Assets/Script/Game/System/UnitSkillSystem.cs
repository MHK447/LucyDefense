using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;


public class UnitSkillSystem
{

    public enum SkillType
    {
        DefenseReduction = 4,
        AttackPowerIncrease = 5,
        MoveSpeedReduction = 6,
        AttackCoin = 7,
        CoinGainSummon = 8,
    }


    public int[] PassiveSkillIdxList = {(int)SkillType.DefenseReduction , (int)SkillType.AttackPowerIncrease ,
        (int)SkillType.MoveSpeedReduction , (int)SkillType.CoinGainSummon };

    public void AddPassiveSkill(int unitidx ,int skillidx , int skillvalue)
    {
        GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.Add(new PassiveSkillData(skillidx, skillvalue, unitidx));
    }


    public void RemovePassiveSkill(int unitidx)
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if (td != null)
        {
            foreach (var skillidx in td.unit_skill)
            {
                var findskill = GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.Find(x => x.SkillIdx == skillidx);

                if (findskill != null)
                {
                    GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.Remove(findskill);
                }

            }
        }
    }

    public int FindPassiveSkillValue(int skillidx)
    {
        int skillvalue = 0;


        var findallskill = GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.FindAll(x => x.SkillIdx == skillidx);


        foreach(var findskill in findallskill)
        {
            skillvalue += findskill.SkillValue;
        }

        return skillvalue;
    }
}
