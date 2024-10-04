using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class GachaUnitCardComponent : MonoBehaviour
{
    private int GradeIdx = 0;

    [SerializeField]
    private Text CostValueText;

    [SerializeField]
    private Text ProbabilityText;

    [SerializeField]
    private Button GachaBtn;

    [SerializeField]
    private Image SelectImg;

    [SerializeField]
    private Image SelectUnitImg;

    [SerializeField]
    private GameObject GradeCrownObj;

    [SerializeField]
    private GameObject NoneSelectObj;

    private float time = 0f;

    private float stoptime = 2f;

    private float Ratio = 0f;

    private bool IsGachaStart = false;

    private void Awake()
    {
        GachaBtn.onClick.AddListener(OnClickGacha);
    }

    private void OnClickGacha()
    {
        if (IsGachaStart) return;

        IsGachaStart = true;

        var td = Tables.Instance.GetTable<GachaUnitCardInfo>().GetData(GradeIdx);

        if(td != null)
        {
            if(GameRoot.Instance.UserData.CurMode.GachaCoin.Value >= td.cost_value)
            {
                GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin, -td.cost_value);

                StartCoroutine(UnitGacha(td.grade_info));
            }
        }
    }

    public IEnumerator UnitGacha(int gradeidx)
    {
        yield return new WaitForEndOfFrame();

        ProjectUtility.SetActiveCheck(SelectUnitImg.gameObject, true);
        ProjectUtility.SetActiveCheck(NoneSelectObj, false);
        ProjectUtility.SetActiveCheck(GradeCrownObj, false);

        time = 0;

        stoptime = 1f;

        var findlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();

        float shakeInterval = 0.1f; // 섞이는 간격
        float nextShakeTime = 0f; // 다음 섞임 시간

        while (stoptime > time)
        {
            time += Time.deltaTime;

            if (time >= nextShakeTime)
            {
                nextShakeTime += shakeInterval;
                var randvalue = Random.Range(0, findlist.Count);
                SelectImg.sprite = Config.Instance.GetUnitImg(findlist[randvalue].icon);
            }

            yield return null; // 매 프레임마다 실행
        }


        bool isUnitSelected = Random.Range(0, 100) < Ratio; 

        if(isUnitSelected)
        {
            var unitlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == gradeidx);

            var randidx = Random.Range(0, unitlist.Count);

            SelectImg.sprite = Config.Instance.GetUnitImg(findlist[randidx].icon);

            GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.AddUnit(unitlist[randidx].unit_idx);
        }
        else
        {
            //꽝!
            ProjectUtility.SetActiveCheck(NoneSelectObj, true);
            ProjectUtility.SetActiveCheck(SelectUnitImg.gameObject, false);
        }


        GameRoot.Instance.WaitTimeAndCallback(0.5f, () => {
            IsGachaStart = false;
            ProjectUtility.SetActiveCheck(SelectUnitImg.gameObject, false);
            ProjectUtility.SetActiveCheck(NoneSelectObj, false);
            ProjectUtility.SetActiveCheck(GradeCrownObj, true);
        });
    }


    public void Set(int grade)
    {
        GradeIdx = grade;

        var td = Tables.Instance.GetTable<GachaUnitCardInfo>().GetData(grade);

        if(td != null)
        {
            ProjectUtility.SetActiveCheck(SelectUnitImg.gameObject, false);
            ProjectUtility.SetActiveCheck(NoneSelectObj, false);
            ProjectUtility.SetActiveCheck(GradeCrownObj, true);


            float buffvalue = 0f;


            switch(grade)
            {
                case 1:
                    {
                        buffvalue = (float)GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.RAREGAMBLEINCREASE);
                    }
                    break;
                case 2:
                    {
                        buffvalue = (float)GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.EPICGAMBLEINCREASE);
                    }
                    break;
                case 3:
                    {
                        buffvalue = (float)GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.LEGENDGAMBLEINCREASE);
                    }
                    break;
            }

            var probabilityvalue = td.probability_value / 10f;

            Ratio = probabilityvalue + buffvalue;
            CostValueText.text = td.cost_value.ToString();
            ProbabilityText.text = $"{Ratio}%";

        }
    }
}
