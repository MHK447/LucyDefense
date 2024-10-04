using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;


public class OutGameGachaComponent : MonoBehaviour
{
    [SerializeField]
    private Image UnitImg;

    [SerializeField]
    private Text CountText;

    private int UnitIdx = 0;
        
    private int UnitCount = 0;

    public void Set(int unitidx , int unitcount)
    {
        UnitIdx = unitidx;
        UnitCount = unitcount;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            UnitImg.sprite = Config.Instance.GetUnitImg(td.icon);
            CountText.text = unitcount.ToString();


        }
            
    }
}
