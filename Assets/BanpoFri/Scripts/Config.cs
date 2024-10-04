using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using BanpoFri;

[System.Serializable]
public class Config : BanpoFri.SingletonScriptableObject<Config>, BanpoFri.ILoader
{

    public enum LandCondination
    {
        Great,
        Basic,
        Sad,
    }

    public enum Language
    {
        ko,
        en,
        ja,
        th,
        de,
        fr,
    }

    public enum InGameUpgradeIdx
    {
        ATTACK,
        ATTACKSPEED,
        ATTACKRANGE,
        ATTACKREGEN,
        HP,
        HPREGEN,
        CRITICALPERCENT,
        CRITICALMULTIPLE,
    }

    public enum LABUpgradeIdx
    {
        ATTACK,
        ATTACKREGEN,
        ATTACKRANGE,
        HP,
        HPREGEN,
        CRITICALPERCENT,
        CRITICALDAMAGE,
    }

    public enum WeaponUpgradeIdx
    {
        ATTACK = 0,
        ATTACKSPEED = 1,
        ATTACKRANGE = 2,
        ATTACKREGEN = 3,
        CRITICALPERCENT = 6,
        CRITICALDAMAGE = 7,
        DONE,
    }

    public enum WeaponType
    {
        Base = 1,
        TrackEnemy,
    }

    public enum CurrencyID
    {
        Money = 1,
        Cash = 2,
        EnergyMoney = 3,
        GachaCoin = 4,
    }


    public enum RewardType
    {
        Currency = 1,
    }


    public enum ManagerGrade
    {
        Noraml = 1,
        Rare = 2,
        Unique = 3,
        UnCommon = 0,
    }



    [System.Serializable]
    public class ColorDefine
    {
        public string key_string;
        public Color color;
    }

    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _textColorDefines = new List<ColorDefine>();
    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _eventTextColorDefines = new List<ColorDefine>();
    private Dictionary<string, Color> _textColorDefinesDic = new Dictionary<string, Color>();
    public List<ColorDefine> TextColorDefines {
        get {
            return _textColorDefines;
        }
    }
    public List<ColorDefine> EventTextColorDefines {
        get {
            return _eventTextColorDefines;
        }
    }

    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _imageColorDefines = new List<ColorDefine>();
    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _eventImgaeColorDefines = new List<ColorDefine>();
    private Dictionary<string, Color> _imageColorDefinesDic = new Dictionary<string, Color>();
    public List<ColorDefine> ImageColorDefines {
        get {
            return _imageColorDefines;
        }
    }
    public List<ColorDefine> EventImageColorDefines {
        get {
            return _eventImgaeColorDefines;
        }
    }

    public Material SkeletonGraphicMat;
    public Material DisableSpriteMat;
    public Material EnableSpriteMat;
    public Material ImgAddtiveMat;
    [SerializeField]
    private SpriteAtlas DynamicAtlas;
    [SerializeField]
    private SpriteAtlas DynamicShopAtlas;
    [SerializeField]
    private SpriteAtlas ItemCardAtlas;
    [SerializeField]
    private SpriteAtlas CommonAtlas;
    [SerializeField]
    private SpriteAtlas facilityAtlas;
    [SerializeField]
    private SpriteAtlas UiIconAtlas;
    [SerializeField]
    private SpriteAtlas UiBgAtlas;
    [SerializeField]
    private SpriteAtlas InGameAtlas;
    [SerializeField]
    private SpriteAtlas PlanetAtlas;
    [SerializeField]
    private SpriteAtlas IconImgAtlas;
    [SerializeField]
    private SpriteAtlas UnitImgAtlas;
    [SerializeField]
    private SpriteAtlas SkillAtlas;
    [SerializeField]
    private SpriteAtlas InGameSkillAtlas;




    public Color GetTextColor(string key)
    {
        if (_textColorDefinesDic.ContainsKey(key))
            return _textColorDefinesDic[key];

        return Color.white;
    }


    public Color GetImageColor(string key)
    {
        if (_imageColorDefinesDic.ContainsKey(key))
            return _imageColorDefinesDic[key];

        return Color.white;
    }

    public Color GetManagerGradeBGColor(ManagerGrade grade)
    {
        switch (grade)
        {
            case ManagerGrade.Noraml:
                return GetImageColor("Card_Normal_Bg");
            case ManagerGrade.Rare:
                return GetImageColor("Card_Rare_Bg");
            case ManagerGrade.Unique:
                return GetImageColor("Card_Unique_Bg");
        }

        return Color.white;
    }

