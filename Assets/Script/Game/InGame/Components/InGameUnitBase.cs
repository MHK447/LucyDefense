using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;using GoogleMobileAds;
using GoogleMobileAds.Api;

public class InGameUnitBase : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attack,
        Move,
    }


    [System.Serializable]
    public class UnitActiveSkillInfo
    {
        public int UnitSkillIdx;
        public int UnitSkillRatio;
        public int UnitSkillValue;
        public int DebuffType;


        public UnitActiveSkillInfo(int unitskillidx , int unitskillratio , int unitskillvalue , int debufftype)
        {
            UnitSkillIdx = unitskillidx;
            UnitSkillRatio = unitskillratio;
            UnitSkillValue = unitskillvalue;
            DebuffType = debufftype;
        }
    }


    [System.Serializable]
    public class Info
    {
        public float AttackRange;
        public double Attack;
        public float criticalChance;
        public float AttackSpeed;
        public List<UnitActiveSkillInfo> UnitSkillInfoList = new List<UnitActiveSkillInfo>();
    }

    [SerializeField]
    private Animator Anim;

    [SerializeField]
    private AnimFunction AttackAnimAction;

    private Vector3 SkillOffset = new Vector3(0,1,0);

    public SpriteRenderer attackRangeIndicator;

    [SerializeField]
    protected Info info = new Info();

    private float radioussize = 1.82f;

    private int UnitIdx = 0;

    public int GetUnitIdx { get { return UnitIdx; } }

    private float lastattacktime = 0f;

    private InGameBattle Battle;

    private State CurState = State.Idle;

    private string AnimStr;

    private InGameEnemyBase Target;

    private UnitTileComponent CurTileComponent;

    private Transform MoveTr;

    [HideInInspector]
    public Transform AttackRangeTr;

    private bool facingRight = false;

    private CompositeDisposable disposables = new CompositeDisposable();

    private int UpgradeIdx = 0;

    public void Set(int unitidx , InGameBattle battle)
    {
        disposables.Clear();

        Target = null;
        UnitIdx = unitidx;
        Battle = battle;
        AttackAnimAction.FuncAction = StartAttack;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            SetInfo();



            ProjectUtility.SetActiveCheck(attackRangeIndicator.gameObject, false);

            attackRangeIndicator.transform.localScale = new Vector2(info.AttackRange * radioussize, info.AttackRange * radioussize);

            if (td.grade == 1 || td.grade == 2)
            {
                UpgradeIdx = 1;
            }
            else if (td.grade == 3)
            {
                UpgradeIdx = 2;
            }
            else if (td.grade == 4 || td.grade == 5)
            {
                UpgradeIdx = 3;
            }

            var finddata = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == UpgradeIdx);

            finddata.LevelProperty.Subscribe(x => { SetInfo(); }).AddTo(disposables);

            GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.ObserveAdd().Subscribe(x => {
                SetInfo();
                x.Value.LevelProperty.Subscribe(x => { SetInfo(); }).AddTo(disposables);
            }).AddTo(disposables);

            GameRoot.Instance.WaitTimeAndCallback(0.1f, () => {
                ChangeState(State.Idle);
                StartAnim("0_idle");
            });
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


    public void SetInfo()
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(UnitIdx);

        if (td != null)
        {

            info.AttackRange = td.attackrange / 100f;

            var attackvalue = td.attack * GameRoot.Instance.UnitUpgradeSystem.GetUgpradeValue(UpgradeIdx);
            
            var damagebuffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.DAMAGEINCREASE,false);


            var damageprecentvalue = ProjectUtility.GetPercentValue(attackvalue, (float)damagebuffvalue);

            var outgamebuffvalue = GameRoot.Instance.OutGameUnitUpgradeSystem.FindBuffValue(UnitIdx, OutGameUnitUpgradeSystem.SKiillInfoType.AttackPowerIncrease);

            var getbuffvalue = ProjectUtility.GetPercentValue(UnitIdx, outgamebuffvalue);

            float selectgachaattackbuff = GameRoot.Instance.GachaSkillSystem.GetBuffValue(SelectGachaWeaponSkillSystem.GachaWeaponSkillType.AttackPowerIncrease);

            var selectgachadamagepercent = ProjectUtility.GetPercentValue(attackvalue, selectgachaattackbuff);



            info.Attack = td.attack + (int)damageprecentvalue + getbuffvalue + selectgachadamagepercent;

            var criticalbuffvalue = ProjectUtility.GetPercentValue(td.criticalchance,(float)GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.CRITICALPERECENT));

            var outgamecriticalbuffvalue = GameRoot.Instance.OutGameUnitUpgradeSystem.FindBuffValue(UnitIdx, OutGameUnitUpgradeSystem.SKiillInfoType.CriticalDamagePecentIncrease);

            info.criticalChance = (float)td.criticalchance + (float)criticalbuffvalue + outgamecriticalbuffvalue;
            
            var buffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.ATTACKSPEED);

            var attackspeedvalue = td.attackspeed / 100f;

            var speedbuffvalue = (attackspeedvalue * buffvalue) / 100f;

            var outgameattackspeedvalue = GameRoot.Instance.OutGameUnitUpgradeSystem.FindBuffValue(UnitIdx, OutGameUnitUpgradeSystem.SKiillInfoType.AttackSpeedIncrease);

            var getoutgameattackspeed = ProjectUtility.GetPercentValue((float)speedbuffvalue, outgameattackspeedvalue);

            float selectattackspeed = GameRoot.Instance.GachaSkillSystem.GetBuffValue(SelectGachaWeaponSkillSystem.GachaWeaponSkillType.AttackSpeedIncrease);

            var selectgachattackspeedvalue = ProjectUtility.GetPercentValue((float)speedbuffvalue, selectattackspeed);

            info.AttackSpeed = attackspeedvalue + (float)speedbuffvalue + getoutgameattackspeed + selectgachattackspeedvalue; 
           


            info.UnitSkillInfoList.Clear();


            for(int i = 0; i < td.unit_skill.Count; ++i)
            {
                switch(td.unit_skill[i])
                {
                    case (int)UnitSkillSystem.SkillType.DefenseReduction:
                    case (int)UnitSkillSystem.SkillType.AttackPowerIncrease:
                    case (int)UnitSkillSystem.SkillType.MoveSpeedReduction:
                    case (int)UnitSkillSystem.SkillType.AttackCoin:
                    case (int)UnitSkillSystem.SkillType.CoinGainSummon:
                        {
                            GameRoot.Instance.UnitSkillSystem.AddPassiveSkill(td.unit_idx, td.unit_skill[i], td.unit_skil_value[i]);
                        }
                        break;
                }
            }


            var findoutgamedata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(UnitIdx);

            if(findoutgamedata != null)
            {
                for(int i = 0; i < findoutgamedata.UnitLevel; ++i)
                {
                    var outgameunitupgradetd = Tables.Instance.GetTable<OutGameUnitUpgrade>().GetData(new KeyValuePair<int, int>(UnitIdx, i + 1));

                    if(outgameunitupgradetd != null)
                    {

                        if (outgameunitupgradetd.debuff_type > 0)
                        {
                            int outgameskilvalue = outgameunitupgradetd.skill_value / 100;
                            int outgameskilldamage = outgameunitupgradetd.skill_damage / 100;
                            var newskill = new UnitActiveSkillInfo(outgameunitupgradetd.upgrade_type, outgameskilvalue, outgameskilldamage, outgameunitupgradetd.debuff_type);
                            info.UnitSkillInfoList.Add(newskill);
                        }
                    }
                }
            }



        }
        }

    private bool IsCriticalHit()
    {
        if (info.criticalChance <= 0) return false;


        var randvalue = Random.Range(0, 100);

        return randvalue < info.criticalChance;
    }


    public void RemoveUnit()
    {
        ProjectUtility.SetActiveCheck(this.gameObject, false);
        GameRoot.Instance.UnitSkillSystem.RemovePassiveSkill(UnitIdx);
    }

    public void SetTileComponent(UnitTileComponent tilecomponent)
    {
        CurTileComponent = tilecomponent;
    }

    public void TileAttackRangeActive(bool isactive)
    {
        ProjectUtility.SetActiveCheck(attackRangeIndicator.gameObject, isactive);
    }

    public void Attack(InGameEnemyBase enemy , double damage)
    {
        ChangeState(State.Attack);

        for(int i = 0; i < info.UnitSkillInfoList.Count; ++i)
        {
            var isskillon = SkillProbability(info.UnitSkillInfoList[i].UnitSkillRatio);

            if(isskillon)
            {
                var skilleffect = this.transform.position + SkillOffset;

                switch (info.UnitSkillInfoList[i].UnitSkillIdx)
                {
                    case (int)OutGameUnitUpgradeSystem.SKiillInfoType.SwordsFury:
                        {
                            

                            GameRoot.Instance.EffectSystem.MultiPlay<SwordCastEffect>(skilleffect, effect =>
                            {
                                if (this != null)
                                {
                                    ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                    effect.SetAutoRemove(true, 2f);
                                    effect.transform.SetParent(this.transform);
                                }
                            });
                        }
                        break;
                    case (int)OutGameUnitUpgradeSystem.SKiillInfoType.ValiantStrike:
                        {
                            GameRoot.Instance.EffectSystem.MultiPlay<BraveEffect>(skilleffect, effect =>
                            {
                                if (this != null)
                                {
                                    ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                    effect.SetAutoRemove(true, 2f);
                                    effect.transform.SetParent(this.transform);
                                }
                            });
                        }
                        break;
                    case (int)OutGameUnitUpgradeSystem.SKiillInfoType.ShieldBash:
                        {
                            GameRoot.Instance.EffectSystem.MultiPlay<ShieldAttackEffect>(skilleffect, effect =>
                            {
                                if (this != null)
                                {
                                    ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                    effect.SetAutoRemove(true, 2f);
                                    effect.transform.SetParent(this.transform);
                                }
                            });
                        }
                        break;
                    case (int)OutGameUnitUpgradeSystem.SKiillInfoType.GuardiansStance:
                        {
                            GameRoot.Instance.EffectSystem.MultiPlay<GurdianEffect>(skilleffect, effect =>
                            {
                                if (this != null)
                                {
                                    ProjectUtility.SetActiveCheck(effect.gameObject, true);
                                    effect.SetAutoRemove(true, 2f);
                                    effect.transform.SetParent(this.transform);
                                }
                            });
                        }
                        break;
                }
                enemy.DebuffDamage((InGameEnemyBase.DebuffType)info.UnitSkillInfoList[i].DebuffType, info.UnitSkillInfoList[i].UnitSkillValue, info.UnitSkillInfoList[i].UnitSkillValue / 10 );
            }
        }


        if(IsCriticalHit())
        {
            var criticaldamagebuff = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.CRITICALDAMAGE);

            var crticialdamage = (criticaldamagebuff * SkillCardSystem.CrtiticalDamage) / 100;

            var outgamecriticalpercent = GameRoot.Instance.OutGameUnitUpgradeSystem.FindBuffValue(UnitIdx, OutGameUnitUpgradeSystem.SKiillInfoType.CriticalDamagePecentIncrease);

            crticialdamage = SkillCardSystem.CrtiticalDamage + crticialdamage;

            var outgamebuffvalue = ProjectUtility.GetPercentValue((float)crticialdamage, outgamecriticalpercent);

            crticialdamage = crticialdamage + outgamebuffvalue;

            damage = damage * crticialdamage;
        }


        enemy.Damage((int)damage);
    }

    public void DeathUnit()
    {
        ProjectUtility.SetActiveCheck(this.gameObject, false);
    }

    public void ChangeState(State state)
    {
        if (CurState == state) return;

        CurState = state;

        switch (state)
        {
            case State.Idle:
                {
                    Target = null;
                    StartAnim("0_idle");
                }
                break;
            case State.Attack:
                {
                    StartAnim("2_Attack_Normal");
                }
                break;
            case State.Move:
                {
                    Target = null;
                    StartAnim("1_Run");
                }
                break;
        }
    }

    private void StartAnim(string anim)
    {
        if (AnimStr == anim) return;

        if(CurState == State.Attack)
            Anim.SetFloat("AttackSpeed", info.AttackSpeed);

        AnimStr = anim;
        Anim.Play(anim, 0, 0f);
    }

    public void MoveToTile(UnitTileComponent tile , Transform movetr)
    {
        MoveTr = movetr;  
        ChangeState(State.Move);
        CurTileComponent = tile;
        SetTileComponent(tile);
    }

     
    public void StartAttack()
    {
        if(Target != null)
        {
            float distanceToEnemy = Vector3.Distance(AttackRangeTr.position, Target.transform.position);
            if (distanceToEnemy < info.AttackRange)
            {

                Attack(Target, info.Attack);

                if (Target.IsDeath)
                    Target = null;  
            }
            else
            {
                Target = null;
            }
        }
    }



    public bool SkillProbability(int probability)
    {

        int randomValue = Random.Range(0, 100);
        return randomValue < probability;
    }


    void OnDrawGizmos()
    {
        if (CurTileComponent == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackRangeTr.position, info.AttackRange); // 공격 범위는 여기서 설정
    }

        void Update()
    {
        if (CurState == State.Move && MoveTr != null)
        {
            Vector3 direction = MoveTr.position - transform.position;
            direction.Normalize();

            // 좌우 방향 판단
            if (direction.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && facingRight)
            {
                Flip();
            }

            // 유닛을 목표 방향으로 이동
            transform.position += direction * 10f * Time.deltaTime;

            float distance = Vector3.Distance(MoveTr.position, this.transform.position);

            if(distance < 0.1f)
            {
                //이동이후
                ChangeState(State.Idle);
            }


            return;
        }

        if (Target == null)
        {
            Target = Battle.FindClosestEnemy(info.AttackRange, AttackRangeTr);

            if (Target != null)
            {
                Vector3 direction = Target.transform.position - transform.position;
                direction.Normalize();

                // 좌우 방향 판단
                if (direction.x > 0 && !facingRight)
                {
                    Flip();
                }
                else if (direction.x < 0 && facingRight)
                {
                    Flip();
                }

                ChangeState(State.Attack);
            }
            else
            {
                ChangeState(State.Idle);
            }
        }
    }



    void Flip()
    {
        // 유닛의 방향 전환
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
