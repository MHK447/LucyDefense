using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BanpoFri;
using UniRx;
using System.Linq;

public class InGameTycoon : InGameMode
{
  
    //private ObjectPool<Dust> dustPool = new ObjectPool<Dust>();
    //private List<BubbleReward> activeBubbles = new List<BubbleReward>();
    //private List<Dust> activeDusts = new List<Dust>();


    [HideInInspector]
    public InGameStage curInGameStage;


    public IReactiveProperty<int> FarsightedTimeProperty = new ReactiveProperty<int>(0);
    public IReactiveProperty<bool> MaxMode = new ReactiveProperty<bool>(true);
    public IReactiveProperty<float> GameSpeedMultiValue = new ReactiveProperty<float>(1f);
    //private int dustMaxCnt = 20;


    private int ProductHeroIdxs = 0;
    public override void Load()
    {
        base.Load();




        Addressables.InstantiateAsync("InGame1_1").Completed += (handle) =>
        {
            curInGameStage = handle.Result.GetComponent<InGameStage>();
            if (curInGameStage != null)
            {
                curInGameStage.Init();
            }
        };

        //CalculateGameSpeed();

    }

    //public void CalculateGameSpeed()
    //{
    //    var buffValue = GameRoot.Instance.BuffSystem.GetValueByBuffType(BuffSystem.BuffType.ProductSpeed);
    //    GameSpeedMultiValue.Value = 1f - buffValue;
    //}

    protected override void LoadUI()
    {
        base.LoadUI();

        GameRoot.Instance.UISystem.OpenUI<HUDTotal>();
        GameRoot.Instance.UISystem.OpenUI<PageLobbyBattle>(popup => popup.Init());
        GameRoot.Instance.InGameSystem.InitPopups();
    }


    public override void UnLoad()
    {
        base.UnLoad();
    }

}
