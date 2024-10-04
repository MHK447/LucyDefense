using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;    

[UIPath("UI/Popup/PopupStageResult")]
public class PopupStageResult : UIBase
{
    [SerializeField]
    private Text SuccessTitle;

    [SerializeField]
    private Text FailedTitle;

    [SerializeField]
    private Text RewardText;

    [SerializeField]
    private Button HomeBtn;

    [SerializeField]
    private Button RewardBtn;


    protected override void Awake()
    {
        base.Awake();

        RewardBtn.onClick.AddListener(OnClickHome);
        HomeBtn.onClick.AddListener(OnClickHome);
    }

    public void OnClickHome()
    {
        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value);
        GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value = 0;

        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.ReturnMainScreen();

        Hide();
    }

    public void Init(bool isfailed)
    {
        ProjectUtility.SetActiveCheck(FailedTitle.gameObject, isfailed);
        ProjectUtility.SetActiveCheck(SuccessTitle.gameObject, !isfailed);
        RewardText.text = Utility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value);
        GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle = false;
    }
}
