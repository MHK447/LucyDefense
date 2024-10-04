using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;



[UIPath("UI/Popup/PopupStageGiveup")]
public class PopupStageGiveup : UIBase
{
    [SerializeField]
    private Button GiveupBtn;

    [SerializeField]
    private Button ContinueBtn;

    [SerializeField]
    private Text GetMineralText;

    [SerializeField]
    private Button DimBtn;

    private int RewardValue = 0;

    protected override void Awake()
    {
        base.Awake();
        GiveupBtn.onClick.AddListener(OnClickGiveUp);
        ContinueBtn.onClick.AddListener(Hide);
        DimBtn.onClick.AddListener(Hide);
    }
        
    public void OnClickGiveUp()
    {
        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, RewardValue);
        RewardValue = 0;
        GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value = 0;
        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.ReturnMainScreen();
        Hide();
    }

    public override void OnShowAfter()
    {
        base.OnShowAfter();
        Time.timeScale = 0f;
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f;
    }

    public void Init()
    {
        var mineralbuffvalue = GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value;

        RewardValue = (int)mineralbuffvalue;

        GetMineralText.text = ProjectUtility.CalculateMoneyToString(RewardValue);
    }

}
