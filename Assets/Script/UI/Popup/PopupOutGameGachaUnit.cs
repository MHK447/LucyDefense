using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;


[UIPath("UI/Popup/PopupOutGameGachaUnit")]
public class PopupOutGameGachaUnit : UIBase
{
    public class GachaUnitInfo
    {
        public int UnitIdx;
        public int UnitCount;

        public GachaUnitInfo(int unitidx,  int unitcount)
        {
            UnitIdx = unitidx;
            UnitCount = unitcount;
        }
    }


    [SerializeField]
    private List<GameObject> CachedComponents = new List<GameObject>();

    [SerializeField]
    private GameObject UpgradeComponentPrefb;

    [SerializeField]
    private Transform UpgradeComponentRoot;


    private List<GachaUnitInfo> GachaUnitInfoList = new List<GachaUnitInfo>();



    public void Set(int unitcount)
    {
        GachaUnitInfoList.Clear();

        foreach(var obj in CachedComponents)
        {
            ProjectUtility.SetActiveCheck(obj, false);
        }


        for(int i = 0; i < unitcount; ++i)
        {
            var getobj = GetCachedObject().GetComponent<OutGameGachaComponent>();

            if(getobj != null)
            {


                int grade = ProjectUtility.GetOutGameGachaGrade();
                var tdlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == grade);
                var randvalue = Random.Range(0, tdlist.Count);

                var unitidx = tdlist[randvalue].unit_idx;

                var finddata = GachaUnitInfoList.Find(x => x.UnitIdx == unitidx);

                if(finddata != null)
                {
                    finddata.UnitCount += 1;
                    
                }
                else
                {
                    var newunitdata = new GachaUnitInfo(unitidx, 1);

                    GachaUnitInfoList.Add(newunitdata);
                }
            }
        }


        foreach(var unitinfo in GachaUnitInfoList)
        {
            var getobj = GetCachedObject().GetComponent<OutGameGachaComponent>();

            if(getobj != null)
            {
                getobj.Set(unitinfo.UnitIdx, unitinfo.UnitCount);

                ProjectUtility.SetActiveCheck(getobj.gameObject, true);

                GameRoot.Instance.OutGameUnitUpgradeSystem.AddUnitData(unitinfo.UnitIdx, unitinfo.UnitCount);
            }
        }
    }



    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(UpgradeComponentPrefb);
            inst.transform.SetParent(UpgradeComponentRoot);
            inst.transform.localScale = Vector3.one;
            CachedComponents.Add(inst);
        }

        return inst;
    }

}
