using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;

public enum UpgradeComponentType
{
    NormalRare = 1,
    Unique,
    LegendEpic,
    SpawnPercent,
}

[UIPath("UI/Popup/PopupUnitUpgrade")]
public class PopupUnitUpgrade : UIBase
{

    [SerializeField]
    private List<UnitUpgradeComponent> UpgradeComponentList = new List<UnitUpgradeComponent>();

    [SerializeField]
    private Text MoneyText;

    [SerializeField]
    private Text GachaCoinText;

    [SerializeField]
    private Button PercentInfoBtn;

    [SerializeField]
    private ProbabilityComponent probabilityComponent;

    private bool IsPrecentInfo = false;

    protected override void Awake()
    {
        base.Awake();

        GameRoot.Instance.UserData.CurMode.EnergyMoney.Subscribe(x => {
            MoneyText.text = x.ToString();
        }).AddTo(this);

        GameRoot.Instance.UserData.CurMode.GachaCoin.Subscribe(x => {
            GachaCoinText.text = x.ToString();
        }).AddTo(this);

        PercentInfoBtn.onClick.AddListener(OnClickPercentInfo);
    }

    private void OnClickPercentInfo()
    {
        IsPrecentInfo = !IsPrecentInfo;

        var UnitUpgradeData = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == (int)UpgradeComponentType.SpawnPercent);

        probabilityComponent.Init(UnitUpgradeData.Level);

        ProjectUtility.SetActiveCheck(probabilityComponent.gameObject, IsPrecentInfo);

    }

    public void Init()
    {
        for (int i = 0; i < UpgradeComponentList.Count; ++i)
        {
            UpgradeComponentList[i].Set((UpgradeComponentType)i + 1);
        }

        ProjectUtility.SetActiveCheck(probabilityComponent.gameObject, false);
    }
}
