using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;



public class InGameUnitUpgradeSystem
{
    public void Create()
    {
        if(GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Count == 0)
        {
            for(int i = 0; i < (int)UpgradeComponentType.SpawnPercent; ++i)
            {
                var unitupgradedata = new InGameUnitUpgradeData(i + 1);
                GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Add(unitupgradedata);
            }
        }

    }


    public InGameUnitUpgradeData FindUnitUpgradeData(UpgradeComponentType type)
    {
       return GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == (int)type);
    }

    public int GetUgpradeValue(int upgradetypeidx)
    {
        int value = 1;

        var finddata = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == upgradetypeidx);

        if(finddata != null)
        {
            var td = Tables.Instance.GetTable<UnitUpgradeInfo>().GetData(finddata.UpgradeTypeIdx);

            if(td != null)
            {
                value = finddata.LevelProperty.Value * td.upgrade_incrase;

                value = value / 100;
            }
        }

        return value;
    }

}