    public Color GetManagerGradeFrameColor(ManagerGrade grade)
    {
        switch (grade)
        {
            case ManagerGrade.Noraml:
                return GetImageColor("Card_Normal_Frame");
            case ManagerGrade.Rare:
                return GetImageColor("Card_Rare_Frame");
            case ManagerGrade.Unique:
                return GetImageColor("Card_Unique_Frame");
        }

        return Color.white;
    }
    public Sprite GetItemcardImg(string key)
    {
        return ItemCardAtlas.GetSprite(key);
    }
    public Sprite GetUnitImg(string key)
    {
        return UnitImgAtlas.GetSprite(key);
    }

    public Sprite GetPlanetImg(string key)
    {
        return PlanetAtlas.GetSprite(key);
    }


    public Color GetUnitGradeColor(int grade)
    {
        switch(grade)
        {
            case 1:
                return GetImageColor("Unit_Grade_1");
            case 2:
                return GetImageColor("Unit_Grade_2");
            case 3:
                return GetImageColor("Unit_Grade_3");
        }

        return Color.white;
    }

    public Sprite GetCurrencyImg(int currenyidx)
    {
        switch(currenyidx)
        {
            case (int)Config.CurrencyID.Cash:
                return GetCommonImg("Icon_Gem03_Diamond_Purple");
            case (int)Config.CurrencyID.GachaCoin:
                return GetCommonImg("Icon_Cards");
            case (int)Config.CurrencyID.EnergyMoney:
                return GetCommonImg("Icon_Energy_Green");
            case (int)Config.CurrencyID.Money:
                return GetCommonImg("Icon_Gem01_Blue");
        }

        return null;
    }


    public Sprite GetInGameAtlas(string key)
    {
        return InGameAtlas.GetSprite(key);
    }


    public Sprite GetSkillAtlas(string key)
    {
        return SkillAtlas.GetSprite(key);
    }

    public Sprite GetInGameSkillAtlas(string key)
    {
        return InGameSkillAtlas.GetSprite(key);
    }

    public Sprite GetIconImg(string key)
    {
        return IconImgAtlas.GetSprite(key);
    }


    public Sprite GetIUiIconImg(string key)
    {
        return UiIconAtlas.GetSprite(key);
    }

    public Sprite GetUIBgImg(string key)
    {
        return UiBgAtlas.GetSprite(key);
    }


    public Sprite GetCommonImg(string key)
    {
        return CommonAtlas.GetSprite(key);
    }

    public Sprite GetDynamicImg(string key)
    {
        return DynamicAtlas.GetSprite(key);
    }

    public Sprite GetDynamicShop(string key)
    {
        return DynamicShopAtlas.GetSprite(key);
    }


    //public Sprite GetProductImg(string key)
    //{
    //    return productAtlas.GetSprite(key);
    //}

    
    public Sprite GetFacilityImg(string key)
    {
        return facilityAtlas.GetSprite(key);
    }



    public void Load()
    {
        _textColorDefinesDic.Clear();
        foreach(var cd in _textColorDefines)
        {
            _textColorDefinesDic.Add(cd.key_string, cd.color);
        }
        foreach(var cd in _eventTextColorDefines)
        {
            _textColorDefinesDic.Add(cd.key_string, cd.color);
        }
        _imageColorDefinesDic.Clear();
        foreach(var cd in _imageColorDefines)
        {
            _imageColorDefinesDic.Add(cd.key_string, cd.color);
        }
        foreach(var cd in _eventImgaeColorDefines)
        {
            _imageColorDefinesDic.Add(cd.key_string, cd.color);
        }
    }

    public Sprite GetManagerBottomImg(ManagerGrade grade)
    {
        switch(grade)
        {
            case ManagerGrade.Noraml:
                return GetDynamicImg("Dynamic_Bg_ManagerPlace_Normal");
            case ManagerGrade.Rare:
                return GetDynamicImg("Dynamic_Bg_ManagerPlace_Rare");
            case ManagerGrade.Unique:
                return GetDynamicImg("Dynamic_Bg_ManagerPlace_Unique");
        }

        return null;
    }

    public Sprite GetManagerFrameImg(ManagerGrade grade)
    {
        switch(grade)
        {
            case ManagerGrade.Noraml:
                return GetItemcardImg("ItemCard_Frame_Card_Normal_Back");
            case ManagerGrade.Rare:
                return GetItemcardImg("ItemCard_Frame_Card_Rare_Back");
            case ManagerGrade.Unique:
                return GetItemcardImg("ItemCard_Frame_Card_Unique_Back");
        }

        return null;
    }
}
