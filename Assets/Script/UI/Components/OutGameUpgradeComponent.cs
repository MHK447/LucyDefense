using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using UniRx;

public class OutGameUpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private Text UnitNameText;

    [SerializeField]
    private Slider UnitCountSlider;

    [SerializeField]
    private Text UnitCountText;

    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private Image UnitImg;

    [SerializeField]
    private GameObject LockObj;

    [SerializeField]
    private Button UnitInfoBtn;

    [SerializeField]
    private int UnitIdx = 0;

    private CompositeDisposable disposables = new CompositeDisposable();

    private void Awake()
    {
        UnitInfoBtn.onClick.AddListener(OnClickUnitInfo);
    }

    public void Init()
    {
        var finddata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(UnitIdx);

        if(finddata != null)
        {
            finddata.UnitCountProperty.Subscribe(x => { InfoSet(); }).AddTo(disposables);
            finddata.UnitLevelProperty.Subscribe(x => { InfoSet(); }).AddTo(disposables);
        }
        else
        {
            GameRoot.Instance.UserData.CurMode.OutGameUnitUpgradeDatas.ObserveAdd().Subscribe(x => {
                if (x.Value.UnitIdx == UnitIdx)
                {
                    x.Value.UnitLevelProperty.Subscribe(x => { InfoSet(); }).AddTo(disposables);
                    x.Value.UnitCountProperty.Subscribe(x => { InfoSet(); }).AddTo(disposables);
                }
            }).AddTo(disposables);
        }

        InfoSet();
    }


    public void InfoSet()
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(UnitIdx);

        if (td != null)
        {
            UnitNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);


            var finddata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(UnitIdx);


            Utility.SetActiveCheck(LockObj, finddata == null);

            float slidevalue = 0f;

            UnitImg.sprite = Config.Instance.GetUnitImg(td.icon);

            int curunitcount = finddata == null ? 0 : finddata.UnitCount;

            int curlevel = finddata == null ? 1 : finddata.UnitLevel;

            var unitleveltd = Tables.Instance.GetTable<OutGameUnitLevelinfo>().GetData(curlevel);

            if (unitleveltd != null)
            {
                slidevalue = (float)curunitcount / (float)unitleveltd.cardcount;
            }
            UnitCountText.text = $"{curunitcount}/{unitleveltd.cardcount}";

            UnitCountSlider.value = slidevalue;

            LevelText.text = Tables.Instance.GetTable<Localize>().GetFormat("unit_level_info", curlevel);
        }
    }


    private void OnClickUnitInfo()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupOutGameUnitUpgradeInfo>(popup => popup.Set(UnitIdx));
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();
    }
}
