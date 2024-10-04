using System;
using System.Linq;
using UnityEngine;
using UniRx;
using BanpoFri;


public class AdsAbilitySystem
{
    public enum AdAbilityState
    {
        Hide,
        Show,
        Active,
        Wait,
    }

    public enum AdAbilityType
    {
        MONEY,
        CASH,
        INGAMEADSMONEY,
    }


    public class AdAbilityValues
    {
        public IReactiveProperty<AdAbilityState> State = new ReactiveProperty<AdAbilityState>(AdAbilityState.Hide);
        public float showStartTime;
        public float showEndTime;
        public float activeStartTime;
        public float activeEndTime;
        public System.Numerics.BigInteger Value;
        public System.Numerics.BigInteger Value2;
        public Action OnResetValue = null;
        public Action OnActiveValue = null;
        public Action OnBeforeShow = null;
        public Action OnTimeOutHide = null;
        public float BeforeTime = -1f;
        private bool beforeCallback = false;
        public Action<int, int> readyRemindTime = null;
        public Func<bool> CheckShowCondition = null;
        public IReactiveProperty<int> activeRemindTime = new ReactiveProperty<int>();
        public bool ContinuousOpen = false;
        public bool ReadyShow = false;
        private float DiffTime = 0f;

        public void Update()
        {
            if (GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>() == null) return;
            if (GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage == null) return;

            if (GameRoot.Instance.TutorialSystem.IsActive())
            {
                var diff = Time.time - DiffTime;
                DiffTime = Time.time;
                showStartTime += diff;
                showEndTime += diff;
                //OnResetValue?.Invoke();
                return;
            }
            switch (State.Value)
            {
                case AdAbilityState.Hide:
                    {
                        if (ReadyShow)
                        {
                            var curTime = Time.time;
                            var diffStart = showStartTime - curTime;
                            if (diffStart < 0f)
                            {
                                if (CheckShowCondition == null || CheckShowCondition())
                                {
                                    State.Value = AdAbilityState.Show;
                                    readyRemindTime?.Invoke((int)(showEndTime - showStartTime), (int)(showEndTime - showStartTime));
                                }
                            }
                            else if (BeforeTime != -1f && !beforeCallback && BeforeTime > diffStart)
                            {
                                OnBeforeShow?.Invoke();
                                beforeCallback = true;
                            }
                        }
                    }
                    break;
                case AdAbilityState.Show:
                    {
                        if (!ContinuousOpen)
                        {
                            var curTime = Time.time;
                            var diffEnd = showEndTime - curTime;
                            if (diffEnd < 0)
                            {
                                Hide();
                                if (OnTimeOutHide != null)
                                    OnTimeOutHide();
                            }
                            else
                                readyRemindTime?.Invoke((int)diffEnd, (int)(showEndTime - showStartTime));
                        }
                    }
                    break;
                case AdAbilityState.Active:
                    {
                        var curTime = Time.time;
                        var diffEnd = activeEndTime - curTime;
                        activeRemindTime.Value = (int)diffEnd;

                        if (diffEnd < 0)
                        {
                            Hide();
                        }

                    }
                    break;
            }
            DiffTime = Time.time;
        }

        public void ForceShow()
        {
            var diff = showEndTime - showStartTime;
            showStartTime = Time.time;
            showEndTime = showStartTime + diff;
            State.Value = AdAbilityState.Show;
        }

        public void Active()
        {
            State.Value = AdAbilityState.Active;
            OnActiveValue?.Invoke();
        }

        public void Wait()
        {
            State.Value = AdAbilityState.Wait;
        }

        public void Hide()
        {
            State.Value = AdAbilityState.Hide;
            ReadyShow = false;
            OnResetValue?.Invoke();
            beforeCallback = false;
        }

