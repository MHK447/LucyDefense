using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;



[UIPath("UI/Popup/PopupInGameUnitInfo")]
public class PopupInGameUnitInfo : UIBase
{
    [SerializeField]
    private Image UnitImage;

    [SerializeField]
    private Text UnitNameText;

    [SerializeField]
    private Text UnitDamageText;

    [SerializeField]
    private Text UnitAttackSpeedText;

    [SerializeField]
    private UnitInfoSkillComponent SKillComponent;

    private int UnitIdx = 0;
   

    public void Set(int unitidx)
    {
        UnitIdx = unitidx;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            UnitNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);
            
            float attackspeed = td.attackspeed / 100f;

            UnitImage.sprite = Config.Instance.GetUnitImg(td.icon);
            UnitDamageText.text = td.attack.ToString();
            UnitAttackSpeedText.text = attackspeed.ToString();

            SKillComponent.Set(td.unit_skill[0]);
        }

    }
}
