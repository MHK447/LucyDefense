using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.UI;


[UIPath("UI/InGame/InGameUnitSelect", false, true)]
public class InGameUnitSelect : InGameFloatingUI
{
    [SerializeField]
    private Button SyntheticBtn;

    [SerializeField]
    private Button CloseBtn;

    private int UnitIdx = 0;

    private UnitTileComponent TileComponent;

    private void Awake()
    {
        SyntheticBtn.onClick.AddListener(OnClickSyntheticBtn);
        CloseBtn.onClick.AddListener(OnClickClose);
    }


    public void Set(int unitidx, UnitTileComponent tilecomponent)
    {
        UnitIdx = unitidx;
        TileComponent = tilecomponent;

        SyntheticBtn.interactable = tilecomponent.UnitList.Count >= 3;
    }


    private void OnClickSyntheticBtn()
    {
        TileComponent.UnitMergeUpgrade();
        ProjectUtility.SetActiveCheck(this.gameObject, false);
    }

    private void OnClickClose()
    {
        //var td = Tables.Instance.GetTable<UnitSellInfo>().GetData(UnitIdx);

        //if(td != null)
        //{
            //reward 지급
            TileComponent.DeleteUnit();
            ProjectUtility.SetActiveCheck(this.gameObject, false);
        //}
    }
}
