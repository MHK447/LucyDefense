using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UniRx;

[UIPath("UI/Popup/PopupIngameUpgrade")]
public class PopupIngameUpgrade : UIBase
{
    [SerializeField]
    private Text WaveText;

    [SerializeField]
    private Text TimeText;  

    [SerializeField]
    private Text WaveEnemyCountText;

    [SerializeField]
    private Slider SliderValue;

    [SerializeField]
    private Button UnitSpawnBtn;

    [SerializeField]
    private Button GachaBtn;

    [SerializeField]
    private Button PauseBtn;

    [SerializeField]
    private Button UpgradeBtn;

    [SerializeField]
    private Text UnitCountText;

    [SerializeField]
    private Text UnitEnergyCostText;

    [SerializeField]
    private Text UnitGachaCoinText;

    [SerializeField]
    private Text EnergyText;

    [SerializeField]
    private GameObject LowHealthObj;

    [SerializeField]
    private WaveClearComponent WaveComponent;

    [SerializeField]
    private Text WaveStartCountText;

    [SerializeField]
    private GameObject WaveStartObj;

    [SerializeField]
    private GameObject BossTimeObj;

    [SerializeField]
    private Text BossTimeText;

    [SerializeField]
    private Button TicketMonsterBtn;

    [SerializeField]
    private Slider LevelSliderValue;

    private CompositeDisposable disposables = new CompositeDisposable();

    private InGameBattle Battle;

    private float Wavedeltime = 0f;

    private int WaveStartCount = 3;

    private float Bossdeltime = 0f;

    private int BossUnitTime = 0;

    private int UnitCount = 0;

    private int unitdeadcount = 0;

    [SerializeField]
    private Text LvText;

    protected override void Awake()
    {
        base.Awake();
        UnitSpawnBtn.onClick.AddListener(OnClickUnitSpawn);
        GachaBtn.onClick.AddListener(OnClickGachaBtn);
        //LegendCardBtn.onClick.AddListener(OnClickLegendCardBtn);
        PauseBtn.onClick.AddListener(OnClickPause);
        TicketMonsterBtn.onClick.AddListener(OnClickTicketMonster);
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
    }

    public void Init(InGameBattle battle)
    {
        WaveStartCount = 3;
        WaveStartCountText.text = WaveStartCount.ToString();
        disposables.Clear();
        ProjectUtility.SetActiveCheck(WaveComponent.gameObject, false);
        ProjectUtility.SetActiveCheck(WaveStartObj, true);
        ProjectUtility.SetActiveCheck(TicketMonsterBtn.gameObject, false);
        GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value = false;


        WaveText.text = Tables.Instance.GetTable<Localize>().GetFormat("str_ingame_wave_num", GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value);
        GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Subscribe(x => {
            WaveText.text = Tables.Instance.GetTable<Localize>().GetFormat("str_ingame_wave_num", x);
        }).AddTo(disposables);

        GameRoot.Instance.UserData.CurMode.StageData.UnitCountPropety.Subscribe(x => { }).AddTo(disposables);

        GameRoot.Instance.UserData.CurMode.StageData.UnitCountPropety.Subscribe(SetSlider).AddTo(disposables);
        ProjectUtility.SetActiveCheck(LowHealthObj, false);

        GameRoot.Instance.UserData.CurMode.StageData.WaveTimeProperty.Subscribe(WaveTime).AddTo(disposables);

        Battle = battle;

        var unitcountbuff = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.UNITADDINCREASEMAX, false);

        UnitCount = GameRoot.Instance.InGameBattleSystem.unit_max_count + (int)unitcountbuff;

        GameRoot.Instance.InGameBattleSystem.CurUnitCountProperty.Subscribe(x => {
            UnitCountText.text = $"{x}/{UnitCount}";
        }).AddTo(disposables);

        GameRoot.Instance.UserData.CurMode.GachaCoin.Subscribe(x => {
            UnitGachaCoinText.text = x.ToString();
        }).AddTo(disposables);

        GameRoot.Instance.UserData.CurMode.EnergyMoney.Subscribe(SetEnergyMoneyText).AddTo(disposables);

        var costvalue = GetSpawnCostValue(GameRoot.Instance.UserData.CurMode.StageData.UnitAddCount);
        UnitEnergyCostText.text = costvalue.ToString();

        ProjectUtility.SetActiveCheck(BossTimeObj, false);

        GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Subscribe(x => {
                ProjectUtility.SetActiveCheck(BossTimeObj, x);
        }).AddTo(disposables);

        GameRoot.Instance.InGameSystem.LevelProperty.Subscribe(x => {

            LvText.text = $"Lv.{x}";
        }).AddTo(disposables);

