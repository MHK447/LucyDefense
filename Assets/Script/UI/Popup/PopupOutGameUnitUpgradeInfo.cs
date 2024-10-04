using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;


[UIPath("UI/Page/PopupOutGameUnitUpgradeInfo")]
public class PopupOutGameUnitUpgradeInfo : UIBase
{

    [SerializeField]
    private Image UnitImg;

    [SerializeField]
    private Text UnitNameText;

    [SerializeField]
    private Text DamageText;

    [SerializeField]
    private Text SkillCoolTimeText;

    [SerializeField]
    private Text CostValueText;

    [SerializeField]
    private List<OutGameUnitUpgradeInfoComponent> InfoComponentList = new List<OutGameUnitUpgradeInfoComponent>();

    [SerializeField]
    private Button UpgradeBtn;

    private int UnitIdx = 0;

    private int CostValue = 0;

    private int CostCardCount = 0;

    protected override void Awake()
    {
        base.Awake();
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
    }

    public void OnClickUpgrade()
    {
        var finddata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(UnitIdx);


        if (GameRoot.Instance.UserData.CurMode.Money.Value >= CostValue && finddata.UnitCountProperty.Value >= CostCardCount)
        {
            if(finddata != null)
            {

                finddata.UnitLevelProperty.Value += 1;

                finddata.UnitCountProperty.Value = finddata.UnitCountProperty.Value - CostCardCount;
            }

            Set(UnitIdx);
        }
    }


    public void Set(int unitidx)
    {
        UnitIdx = unitidx;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            UnitImg.sprite = Config.Instance.GetUnitImg(td.icon);

            UnitNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);

            DamageText.text = td.attack.ToString();

            var skillcooltime = td.attackspeed / 100f;

            SkillCoolTimeText.text = skillcooltime.ToString();

            var tdlist = Tables.Instance.GetTable<OutGameUnitUpgrade>().DataList.FindAll(x => x.unit_idx == unitidx);

            foreach(var obj in InfoComponentList)
            {
                ProjectUtility.SetActiveCheck(obj.gameObject, false);
            }

            for(int i = 0; i < tdlist.Count; ++i)
            {
                ProjectUtility.SetActiveCheck(InfoComponentList[i].gameObject, true);
                InfoComponentList[i].Set(UnitIdx , tdlist[i].upgrade_type, tdlist[i].level);
            }

            var unitdata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(unitidx);

            int level = unitdata == null ? 1 : unitdata.UnitLevel;

            var outgameunittd = Tables.Instance.GetTable<OutGameUnitLevelinfo>().GetData(level);

            if(outgameunittd != null)
            {
                CostValue = outgameunittd.costvalue;

                CostCardCount = outgameunittd.cardcount;
            }


            UpgradeBtn.interactable = unitdata != null;

            CostValueText.text = CostValue.ToString();

        }

    }
}
