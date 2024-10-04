using System.Collections;
using System;
using System.Numerics;
using System.Collections.Generic;
using UniRx;

public interface IUserDataMode
{
	// Config.Language Language {get; set;}= Config.Language.en;
	IReactiveProperty<BigInteger> Money { get; set; }
	DateTime LastLoginTime { get; set; }
	DateTime CurPlayDateTime { get; set; }
	public StageData StageData { get; set; }
	public PlayerWeapon PlayerWeapon { get; set; }
	List<LabUpgradeData> LABBuffList { get; set; }
	public PlanetData PlanetData { get; set; }
	List<LABUnLockData> LABUnLockDataList { get; set; }
	List<InGameUpgradeData> InGameUpgradeDataList { get; set; }
	IReactiveProperty<BigInteger> EnergyMoney { get; set; }
	IReactiveProperty<int> GachaCoin { get; set; }
	public List<UnitCardData> UnitCardDatas { get; set; }
	public List<InGameUnitUpgradeData> UnitUpgradeDatas { get; set; }
	public List<PassiveSkillData> PassiveSkillDatas { get; set; }
	public IReactiveCollection<SkillCardData> SkillCardDatas { get; set; }
	public IReactiveCollection<OutGameUnitUpgradeData> OutGameUnitUpgradeDatas { get; set; }
	public IReactiveCollection<SelectGachaWeaponSkillData> SelectGachaWeaponSkillDatas { get; set; }
}

public class UserDataMain : IUserDataMode
{
	public IReactiveProperty<BigInteger> Money { get; set; } = new ReactiveProperty<BigInteger>(0);
	public DateTime LastLoginTime { get; set; } = default(DateTime);
	public DateTime CurPlayDateTime { get; set; } = new DateTime(1, 1, 1);
	public StageData StageData { get; set; } = new StageData();
	public PlayerWeapon PlayerWeapon { get; set; } = new PlayerWeapon();
	public List<LabUpgradeData> LABBuffList { get; set; } = new List<LabUpgradeData>();
	public PlanetData PlanetData { get; set; } = new PlanetData();
	public List<LABUnLockData> LABUnLockDataList { get; set; } = new List<LABUnLockData>();
	public List<InGameUpgradeData> InGameUpgradeDataList { get; set; } = new List<InGameUpgradeData>();
	public IReactiveProperty<BigInteger> EnergyMoney { get; set; } = new ReactiveProperty<BigInteger>(0);
	public IReactiveProperty<int> GachaCoin { get; set; } = new ReactiveProperty<int>(0);
	public List<UnitCardData> UnitCardDatas { get; set; } = new List<UnitCardData>();
	public List<InGameUnitUpgradeData> UnitUpgradeDatas { get; set; } = new List<InGameUnitUpgradeData>();
	public List<PassiveSkillData> PassiveSkillDatas { get; set; } = new List<PassiveSkillData>();
	public IReactiveCollection<SkillCardData> SkillCardDatas { get; set; } = new ReactiveCollection<SkillCardData>();
	public IReactiveCollection<OutGameUnitUpgradeData> OutGameUnitUpgradeDatas { get; set; } = new ReactiveCollection<OutGameUnitUpgradeData>();
	public IReactiveCollection<SelectGachaWeaponSkillData> SelectGachaWeaponSkillDatas { get; set; } = new ReactiveCollection<SelectGachaWeaponSkillData>();

}

public class UserDataEvent : UserDataMain
{
}