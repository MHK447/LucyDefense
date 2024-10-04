using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;


public class PassiveCardComponent : MonoBehaviour
{
    [SerializeField]
    private Image SkillImg;

    [SerializeField]
    private Text SkillNameText;

    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private GameObject LockObj;

    [SerializeField]
    private Text SkillValueText;

    public GameObject SelectObj;

    private int SkillIdx = 0;

    public int GetSkillIdx { get { return SkillIdx; } }

    private Vector3 InitScale;


    private CompositeDisposable disposables = new CompositeDisposable();

    public void Set(int skillidx)
    {
        SkillIdx = skillidx;

        var td = Tables.Instance.GetTable<SkillCardInfo>().GetData(SkillIdx);

        if(td != null)
        {
            var finddata = GameRoot.Instance.SkillCardSystem.FindSkillCardData(skillidx);

            SkillSet(skillidx);
            disposables.Clear();

            GameRoot.Instance.UserData.CurMode.SkillCardDatas.ObserveAdd().Subscribe(x => {
                if(x.Value.SkillIdx == SkillIdx)
                {
                    SkillSet(SkillIdx);

                    x.Value.LevelProperty.Subscribe(y => { SkillSet(skillidx); }).AddTo(disposables);
                }

            }).AddTo(disposables);


            if(finddata != null)
            {
                finddata.LevelProperty.Subscribe(x => { SkillSet(skillidx); }).AddTo(disposables);
            }

        }
    }


    public void SkillSet(int skillidx)
    {
        var td = Tables.Instance.GetTable<SkillCardInfo>().GetData(SkillIdx);

        if (td != null)
        {
            SkillImg.sprite = Config.Instance.GetSkillAtlas(td.image);
            SkillNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.desc_name);
            var finddata = GameRoot.Instance.SkillCardSystem.FindSkillCardData(skillidx);

            ProjectUtility.SetActiveCheck(LockObj, finddata == null);

            LevelText.text = finddata == null ? "1" : finddata.Level.ToString();

            InitScale = this.transform.localScale;

            SkillValueText.text = Tables.Instance.GetTable<Localize>().GetFormat(td.sign_desc, GameRoot.Instance.SkillCardSystem.GetBuffValue(skillidx, false));
        }
    }


    public void WeaponDireciton()
    {
        this.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f) // 0.5초 동안 1.5배로 스케일 업
                    .OnComplete(() =>
                    {
                        this.transform.DOScale(InitScale, 0.5f); // 0.5초 동안 원래 스케일로 돌아옴
                      });

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
