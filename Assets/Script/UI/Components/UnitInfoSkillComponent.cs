using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class UnitInfoSkillComponent : MonoBehaviour
{
    [System.Serializable]
    public class SkillBtn
    {
        public Image SkillImg;
        public Button SKillBtn;
        public GameObject LockObj;

        public int SkillIdx = 0;
        public int SkillLevel = 0;
    }

    [SerializeField]
    private Text SkillNameText;

    [SerializeField]
    private Text SkillDescText;


    [SerializeField]
    private List<SkillBtn> SkillBtnList = new List<SkillBtn>();

    private int Unitidx  = 0;

    private int UnitLevel = 0;

    private int CurSelectSkillIdx = 0;


    private void Awake()
    {
        //int iter = 1;

        foreach (var skill in SkillBtnList)
        {
            skill.SKillBtn.onClick.AddListener(()=> { SelectSkill(skill.SkillIdx ,skill.SkillLevel); });
        }
    }

    public void Set(int unitidx)
    {
        Unitidx = unitidx;


        var outgameunitinfo = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(unitidx);

        foreach(var skill in SkillBtnList)
        {
            ProjectUtility.SetActiveCheck(skill.SKillBtn.gameObject, false);
        }


        var tdlist = Tables.Instance.GetTable<OutGameUnitUpgrade>().DataList.FindAll(x => x.unit_idx == unitidx);


        UnitLevel = outgameunitinfo == null ? 0 : outgameunitinfo.UnitLevel;

        for (int i = 0; i < tdlist.Count; ++i)
        {
            bool isopen = tdlist[i].level <= UnitLevel ? true : false;
            SkillBtnList[i].SkillIdx = tdlist[i].upgrade_type;
            SkillBtnList[i].SkillLevel = i + 1;
            ProjectUtility.SetActiveCheck(SkillBtnList[i].SKillBtn.gameObject, true);
            ProjectUtility.SetActiveCheck(SkillBtnList[i].LockObj, isopen);
        }

        SelectSkill(SkillBtnList.First().SkillIdx, SkillBtnList.First().SkillLevel);

    }


    public void SelectSkill(int skillidx , int unitlevel)
    {
        var td = Tables.Instance.GetTable<UnitOutGameSkillInfo>().GetData(skillidx);

        var outgameupgradetd = Tables.Instance.GetTable<OutGameUnitUpgrade>().GetData(new KeyValuePair<int, int>(Unitidx, unitlevel));

        if(outgameupgradetd != null)
        {
            float skillvalue = outgameupgradetd.skill_value / 100f;
            float skilldamage = outgameupgradetd.skill_damage / 100f;
            SkillNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);
            SkillDescText.text = Tables.Instance.GetTable<Localize>().GetFormat(td.ingame_select_desc, skillvalue, skilldamage);
        }
    }

}
