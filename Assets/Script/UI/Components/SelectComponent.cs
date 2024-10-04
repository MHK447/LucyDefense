using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class SelectComponent : MonoBehaviour
{
    [SerializeField]
    private Image BulletImg;

    [SerializeField]
    private Text NameText;

    [SerializeField]
    private Text AttackDesc;

    [SerializeField]
    private Button SelectBtn;

    [SerializeField]
    private List<Image> ImageList = new List<Image>();

    [SerializeField]
    private int SkillIdx = 0;

    private System.Action<int> ClickAction = null;

    private void Awake()
    {
        SelectBtn.onClick.AddListener(OnClickSelect);
    }

    public void Set(int skillidx , System.Action<int> clickaction)
    {
        var td = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().GetData(skillidx);

        if(td != null)
        {
            SkillIdx = skillidx;
            ClickAction = clickaction;
            BulletImg.sprite = Config.Instance.GetInGameSkillAtlas(td.icon);
            NameText.text = Tables.Instance.GetTable<Localize>().GetString(td.desc_1);

            foreach(var starimg in ImageList)
            {
                starimg.sprite = Config.Instance.GetIconImg("Icon_ImageIcon_StarGrade_l_Off");
            }

            var findata = GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.ToList().Find(x => x.SkillTypeIdx == skillidx);

            if(findata != null)
            {
                for (int i = 0; i < findata.Level + 1; ++i)
                {
                    ImageList[i].sprite = Config.Instance.GetIconImg("Icon_ImageIcon_StarGrade_l_On");
                }
            }

            var level = findata == null ? 1 : findata.Level;


            if (td.desc_type == 1)
            {
                var buffvalue = td.value_1 + (td.level_buff_value * (level - 1));
               AttackDesc.text =  Tables.Instance.GetTable<Localize>().GetFormat(td.desc_2, buffvalue);
            }
            else
            {
               AttackDesc.text = Tables.Instance.GetTable<Localize>().GetString(td.desc_2);
            }
        }
    }

    public void OnClickSelect()
    {
        ClickAction?.Invoke(SkillIdx);
        
    }

}
