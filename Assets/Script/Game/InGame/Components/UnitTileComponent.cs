using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

public class UnitTileComponent : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer SelectTileImg;

    [SerializeField]
    private List<Transform> SpawnList = new List<Transform>();

    [SerializeField]
    private int TileSpawnOrder;

    [SerializeField]
    private Transform AttackArangeTr;

    [HideInInspector]
    public List<InGameUnitBase> UnitList = new List<InGameUnitBase>();

    private int TileUnitIdx = -1;

    private int TileOrder;

    public List<Transform> GetSpawnList { get { return SpawnList; } }

    public Transform GetUnitAttackArangeTr { get { return AttackArangeTr; } }

    public int GetTileSpawnOrder { get { return TileSpawnOrder; } }

    public int GetTileUnitIdx { get { return TileUnitIdx; } }

    public bool IsTileMax { get { return UnitList.Count >= 3; } }

    public void Set(int tileorder)
    {
        TileOrder = tileorder;
    }
        
    public void SpawnTileUnit(InGameUnitBase unit)
    {
        TileUnitIdx = unit.GetUnitIdx;
        UnitList.Add(unit);
        unit.transform.position = SpawnList[UnitList.Count - 1].transform.position;
        unit.SetTileComponent(this);
        unit.AttackRangeTr = GetUnitAttackRange();

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unit.GetUnitIdx);

        if(td != null)
        {
            switch(td.grade)
            {
                case 1:
                    {
                        GameRoot.Instance.EffectSystem.MultiPlay<UnitNormalEffect>(unit.transform.position, effect =>
                        {
                            if (this != null)
                            {
                                ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                effect.SetAutoRemove(true, 1f);
                                effect.transform.SetParent(this.transform);
                            }
                        });
                    }
                    break;
                case 2:
                    {
                        GameRoot.Instance.EffectSystem.MultiPlay<UnitRareEffect>(unit.transform.position, effect =>
                        {
                            if (this != null)
                            {
                                ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                effect.SetAutoRemove(true, 1f);
                                effect.transform.SetParent(this.transform);
                            }
                        });
                    }
                    break;
                case 3:
                    {
                        GameRoot.Instance.EffectSystem.MultiPlay<UnitEpicEffect>(unit.transform.position, effect =>
                        {
                            if (this != null)
                            {
                                ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                effect.SetAutoRemove(true, 1f);
                                effect.transform.SetParent(this.transform);
                            }
                        });
                    }
                    break;
                case 4:
                    {
                        GameRoot.Instance.EffectSystem.MultiPlay<UnitLegendEffect>(unit.transform.position, effect =>
                        {
                            if (this != null)
                            {
                                ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                effect.SetAutoRemove(true, 1f);
                                effect.transform.SetParent(this.transform);
                            }
                        });
                    }
                    break;

            }
        }
    }

    public void SetTileUnitIdx(int unitidx)
    {
        TileUnitIdx = unitidx;
    }

    public Transform GetUnitAttackRange()
    {
        if(UnitList.Count > 0)
        {
            return UnitList[0].attackRangeIndicator.transform;
        }

        return null;
    }

    public void EnableTile()
    {
        ProjectUtility.SetActiveCheck(SelectTileImg.gameObject, true);
    }

    public void DisableTile()
    {
        ProjectUtility.SetActiveCheck(SelectTileImg.gameObject, false);
    }

    public void UnitMergeUpgrade()
    {
        if(UnitList.Count >= 3)
        {
            var firstunit = UnitList[0];

            var unitgrade = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(firstunit.GetUnitIdx).grade;

            var upgradeunitlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == unitgrade + 1);

            if(upgradeunitlist.Count > 0)
            {
                var rand = Random.Range(0, upgradeunitlist.Count);

                var selectunit = upgradeunitlist[rand];

                foreach(var unit in UnitList)
                {
                    unit.RemoveUnit();
                }

                UnitList.Clear();

                GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.MergeAddUnit(selectunit.unit_idx, this);
            }
        }
    }

    public void DeleteUnit()
    {
        foreach (var unit in UnitList)
        {
            unit.RemoveUnit();
        }

        UnitList.Clear();
    }


    public void MoveChangeTileUnit(UnitTileComponent movetotile)
    {
        for(int i = 0; i < UnitList.Count; ++i)
        {
            UnitList[i].MoveToTile(movetotile, movetotile.GetSpawnList[i]);
        }

        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.CurUnitCountCheck();
    }
}
