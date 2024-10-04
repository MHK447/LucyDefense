using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public enum SKillCardIdx
{
    ATTACKSPEED = 1,
    CRITICALDAMAGE,
    CRITICALPERECENT,
    DAMAGEINCREASE,
    SKILLPERCENT,
    WAVEADDCOIN,
    BOSSDAMAGE,
    EPICGAMBLEINCREASE,
    SLOWENEMY,
    ENEMYDEADCOINUP,
    UNITADDINCREASEMAX,
    RAREGAMBLEINCREASE,
    LEGENDGAMBLEINCREASE,
}

public class SkillCardSystem 
{

    public static int CrtiticalDamage = 2;

    public SkillCardData FindSkillCardData(int skillidx)
    {
        return GameRoot.Instance.UserData.CurMode.SkillCardDatas.ToList().Find(x => x.SkillIdx == skillidx);
    }   


    public int GachaUnitCard()
    {
        var tdlist = Tables.Instance.GetTable<SkillCardInfo>().DataList.ToList();

        return Random.Range(0, tdlist.Count);
    }

    public double GetBuffValue(int skillidx , bool ismulti = true)
    {
        double Value = ismulti ? 1 : 0;


        var finddata = FindSkillCardData(skillidx);

        if(finddata != null)
        {
            var skilltd = Tables.Instance.GetTable<SkillCardInfo>().GetData(skillidx);

            if(skilltd != null)
            {
                double basevalue = skilltd.skill_base_value / 10;
                double skillplusvalue = (skilltd.skill_value / 10) * (finddata.Level - 1);

                Value = basevalue + skillplusvalue;
            }
        }
        return Value;
    }


    public void SkillCardLevelUp(int skillidx)
    {
        var finddata = FindSkillCardData(skillidx);

        if(finddata != null)
        {
            finddata.LevelProperty.Value += 1;
        }
        else
        {
            var skillcardata = new SkillCardData(skillidx, 1);

            GameRoot.Instance.UserData.CurMode.SkillCardDatas.Add(skillcardata);
        }
    }
}
