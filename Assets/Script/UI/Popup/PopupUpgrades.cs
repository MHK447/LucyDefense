using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UniRx;
using UnityEngine.UI;
using System.Linq;


public enum UPGRADETYPE
{
    ATTACK,
    DEFENCE,
    SPECIAL,
}

[UIPath("UI/Popup/PopupUpgrades")]
public class PopupUpgrades : UIBase
{
    [SerializeField]
    private Transform UpgradeComponentRoot;

    [SerializeField]
    private GameObject UpgradeComponentPrefb;

    [SerializeField]
    private List<GameObject> CachedComponents = new List<GameObject>();

    [SerializeField]
    private List<Toggle> UpgradeToggles = new List<Toggle>();

    public UPGRADETYPE CurrentTab { get; private set; } = UPGRADETYPE.ATTACK;

    private UPGRADETYPE defualtOption = UPGRADETYPE.ATTACK;

    [SerializeField]
    private GameObject SkillRoot;

    [SerializeField]
    private Button SkillUnLockBtn;

    [SerializeField]
    private Text CostText;

    private int CostValue = 0;

    private CompositeDisposable disposables = new CompositeDisposable();

    protected override void Awake()
    {
        base.Awake();
        int iter = 0;
        foreach (var toggle in UpgradeToggles)
        {
            var tabIdx = iter;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(on =>
            {
                ChangeTab((UPGRADETYPE)tabIdx, on);
            });
            ChangeTab((UPGRADETYPE)tabIdx, false);
            ++iter;
        }

        SkillUnLockBtn.onClick.AddListener(OnClickUnLockBtn);

            
    }

    public void SelectTab(UPGRADETYPE tab)
    {
        CurrentTab = tab;

        foreach (var obj in CachedComponents)
        {
            ProjectUtility.SetActiveCheck(obj, false);
        }

        var unlockdata = GameRoot.Instance.UserData.CurMode.LABUnLockDataList.Find(x => x.UpgradeType == (int)tab + 1);

        var tdlist = Tables.Instance.GetTable<PlayerUnitUpgradeInfo>().DataList.FindAll(x => x.upgrade_type == (int)tab + 1 && x.order <= unlockdata.UnLockOrder);

        RectTransform SkillRecT = CachedComponents.FindAll(x => x.gameObject.activeSelf == true).LastOrDefault().transform as RectTransform;

        RectTransform RecT = SkillRoot.transform as RectTransform;

        RecT.anchoredPosition = new Vector2(RecT.anchoredPosition.x, SkillRecT.anchoredPosition.y - 200);

        SkillRoot.transform.SetAsLastSibling();

        if (unlockdata.UnLockOrder == 1)
        {
            var td = Tables.Instance.GetTable<Define>().GetData("upgrades_unlock_cost_1");

            CostValue = td.value;
        }
        else
        {
            CostValue = Tables.Instance.GetTable<Define>().GetData("upgrades_unlock_cost_2").value;
        }

        CostText.text = Utility.CalculateMoneyToString(CostValue);
        Utility.SetActiveCheck(SkillRoot.gameObject, unlockdata.UnLockOrder < 3);


        disposables.Clear();


        GameRoot.Instance.StartCoroutine(SkillPosWaitOneFrame());
    }

    private IEnumerator SkillPosWaitOneFrame()
    {
        yield return new WaitForEndOfFrame(); 
        yield return new WaitForEndOfFrame();


        RectTransform SkillRecT = CachedComponents.FindAll(x => x.gameObject.activeSelf == true).LastOrDefault().transform as RectTransform;

        RectTransform RecT = SkillRoot.transform as RectTransform;

        RecT.anchoredPosition = new Vector2(RecT.anchoredPosition.x, SkillRecT.anchoredPosition.y - 200);

        SkillRoot.transform.SetAsLastSibling();
    }


    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnClickUnLockBtn()
    {
        var unlockdata = GameRoot.Instance.UserData.CurMode.LABUnLockDataList.Find(x => x.UpgradeType == (int)CurrentTab + 1);

        int pricevalue = 0;


        if (unlockdata.UnLockOrder == 1)
        {
            pricevalue = Tables.Instance.GetTable<Define>().GetData("upgrades_unlock_cost_1").value;
        }
        else
        {
            pricevalue = Tables.Instance.GetTable<Define>().GetData("upgrades_unlock_cost_2").value;
        }



        SelectTab(CurrentTab);



    }

    public override void OnShowBefore()
    {
        base.OnShowBefore();
        StartCoroutine(WaitOneFrame());
    }

    IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();

        var viewTab = defualtOption;
        if (!UpgradeToggles[(int)defualtOption].isOn)
        {
            UpgradeToggles[(int)defualtOption].isOn = true;
        }
        else
        {
            var ani = UpgradeToggles[(int)defualtOption].gameObject.GetComponent<Animator>();
            if (ani != null)
            {
                ani.SetTrigger("Selected");
            }
            ChangeTab(defualtOption, true);
        }
    }


    public void ChangeTab(UPGRADETYPE tab, bool on)
    {
        if (CurrentTab == tab) return;

        CurrentTab = tab;

        if (on)
        {
            SelectTab(tab);
        }

        foreach (var toggle in UpgradeToggles)
        {
            var toggleani = toggle.gameObject.GetComponent<Animator>();
            toggleani.SetTrigger("Normal");
        }

        var ani = UpgradeToggles[(int)tab].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            if (on)
            {
                SoundPlayer.Instance.PlaySound("btn");
                if (!UpgradeToggles[(int)tab].isOn)
                    UpgradeToggles[(int)tab].isOn = true;
                ani.SetTrigger("Selected");
            }
            else
                ani.SetTrigger("Normal");
        }
    }


    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(UpgradeComponentPrefb);
            inst.transform.SetParent(UpgradeComponentRoot);
            inst.transform.localScale = Vector3.one;
            CachedComponents.Add(inst);
        }

        return inst;
    }
}