        GameRoot.Instance.InGameSystem.DeadCount.Subscribe(x => {
            var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;
            var stagetd = Tables.Instance.GetTable<StageCardDrawInfo>().GetData(stageidx);

            if (stagetd != null)
            {
                var find = stagetd.unitdead_count.FindAll(y => y > x).FirstOrDefault();

                if (stagetd.unitdead_count.Contains(x))
                {
                    LevelSliderValue.value = 0;
                    return;
                }

                int beforevalue = 0;

                if (find > 0)
                {
                    unitdeadcount = find;

                    beforevalue = stagetd.unitdead_count.FindAll(y => y < x).LastOrDefault();
                }

                if (unitdeadcount <= 0) return;

                var calcvalue = unitdeadcount - beforevalue;
                var beforecalcvalue = x - beforevalue;
                var value = (float)beforecalcvalue / (float)calcvalue;

                LevelSliderValue.value = value;
            }
        }).AddTo(disposables);
    }

    public void StartWave(int waveidx)
    {
        var td = Tables.Instance.GetTable<StageWaveInfo>().GetData(waveidx);

        if(td != null)
        {
            if (td.unit_idx > 1000)
            {
                GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value = true;
                BossUnitTime = GameRoot.Instance.InGameBattleSystem.boss_unit_time;
                Bossdeltime = 0f;
            }
        }

        ProjectUtility.SetActiveCheck(WaveComponent.gameObject, false);
        ProjectUtility.SetActiveCheck(WaveComponent.gameObject, true);
        WaveComponent.Set(waveidx);
    }

    public void WaveTime(int time)
    {
        if (time == 0 && GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value > 1)
        {
            StartWave(GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value + 1);
        }

        TimeText.text = ProjectUtility.GetTimeStringFormattingShort(time);


    }

    public void SetSlider(int unitcount)
    {
        if (unitcount >= 80 && LowHealthObj.activeSelf == false)
        {
            ProjectUtility.SetActiveCheck(LowHealthObj, true);
        }
        else if(unitcount < 80 && LowHealthObj.activeSelf == true)
        {
            ProjectUtility.SetActiveCheck(LowHealthObj, false);
        }

        if (GameRoot.Instance.InGameBattleSystem.end_unit_count <= unitcount)
        {
            disposables.Clear();
            ProjectUtility.SetActiveCheck(LowHealthObj, false);
            GameRoot.Instance.UISystem.OpenUI<PopupStageResult>(popup => popup.Init(true));
        }

      

        SliderValue.value = (float)unitcount / (float)GameRoot.Instance.InGameBattleSystem.end_unit_count;
        WaveEnemyCountText.text = $"{unitcount}/{GameRoot.Instance.InGameBattleSystem.end_unit_count}";
    }

    public int GetSpawnCostValue(int unitaddcount)
    {
        return GameRoot.Instance.InGameBattleSystem.initial_cost_start + (GameRoot.Instance.InGameBattleSystem.inital_increase_count * unitaddcount);
    }

    private void OnClickGachaBtn()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupGachaUnitCard>(popup => popup.Init());
    }

    private void OnClickLegendCardBtn()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupLegendCombine>(popup => popup.Init());
    }

    private void OnClickPause()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupStageGiveup>(popup => popup.Init());
    }

    private void OnClickTicketMonster()
    {
        var ingamebattle = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle;

        ingamebattle.SpawnEnemy(GameRoot.Instance.InGameSystem.TicketEnemyIdx);

        ProjectUtility.SetActiveCheck(TicketMonsterBtn.gameObject, false);
    }

    private void OnClickUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUnitUpgrade>(popup => popup.Init());
    }
    
    private void OnClickUnitSpawn()
    {

        var costvalue = GetSpawnCostValue(GameRoot.Instance.UserData.CurMode.StageData.UnitAddCount);

        if (GameRoot.Instance.UserData.CurMode.EnergyMoney.Value >= costvalue)
        {
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.EnergyMoney, -costvalue);

            Battle.RandGachaUnit();

            GameRoot.Instance.UserData.CurMode.StageData.UnitAddCount += 1;

            var nextcostvalue = GetSpawnCostValue(GameRoot.Instance.UserData.CurMode.StageData.UnitAddCount);
            UnitEnergyCostText.text = nextcostvalue.ToString();

            UnitEnergyCostText.color = nextcostvalue <= GameRoot.Instance.UserData.CurMode.EnergyMoney.Value ?
                Config.Instance.GetTextColor("TextColor_White"):Config.Instance.GetTextColor("TextColor_Red");


            //GameRoot.Instance.GetAdManager.ShowRewardedAd(() =>
            //{
            //    GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.EnergyMoney, 1000);
            //});

            //GameRoot.Instance.GetAdManager.ShowInterstitialAd();
            
        }
    }


    private void SetEnergyMoneyText(System.Numerics.BigInteger energy)
    {
        EnergyText.text = energy.ToString();

        var nextcostvalue = GetSpawnCostValue(GameRoot.Instance.UserData.CurMode.StageData.UnitAddCount);

        UnitEnergyCostText.color = nextcostvalue <= GameRoot.Instance.UserData.CurMode.EnergyMoney.Value ?
            Config.Instance.GetTextColor("TextColor_White") : Config.Instance.GetTextColor("TextColor_Red");
    }

    private void OnDisable()
    {
        disposables.Clear();
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }


    private void Update()
    {
        if(!GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle) return;

        if(GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value)
        {
            Bossdeltime += Time.deltaTime;

            if(Bossdeltime >= 1f)
            {
                Bossdeltime = 0f;

                BossUnitTime -= 1;

                BossTimeText.text = Utility.GetTimeStringFormattingShort(BossUnitTime);

                if(BossUnitTime <= 0)
                {
                    BossUnitTime = 0;
                    GameRoot.Instance.UISystem.OpenUI<PopupStageGiveup>(popup => popup.Init());
                }
            }
        }

        if (GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle && WaveStartCount > 0)
        {
            Wavedeltime += Time.deltaTime;

            if(Wavedeltime >= 1f)
            {
                Wavedeltime = 0f;
                WaveStartCount -= 1;
                WaveStartCountText.text = WaveStartCount.ToString();

                if(WaveStartCount <= 0)
                {
                    WaveStartCountText.text = "Go!!";
                    Wavedeltime = 0f;

                    GameRoot.Instance.WaitTimeAndCallback(1f, () => {
                        StartWave(1);
                        ProjectUtility.SetActiveCheck(WaveStartObj, false);
                    });
                }
            }
        }
    }
}
