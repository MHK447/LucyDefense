using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using UniRx;


[UIPath("UI/Popup/PopupGachaUnitCard")]
public class PopupGachaUnitCard : UIBase
{
    [SerializeField]
    List<GachaUnitCardComponent> GachaUnitCardComponentList = new List<GachaUnitCardComponent>();

    [SerializeField]
    private Text TicketText;

    private CompositeDisposable disposables = new CompositeDisposable();

    public void Init()
    {
        disposables.Clear();

        GameRoot.Instance.UserData.CurMode.GachaCoin.Subscribe(x => {
            TicketText.text = x.ToString();
        }).AddTo(disposables);

        var tdlist = Tables.Instance.GetTable<GachaUnitCardInfo>().DataList.ToList();

        for(int i = 0; i < tdlist.Count; ++i)
        {
            GachaUnitCardComponentList[i].Set(tdlist[i].grade_info);
        }
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
