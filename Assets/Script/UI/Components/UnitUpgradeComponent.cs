using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using System.Linq;

public class UnitUpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private Text UpgradeCostText;

    [SerializeField]
    private Button UpgradeBtn;

    [SerializeField]
    private Button InfoIconBtn;

    private bool IsOpenInfo = false;

    private UpgradeComponentType CurType;

    private InGameUnitUpgradeData UnitUpgradeData;

    private int BaseCostValue = 0;
    private int CurPriceValue = 0;

    private int MaxLevel = 0;

    private void Awake()
    {
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);

        if(InfoIconBtn != null)
        InfoIconBtn.onClick.AddListener(OnClickInfo);
    }

    public void Set(UpgradeComponentType type)
    {
        CurType = type;

        UnitUpgradeData = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == (int)type);


        MaxLevel = int.MaxValue;


        if (type == UpgradeComponentType.SpawnPercent)
        {
            MaxLevel = Tables.Instance.GetTable<UnitGradeInfo>().DataList.Last().level;
        }


        var td = Tables.Instance.GetTable<UnitUpgradeInfo>().GetData((int)CurType);

        if(td != null)
        {
            BaseCostValue = td.cost_value;
            SetCostText();
        }
    }

    private void OnClickInfo()
    {

    }

    private void SetCostText()
    {
        CurPriceValue = BaseCostValue * UnitUpgradeData.Level;

        UpgradeCostText.text = CurPriceValue.ToString();

        LevelText.text = MaxLevel <= UnitUpgradeData.Level ? $"Lv.MAX" : $"Lv.{UnitUpgradeData.Level}";

        UpgradeBtn.interactable = MaxLevel > UnitUpgradeData.Level;
    }

    public void OnClickUpgrade()
    {
        if (UpgradeBtn.interactable == false) return;

        var td = Tables.Instance.GetTable<UnitUpgradeInfo>().GetData((int)CurType);

        if (td != null)
        {
            switch (td.cost_idx)
            {
                case (int)Config.CurrencyID.EnergyMoney:
                    {
                        if (GameRoot.Instance.UserData.CurMode.EnergyMoney.Value >= CurPriceValue)
                        {
                            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.EnergyMoney, -CurPriceValue);
                            UnitUpgradeData.LevelProperty.Value += 1;
                            SetCostText();
                        }
                    }
                    break;
                case (int)Config.CurrencyID.GachaCoin:
                    {
                        if (GameRoot.Instance.UserData.CurMode.GachaCoin.Value >= CurPriceValue)
                        {
                            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin, -CurPriceValue);
                            UnitUpgradeData.LevelProperty.Value += 1;
                            SetCostText();
                        }
                    }
                    break;
            }
        }

    }
}
