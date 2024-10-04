using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class OutGameUnitUpgradeInfoComponent : MonoBehaviour
{
    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private Text DescText;

    [SerializeField]
    private GameObject LockObj;


    public void Set(int unitidx,int skillidx , int level)
    {
        var unitdata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(unitidx);


        var td = Tables.Instance.GetTable<UnitOutGameSkillInfo>().GetData(skillidx);

        var outgameupgradetd = Tables.Instance.GetTable<OutGameUnitUpgrade>().GetData(new KeyValuePair<int, int>(unitidx, level));


        if(td != null && outgameupgradetd != null)
        {
            LevelText.text = level.ToString();
            float skillvalue = outgameupgradetd.skill_value / 100f;
            DescText.text = Tables.Instance.GetTable<Localize>().GetFormat(td.name, skillvalue);

            var unitskilllevel = unitdata == null ? 1 : unitdata.UnitLevel;

            ProjectUtility.SetActiveCheck(LockObj, unitskilllevel <= level);
        }
    }
}