        public void Clear()
        {
            beforeCallback = false;
            OnResetValue = null;
            OnActiveValue = null;
            State.Value = AdAbilityState.Hide;
            readyRemindTime = null;
            activeRemindTime.Value = 0;
            ReadyShow = false;
            ContinuousOpen = false;
        }
    }

    public AdAbilityValues Money = new AdAbilityValues();
    public AdAbilityValues Cash = new AdAbilityValues();
    private CompositeDisposable disposables = new CompositeDisposable();
    private bool vipAction = false;

    public float ads_gem_reward_cooltime { get; private set; }
    public float ads_reward_time { get; private set; }


    public void ClearAbility()
    {
        Money.Hide();
        Cash.Hide();
    }

    public void Init()
    {
        ads_gem_reward_cooltime = Tables.Instance.GetTable<Define>().GetData("ads_gem_reward_cooltime").value;
        ads_reward_time = Tables.Instance.GetTable<Define>().GetData("ads_reward_time").value;

        disposables.Clear();
        Money.Clear();
        Cash.Clear();

        Cash.State.Subscribe(x => {
            if (x == AdAbilityState.Show)
            {
                //GameRoot.Instance.UserData.CurMode.StageData.AdsShowCount += 1;
                Cash.Value = CalcCurrencyAbility((int)Config.CurrencyID.Cash);
            }
        }).AddTo(disposables);

        Cash.OnResetValue = () =>
        {
            var curTime = Time.time;
            Cash.showStartTime = curTime + ads_gem_reward_cooltime;
            Cash.showEndTime = Money.showStartTime + ads_reward_time;
            Cash.ReadyShow = true;
        };
        Cash.OnActiveValue = () =>
        {
            Cash.Hide();
        };


        //

        Money.State.Subscribe(x => {
            if (x == AdAbilityState.Show)
            {
                //GameRoot.Instance.UserData.CurMode.StageData.AdsShowCount += 1;
                Money.Value = CalcCurrencyAbility((int)Config.CurrencyID.Money);
            }
        }).AddTo(disposables);

        Money.OnResetValue = () =>
        {
            var curTime = Time.time;
            Money.showStartTime = curTime + ads_gem_reward_cooltime;
            Money.showEndTime = Money.showStartTime + ads_reward_time;
            Money.ReadyShow = true;
        };
        Money.OnActiveValue = () =>
        {
            Money.Hide();
        };

        //AddWegan.OnResetValue = () =>
        //{
        //    var curTime = Time.time;
        //    AddWegan.showStartTime = curTime + ads_add_cooltime;
        //    AddWegan.showEndTime = AddWegan.showStartTime + ads_watchable_time;
        //    AddWegan.ReadyShow = true;
        //};
        //AddWegan.OnActiveValue = () =>
        //{
        //    AddWegan.Hide();

        //    for (int i = 0; i < ads_add_value; ++i)
        //    {
        //        GameRoot.Instance.InGameSystem.GetInGame<InGameBase>().Stage.AddTrain();
        //    }
        //};

        //AddWegan.CheckShowCondition = () => GameRoot.Instance.TrainSystem.IsTrainAddPossible();

        //AddWegan.Hide();

        //Attack.OnResetValue = () =>
        //{
        //    Attack.Value = 1;
        //    var curTime = Time.time;
        //    Attack.showStartTime = curTime + ads_damage_triple_cooltime;
        //    Attack.showEndTime = Attack.showStartTime + ads_watchable_time;
        //    Attack.ReadyShow = true;
        //};
        //Attack.OnActiveValue = () =>
        //{
        //    var curTime = Time.time;
        //    Attack.activeStartTime = curTime;
        //    Attack.activeEndTime = curTime + ads_damage_triple_time;
        //    Attack.activeRemindTime.Value = ads_damage_triple_time;
        //    Attack.Value = ads_damage_triple_value / 10;
        //};

        //Attack.Hide();
    }

