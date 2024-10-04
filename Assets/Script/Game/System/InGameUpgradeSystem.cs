using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using BanpoFri;
using System.Linq;

public class InGameUpgradeSystem 
{


    public BigInteger CostValue(int upgradeidx , int level = 0)
    {
        BigInteger value = 0;

        var td = Tables.Instance.GetTable<InGameUpgradeInfo>().GetData(upgradeidx);

        if(td != null)
        {
            if (level > 1)
            {
                var upgradevalue = ProjectUtility.FibonacciDynamic(level);

                value = td.base_cost + (td.increase_cost * upgradevalue);
            }
            else
            {
                value = td.base_cost;
            }
        }

        return value;
    }

    public float GetValue(int upgradeidx , int level)
    {
        float value = 0;
        var td = Tables.Instance.GetTable<InGameUpgradeInfo>().GetData(upgradeidx);

        switch (upgradeidx)
        {
            case (int)Config.InGameUpgradeIdx.ATTACKREGEN:
                {
                    if (td != null)
                    {
                        if (level > 1)
                        {
                        
                            value = (float)(td.increase_value * level);

                            value = value / 100;
                        }
                        else
                        {
                            value = 0;
                        }
                    }
                }
                break;

            case (int)Config.InGameUpgradeIdx.ATTACKRANGE:
                {
                    if (td != null)
                    {
                        if (level > 1)
                        {
                            value = (float)td.base_value + (float)(td.increase_value * level);

                            value = value / 100;
                        }
                        else
                        {
                            value = td.base_value / 100;
                        }
                    }
                }
                break;
            case (int)Config.InGameUpgradeIdx.HPREGEN:
            case (int)Config.InGameUpgradeIdx.HP:
            case (int)Config.InGameUpgradeIdx.ATTACK:
            case (int)Config.InGameUpgradeIdx.ATTACKSPEED:
            case (int)Config.InGameUpgradeIdx.CRITICALPERCENT:
            case (int)Config.InGameUpgradeIdx.CRITICALMULTIPLE:
                {
                    if (td != null)
                    {
                        if (level > 1)
                        {
                            var upgradevalue = ProjectUtility.FibonacciDynamic(level);

                            value = (float)td.base_value + (float)(td.increase_value * upgradevalue);

                            value = value / 100;
                        }
                        else
                        {
                            value = 0;
                        }
                    }
                }
                break;
        }
        return value;
    }

    public void LevelUp(int upgradeidx , int level)
    {
        var finddata = FindUpgradeData(upgradeidx);

        if(finddata != null)
        {
            finddata.LevelProperty.Value += level;
        }
        else
        {
            var ingameupgradedata = new InGameUpgradeData(1, upgradeidx);
            GameRoot.Instance.UserData.CurMode.InGameUpgradeDataList.Add(ingameupgradedata);
        }
    }

    public void InitStartClear()
    {
        GameRoot.Instance.UserData.CurMode.InGameUpgradeDataList.Clear();

        var tdlist = Tables.Instance.GetTable<InGameUpgradeInfo>().DataList.ToList();

        foreach(var td in tdlist)
        {
            GameRoot.Instance.UserData.CurMode.InGameUpgradeDataList.Add(new InGameUpgradeData(1, td.upgrade_idx));
        }
    }

    public InGameUpgradeData FindUpgradeData(int upgradeidx)
    {
        return GameRoot.Instance.UserData.CurMode.InGameUpgradeDataList.Find(x => x.UpgradeIdx == upgradeidx);
    }
}
