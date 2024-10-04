using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using DG.Tweening;


[UIPath("UI/Popup/PopupPassiveCardUpgrade")]
public class PopupPassiveCardUpgrade : UIBase
{
    [SerializeField]
    private List<PassiveCardComponent> PassiveCardComponentList = new List<PassiveCardComponent>();

    [SerializeField]
    private Button CardUpgradeBtn;

    [SerializeField]
    private Text CardCostText;

    [SerializeField]
    private ScrollRect ScrollRect = null;

    private int card_upgrade_cost = 0;

    private int CardIdx = 0; 

    protected override void Awake()
    {
        base.Awake();

        CardUpgradeBtn.onClick.AddListener(OnClickUpgrade);

        card_upgrade_cost = Tables.Instance.GetTable<Define>().GetData("card_upgrade_cost").value;
    }


    public void OnClickUpgrade()
    {
        if(card_upgrade_cost <= GameRoot.Instance.UserData.CurMode.Money.Value)
        {
            CardUpgradeBtn.interactable = false;

            StartCoroutine(RandomlyScaleCards());

            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -card_upgrade_cost);
        }
    }


    public void SortingRollBack()
    {
        transform.GetComponent<Canvas>().sortingOrder = UISystem.START_PAGE_SORTING_NUMBER;
    }

    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    IEnumerator RandomlyScaleCards()
    {

        float totalTime = 2.0f; // 총 지속 시간
        float interval = 0.3f; // 활성화/비활성화 간격
        float elapsedTime = 0f; // 경과 시간


        while (elapsedTime < totalTime)
        {
            // 랜덤 유닛 선택
            var randvalue = Random.Range(0, PassiveCardComponentList.Count);

            // 선택된 유닛 활성화
            ProjectUtility.SetActiveCheck(PassiveCardComponentList[randvalue].SelectObj, true);


            yield return new WaitForSeconds(interval); // 0.3초 대기

            // 선택된 유닛 비활성화
            ProjectUtility.SetActiveCheck(PassiveCardComponentList[randvalue].SelectObj, false);
            elapsedTime += interval; // 경과 시간 업데이트
        }


        // Select a random card
        CardIdx = GameRoot.Instance.SkillCardSystem.GachaUnitCard();

        var finddata = PassiveCardComponentList.Find(x => x.GetSkillIdx == CardIdx);

        if (finddata != null)
        {
            GameRoot.Instance.StartCoroutine(ScrollRect.FocusOnItemCoroutine(finddata.transform as RectTransform, 1f, ()=> {
                GameRoot.Instance.SkillCardSystem.SkillCardLevelUp(CardIdx);
                CardUpgradeBtn.interactable = true;

                ProjectUtility.SetActiveCheck(finddata.SelectObj, true);
                GameRoot.Instance.WaitTimeAndCallback(1f, () => { ProjectUtility.SetActiveCheck(finddata.SelectObj, false); });
            }));
        }
    }

    public override void CustomSortingOrder()
    {
        base.CustomSortingOrder();

        transform.GetComponent<Canvas>().sortingOrder = (int)UIBase.HUDTypeTopSorting.POPUPTOP;
    }


    public void Init()
    {
        var tdlist = Tables.Instance.GetTable<SkillCardInfo>().DataList;

        for(int i = 0; i < tdlist.Count; ++i)
        {
            PassiveCardComponentList[i].Set(tdlist[i].skill_idx);
        }
    }
}