    public void AddAbsAbilityStateListener(AdAbilityType type, Action<AdAbilityState> stateCallback,
        Action<int, int> readyRemindTime = null, Action<int> activeRemindTime = null)
    {
        AdAbilityValues target = null;
        switch (type)
        {
            case AdAbilityType.MONEY:
                target = Money;
                break;
            case AdAbilityType.CASH:
                target = Cash;
                break;
        }
        if (target != null)
        {
            target.State.Subscribe(stateCallback).AddTo(disposables);
            if (readyRemindTime != null)
            {
                target.readyRemindTime += readyRemindTime;
            }
            if (activeRemindTime != null)
                target.activeRemindTime.Subscribe(activeRemindTime).AddTo(disposables);
        }
    }

    public void RemoveAbsAbilityStateListener(AdAbilityType type)
    {
        AdAbilityValues target = null;
        switch (type)
        {
            case AdAbilityType.MONEY:
                target = Money;
                break;
            case AdAbilityType.CASH:
                target = Cash;
                break;
        }
        if (target != null)
        {
            target.readyRemindTime = null;
            disposables.Clear();
        }
    }

    public System.Numerics.BigInteger GetValue(AdAbilityType type)
    {
        AdAbilityValues target = null;
        switch (type)
        {
            case AdAbilityType.MONEY:
                target = Money;
                break;
            case AdAbilityType.CASH:
                target = Cash;
                break;
        }
        if (target != null)
            return target.Value;

        return 0;
    }

    public AdAbilityValues GetAbilityValues(AdAbilityType type)
    {
        switch (type)
        {
            case AdAbilityType.MONEY:
                return Money;
            case AdAbilityType.CASH:
                return Cash;
        }
        return null;
    }

    public void OnOffSwitch(AdAbilityType type, bool value)
    {
        switch (type)
        {
            case AdAbilityType.MONEY:
                if (value)
                    Money.Active();
                else
                    Money.Hide();
                break;
            case AdAbilityType.CASH:
                if (value)
                    Cash.Active();
                else
                    Cash.Hide();
                break;
        }
    }

    public void SetWait(AdAbilityType type)
    {
        switch (type)
        {
            case AdAbilityType.MONEY:
                Money.Wait();
                break;
            case AdAbilityType.CASH:
                Cash.Wait();
                break;
        }
    }

    public void ForceShow(AdAbilityType type)
    {
        switch (type)
        {
            case AdAbilityType.MONEY:
                Money.ForceShow();
                break;
            case AdAbilityType.CASH:
                Cash.ForceShow();
                break;
        }
    }

    public System.Numerics.BigInteger CalcCurrencyAbility(int currencyidx)
    {
        var td = Tables.Instance.GetTable<AdsInGameInfo>().GetData(currencyidx);


        System.Numerics.BigInteger returnvalue = 0;

        if (td != null)
        {
            switch (currencyidx)
            {
                case (int)Config.CurrencyID.Money:
                    {
                        //var upgradecount = GameRoot.Instance.UserData.CurMode.StageData.UpgradeClickCount;

                        int level = 0;

                        for(int i = 0; i < GameRoot.Instance.UserData.CurMode.PlayerWeapon.WeaponList.Count; ++i)
                        {
                            for(int j = 0; j < GameRoot.Instance.UserData.CurMode.PlayerWeapon.WeaponList[i].WeaponUpgradeList.Count; ++j)
                            {
                                level += GameRoot.Instance.UserData.CurMode.PlayerWeapon.WeaponList[i].WeaponUpgradeList[j].Level;
                            }
                        }

                        System.Numerics.BigInteger multiplefactor1 = td.multiple_value * level;

                        returnvalue = (td.value_1 * multiplefactor1) / 100;
                        return returnvalue;
                    }
                case (int)Config.CurrencyID.Cash:
                    {
                        return td.value_1;
                    }
            }



        }

        return 0;
    }

    public void UpdateOneSecond()
    {
        Cash.Update();
        Money.Update();
    }
}
