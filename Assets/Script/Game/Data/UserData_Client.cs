﻿using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using BanpoFri;
using BanpoFri.Data;
public enum DataState
{
	None,
	Main,
	Event
}
public partial class UserDataSystem
{
	public bool Bgm = true;
	public bool Effect = true;
	public bool SlowGraphic = false;
	public Config.Language Language = Config.Language.ko;
	public IReactiveProperty<int> Cash { get; private set; } = new ReactiveProperty<int>(0);
	public IReactiveCollection<string> BuyInappIds {get; private set;} = new ReactiveCollection<string>();
	public IReactiveCollection<string> Tutorial {get; private set;} = new ReactiveCollection<string>();
	public IReactiveCollection<string> GameNotifications { get; private set; } = new ReactiveCollection<string>();
	public Dictionary<string, int> RecordCount {get; private set;} = new Dictionary<string, int>();
	public IUserDataMode CurMode {get; private set;}
	private UserDataMain mainData = new UserDataMain();
	private UserDataEvent eventData = new UserDataEvent();
	public DataState DataState {get; private set;} = DataState.None;
	public bool IsMainState { get { return DataState == DataState.Main; } }
	public IReactiveCollection<int> OneLink { get; private set; } = new ReactiveCollection<int>();

	public IReactiveProperty<BigInteger> HUDMoney = new ReactiveProperty<BigInteger>(0);
	public IReactiveProperty<BigInteger> HudEnergyMoney = new ReactiveProperty<BigInteger>(0);
	public IReactiveProperty<int> HUDCash = new ReactiveProperty<int>(0);


    void ConnectReadOnlyDatas()
    {
        ChangeDataMode(DataState.Main);


        Cash.Value = flatBufferUserData.Cash;
        mainData.Money.Value = BigInteger.Parse(flatBufferUserData.Money);
        mainData.LastLoginTime = new System.DateTime(flatBufferUserData.Lastlogintime);
        mainData.CurPlayDateTime = new System.DateTime(flatBufferUserData.Curplaydatetime);
        mainData.EnergyMoney.Value = BigInteger.Parse(flatBufferUserData.Energymoney);
        mainData.GachaCoin.Value = flatBufferUserData.Gachacoin;
        mainData.StageData.StageHighWave = flatBufferUserData.Highwaveidx;

        mainData.UnitCardDatas.Clear();

        for(int i = 0; i < flatBufferUserData.UnitcarddatasLength; ++i)
        {
            var data = flatBufferUserData.Unitcarddatas(i);

            var newdata = new UnitCardData(data.Value.Unitidx, data.Value.Level, data.Value.Cardcount);

            mainData.UnitCardDatas.Add(newdata);
        }


        mainData.SkillCardDatas.Clear();
        for (int i = 0; i < flatBufferUserData.SkillcarddatasLength; ++i)
        {
            var data = flatBufferUserData.Skillcarddatas(i);

            var newdata = new SkillCardData(data.Value.Skillidx, data.Value.Level);

            mainData.SkillCardDatas.Add(newdata);
        }


        mainData.OutGameUnitUpgradeDatas.Clear();

        for(int i = 0; i  < flatBufferUserData.OutgameunitupgradedatasLength; ++i)
        {
            var data = flatBufferUserData.Outgameunitupgradedatas(i);

            var newdata = new OutGameUnitUpgradeData(data.Value.Unitidx, data.Value.Unitlevel, data.Value.Cardcount);

            mainData.OutGameUnitUpgradeDatas.Add(newdata);
        }

    }


    public UserDataEvent CurEventData { get { return eventData; } }


    public UserDataMain CurMainData { get { return mainData; } }

    private void SnycCollectionToDB<T, U>(IList<T> db, IEnumerable<U> collector) where T : class
    {
        db.Clear();
        foreach (var iter in collector)
        {
            db.Add(iter as T);
        }
    }

    private void SnycCollectionToClient<T, U>(IList<T> db, IEnumerable<U> collector)
    where T : class, IReadOnlyData
    where U : class, IReadOnlyData
    {
        db.Clear();
        foreach (var iter in collector)
        {
            db.Add(iter.Clone() as T);
        }
    }

    public void SyncHUDCurrency(int currencyID = -1)
    {
        if (currencyID < 0)
        {
            HUDMoney.Value = CurMode.Money.Value;
            HudEnergyMoney.Value = CurMode.EnergyMoney.Value;
            HUDCash.Value = Cash.Value;
        }
        else if (currencyID == (int)Config.CurrencyID.Cash)
        {
            HUDCash.Value = Cash.Value;
        }
        else if (currencyID == (int)Config.CurrencyID.EnergyMoney)
        {
            HudEnergyMoney.Value = CurMode.EnergyMoney.Value;
        }
        else if (currencyID == (int)Config.CurrencyID.Money)
        {
            HUDMoney.Value = CurMode.Money.Value;
        }
    }

    public void SetHUDUIReward(int rewardType, int rewardIdx, BigInteger rewardCnt)
    {
        if (rewardType != (int)Config.RewardType.Currency) return;
        switch (rewardIdx)
        {
            case (int)Config.CurrencyID.EnergyMoney:
                {
                    HudEnergyMoney.Value += (int)rewardCnt;
                }
                break;
            case (int)Config.CurrencyID.Money:
                {
                    HUDMoney.Value += rewardCnt;
                }
                break;
            case (int)Config.CurrencyID.Cash:
                {
                    HUDCash.Value += (int)rewardCnt;
                }
                break;
        }
    }

    public void SetReward(int rewardType, int rewardIdx, BigInteger rewardCnt, bool hudRefresh = true)
    {
        switch (rewardType)
        {
            case (int)Config.RewardType.Currency:
                {
                    switch (rewardIdx)
                    {
                        case (int)Config.CurrencyID.Money:
                            {
                                CurMode.Money.Value += rewardCnt;
                            }
                            break;
                        case (int)Config.CurrencyID.Cash:
                            {
                                Cash.Value += (int)rewardCnt;
                            }
                            break;
                        case (int)Config.CurrencyID.EnergyMoney:
                            {   
                                CurMode.EnergyMoney.Value += (int)rewardCnt;
                            }
                            break;
                        case (int)Config.CurrencyID.GachaCoin:
                            {
                                CurMode.GachaCoin.Value += (int)rewardCnt;
                            }
                            break;


                    }
                    if (hudRefresh)
                    {
                        SetHUDUIReward(rewardType, rewardIdx, rewardCnt);
                    }
                }
                break;
           
        }




    }

    public void RefreshUICurrency()
	{
		
	}


	private void TutoDataCheck()
    {

    }
}