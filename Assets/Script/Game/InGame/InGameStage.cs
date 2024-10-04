using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Linq;
using BanpoFri;

public class InGameStage : MonoBehaviour
{
    [SerializeField]
    private InGameBattle Battle;

    public bool IsLoadComplete { get; private set; }

    public InGameBattle GetBattle { get { return Battle; } }

    private CompositeDisposable disposable = new CompositeDisposable();

    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();
        Battle.Init();
    }

            
    public void StartBattle(int stageidx)
    {
            GameRoot.Instance.UserData.CurMode.EnergyMoney.Value = GameRoot.Instance.InGameBattleSystem.start_battle_energymoney;
            GameRoot.Instance.UserData.CurMode.Money.Value = 0;
            GameRoot.Instance.UserData.CurMode.StageData.StageIdx = 1;
            GameRoot.Instance.UISystem.OpenUI<PageFade>(page => {
                page.Set(() => {
                    GameRoot.Instance.UISystem.OpenUI<PopupIngameUpgrade>(popup => popup.Init(Battle));

                    var getui = GameRoot.Instance.UISystem.GetUI<HUDTotal>();

                    if (getui != null)
                        getui.Hide();

                    var pagelobbyui = GameRoot.Instance.UISystem.GetUI<PageLobbyBattle>();

                    if (pagelobbyui != null)
                        pagelobbyui.Hide();

                    var pagecardui = GameRoot.Instance.UISystem.GetUI<PopupPassiveCardUpgrade>();

                    if (pagecardui != null)
                        pagecardui.Hide();


                    var outgameunitupgarde = GameRoot.Instance.UISystem.GetUI<PopupOutGameUnitUpgrade>();

                    if (outgameunitupgarde != null)
                        outgameunitupgarde.Hide();


                    GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle = true;

                    GameRoot.Instance.WaitTimeAndCallback(4f, () =>
                    {
                        Battle.StartBattle(1);
                    });
                });
            });
    }


    public void ReturnMainScreen()
    {
        GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value = 0;
        GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle = false;
        GameRoot.Instance.UISystem.OpenUI<PageFade>(page => {
            page.Set(() => {
                GameRoot.Instance.UISystem.OpenUI<PageLobbyBattle>(popup => popup.Init());
                GameRoot.Instance.UISystem.OpenUI<HUDTotal>();
                var getui = GameRoot.Instance.UISystem.GetUI<PopupIngameUpgrade>();

                GameRoot.Instance.UserData.CurMode.StageData.StageEndClear();

                foreach(var unitupgrade in GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas)
                {
                    unitupgrade.LevelProperty.Value = 1;
                }
                var battle = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.Battle;

                battle.InitClear();
                if (getui != null)
                {
                    getui.Hide();
                }
            });
        });
    }

    private void OnDestroy()
    {
        disposable.Clear();
    }
}
