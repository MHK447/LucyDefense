using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
[EffectPath("Effect/IconText", false, true)]
public class IconTextEffect : Effect
{
    [SerializeField]
    private Animator ani;
    [SerializeField]
    private Text text;
    [SerializeField]
    private Image CurrencyImg;

    public void SetText(string _text, int currencyidx ,bool playAni = true, bool iscritical = false)
    {
        text.color = iscritical ? Color.red : Color.white;

        CurrencyImg.sprite = Config.Instance.GetCurrencyImg(currencyidx);

        text.text = $"-{_text}";
        if (playAni) 
            ani.Play("Show", 0, 0f);
    }

    public void Reset()
    {
        ani?.Rebind();
    }
}
