using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.UI;


[UIPath("UI/Popup/PopupLegendCombine")]
public class PopupLegendCombine : UIBase
{
    [SerializeField]
    private List<GameObject> CachedObject = new List<GameObject>();

    [SerializeField]
    private GameObject LegendPrefab;

    [SerializeField]
    private Transform LegendeRoot;

    [SerializeField]
    private List<CombineUnitComponent> CombineComponentList = new List<CombineUnitComponent>();

    [SerializeField]
    private Image SelectUnitImg;

    [SerializeField]
    private Button CombineBtn;

    private InGameBattle Battle;

    private int SelectUnitIdx = 0;

    protected override void Awake()
    {
        base.Awake();
        CombineBtn.onClick.AddListener(OnClickCombineBtn);

        Battle = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle;
    }

    public void Init()
    {
        foreach(var obj in CachedObject)
        {
            ProjectUtility.SetActiveCheck(obj, false);
        }

        var unitlist = GameRoot.Instance.UserData.CurMode.UnitCardDatas;

        foreach(var unit in unitlist)
        {
            var getobj = GetCachedObject().GetComponent<LegendCombineComponent>();

            if (getobj != null)
            {
                ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                getobj.Set(unit.UnitIdx, SelectUnit);
            }
        }

        SelectUnit(unitlist.First().UnitIdx);
    }

    public void OnClickCombineBtn()
    {
        //조합

        var td = Tables.Instance.GetTable<LegendUnitInfo>().GetData(SelectUnitIdx);

        if(td != null)
        {
            for(int i = 0; i < td.combine_unit.Count; ++i)
            {
                var findunit = Battle.FindPlayerUnit(td.combine_unit[i]);

                if(findunit != null)
                {
                    findunit.DeathUnit();
                }
            }

            Battle.AddUnit(td.unit_idx);

            SelectUnit(td.unit_idx);
        }
    }


    public void SelectUnit(int unitidx)
    {
        SelectUnitIdx = unitidx;

        bool iscombinesuccesscheck = true;

        var td = Tables.Instance.GetTable<LegendUnitInfo>().GetData(unitidx);

        if (td != null)
        {
            foreach (var combimg in CombineComponentList)
            {
                ProjectUtility.SetActiveCheck(combimg.RootTr.gameObject, false);
            }

            for (int i = 0; i < td.combine_unit.Count; ++i)
            {
                ProjectUtility.SetActiveCheck(CombineComponentList[i].RootTr.gameObject, true);
                var unitinfotd = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(td.combine_unit[i]);

                if (unitinfotd != null)
                {

                    var findunit = Battle.FindPlayerUnit(td.combine_unit[i]);

                    if (findunit == null)
                        iscombinesuccesscheck = false;

                    CombineComponentList[i].PossessText.text = findunit == null ? Tables.Instance.GetTable<Localize>().GetString("str_noneposses")
                        : Tables.Instance.GetTable<Localize>().GetString("str_posses");

                    CombineComponentList[i].GradeImg.color = Config.Instance.GetUnitGradeColor(unitinfotd.grade);
                    ProjectUtility.SetActiveCheck(CombineComponentList[i].RootTr.gameObject, true);
                    CombineComponentList[i].CombineImg.sprite = Config.Instance.GetUnitImg(unitinfotd.icon);
                }
            }
        }

        ProjectUtility.SetActiveCheck(CombineBtn.gameObject, iscombinesuccesscheck);
    }

    public GameObject GetCachedObject()
    {
        var inst = CachedObject.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(LegendPrefab);
            inst.transform.SetParent(LegendeRoot);
            inst.transform.localScale = Vector3.one;
            CachedObject.Add(inst);
        }

        return inst;
    }
}
