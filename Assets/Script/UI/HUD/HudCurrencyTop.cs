using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;


public class HudCurrencyTop : MonoBehaviour
{
    [SerializeField]
    private bool IsBuffValueCheck;

    [SerializeField]
    private Text MineralText;

    [SerializeField]
    private Text BattleBoneText;

    [SerializeField]
    private Text BattleGasText;

    [SerializeField]
    private Text CashText;

    [SerializeField]
    private Text PiggyCashText;

    [SerializeField]
    private Text CandyText;

    [SerializeField]
    private Transform CandyIconTr;

    public Transform GetCandyIconTr { get { return CandyIconTr; } }

    [SerializeField]
    private Transform CashIconTr;

    public Transform GetCashIconTr { get { return CashIconTr; } }

    [SerializeField]
    private Transform MoneyIconTr;

    public Transform GetMoneyIconTr { get { return MoneyIconTr; } }




    [SerializeField]
    private Transform MineralIconTr;

    public Transform GetMineralIconTr { get { return MineralIconTr; } }

    [SerializeField]
    private GameObject MineralRoot;

    [SerializeField]
    private GameObject CandyRoot;

    [SerializeField]
    private Text MoneyBuffText;

    [SerializeField]
    private Text MaterialBuffText;

    private void Awake()
    {
        SyncData();
    }


    private void SyncData()
    {
        if (CashText != null)
        {
            CashText.text = GameRoot.Instance.UserData.Cash.Value.ToString();
            GameRoot.Instance.UserData.HUDCash.Subscribe(x =>
            {
                CashText.text = x.ToString();
            }).AddTo(this);

        }


        if (MineralText != null)
        {
            MineralText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);

            GameRoot.Instance.UserData.HUDMoney.Subscribe(x =>
            {
                MineralText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);
            }).AddTo(this);
        }

    }

    

    private void OnEnable()
    {
    }


}
