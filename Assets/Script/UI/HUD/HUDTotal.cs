using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;

[UIPath("UI/Page/HUDTotal", true)]
public class HUDTotal : UIBase
{
    public enum LobbyTab
    {
        Shop = 0,
        Card = 1,   
        Fight = 2,
        Upgrade = 3,
        Boss = 4,
    }

    [SerializeField]
    private List<Toggle> lobbyToggles = new List<Toggle>();


    public LobbyTab CurrentTab { get; private set; } = LobbyTab.Fight;


    private LobbyTab defualtOption = LobbyTab.Fight;

    protected override void Awake()
    {
        base.Awake();

        int iter = 0;
        foreach (var toggle in lobbyToggles)
        {
            var tabIdx = iter;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(on =>
            {
                ChangeTab((LobbyTab)tabIdx, on);
            });
            ChangeTab((LobbyTab)tabIdx, false);
            ++iter;
        }

    }

    public void SelectTab(LobbyTab tab)
    {

        var passivecardupgrade = GameRoot.Instance.UISystem.GetUI<PopupPassiveCardUpgrade>();

        if (passivecardupgrade != null)
        {
            passivecardupgrade.SortingRollBack();
        }


        var lobbybattle = GameRoot.Instance.UISystem.GetUI<PageLobbyBattle>();

        if (lobbybattle != null)
        {
            lobbybattle.SortingRollBack();
        }

        var upgrade = GameRoot.Instance.UISystem.GetUI<PopupOutGameUnitUpgrade>();

        if(upgrade != null)
        {
            upgrade.SortingRollBack();
        }
        
        switch (tab)
        {
            case LobbyTab.Shop:
                break;
            case LobbyTab.Card:
                {
                    GameRoot.Instance.UISystem.OpenUI<PopupPassiveCardUpgrade>(popup => {
                        popup.Init();
                        popup.CustomSortingOrder();
                    });
                }
                break;
            case LobbyTab.Fight:
                GameRoot.Instance.UISystem.OpenUI<PageLobbyBattle>(popup => {
                    popup.Init();
                    popup.CustomSortingOrder();
                });
                break;
            case LobbyTab.Upgrade:
                {
                    GameRoot.Instance.UISystem.OpenUI<PopupOutGameUnitUpgrade>(popup => {
                        popup.CustomSortingOrder();
                        popup.Init();
                    });
                }
                break;
        }

        foreach (var toggle in lobbyToggles)
        {
            var toggleani = toggle.gameObject.GetComponent<Animator>();
            toggleani.SetTrigger("Normal");
        }

        var ani = lobbyToggles[(int)tab].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            SoundPlayer.Instance.PlaySound("btn");
            ani.SetTrigger("Selected");
        }
    }


    IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();

        var viewTab = defualtOption;
        if (!lobbyToggles[(int)defualtOption].isOn)
        {
            lobbyToggles[(int)defualtOption].isOn = true;
        }
        else
        {
            var ani = lobbyToggles[(int)defualtOption].gameObject.GetComponent<Animator>();
            if (ani != null)
            {
                ani.SetTrigger("Selected");
            }
            ChangeTab(defualtOption, true);
        }
    }

    public void ChangeTab(LobbyTab tab, bool on)
    {
        if (CurrentTab == tab) return;

        CurrentTab = tab;

        if (on)
        {
            SelectTab(tab);
        }

        foreach (var toggle in lobbyToggles)
        {
            var toggleani = toggle.gameObject.GetComponent<Animator>();
            toggleani.SetTrigger("Normal");
        }

        var ani = lobbyToggles[(int)tab].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            if (on)
            {
                if (!lobbyToggles[(int)tab].isOn)
                    lobbyToggles[(int)tab].isOn = true;
                ani.SetTrigger("Selected");
            }
            else
                ani.SetTrigger("Normal");
        }
    }

    public override void OnShowBefore()
    {
        base.OnShowBefore();
        StartCoroutine(WaitOneFrame());
    }
}
