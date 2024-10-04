using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

public class InGameEnemyBase : MonoBehaviour
{
    public enum State
    {
        Done,
        Move,
        Dead,
    }

    public enum DebuffType
    {
        Damage = 1,
        Sturn,
        Slow,
    }

    [System.Serializable]
    public class DebuffInfo
    {
        public DebuffType Type;
        public int Time;
        public int Value;

        public DebuffInfo(DebuffType type , int value , int time)
        {
            Type = type;
            Value = value;
            Time = time;
        }
    }



    [SerializeField]
    protected Animator Anim;

    [SerializeField]
    private Transform HpTr;

    private InGameHpUI HpUI;

    private InGameTimeUI TimeUI;

    private int UnitIdx = 0;

    private InGameBattle.Direction CurDirection;

    private State CurState;

    private List<DebuffInfo> DebuffList = new List<DebuffInfo>();

    public Transform GetHpTr { get { return HpTr; } }

    public bool IsDeath { get { return CurState == State.Dead; } }

    public int GetUnitIdx { get { return UnitIdx; } }

    public int Hp = 0;

    private float MoveSpeed = 0;

    private int MoveSpawnCount = 1;

    private int MoveEndSpawnCount = 4;

    private float ticketdeltime = 0f;

    private float debuffdeltatime = 0f;

    [HideInInspector]
    public InGameBattle Battle;

    private InGameBattle.SpawnTrPos Target;

    private bool IsBoss = false;

    public void Set(int unitidx , InGameBattle battle)
    {
        DebuffList.Clear();

        UnitIdx = unitidx;

        var td = Tables.Instance.GetTable<EnemyInfo>().GetData(UnitIdx);

        if(td != null)
        {
            var curwaveidx = GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value;

            var basehp = GameRoot.Instance.InGameBattleSystem.enemy_normal_base_hp;

            var increasehp = GameRoot.Instance.InGameBattleSystem.enemy_wave_increase_hp / 100;

            var value = basehp  * (increasehp);

            var increase = curwaveidx < 10 ? 1 : (curwaveidx / 10) * 2;

            IsBoss = td.unit_idx > 1000;

            Hp = (int)value * increase;

            var movespeedbuffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.SLOWENEMY,false);

            var movespeedvalue = (float)td.movespeed / 100f;

            var decreasebuffvalue = (movespeedvalue * movespeedbuffvalue) / 100f;

            MoveSpeed = movespeedvalue - (int)decreasebuffvalue;
            MoveSpawnCount = 1;
            Battle = battle;

            Target = Battle.GetSpawnTr(MoveSpawnCount);

            SetInfo();
        }

        SetState(State.Move);
        SetDirection(InGameBattle.Direction.TOP);
        ticketdeltime = 0f;

        if (UnitIdx == GameRoot.Instance.InGameSystem.TicketEnemyIdx)
        {
            if (TimeUI == null)
            {
                GameRoot.Instance.UISystem.LoadFloatingUI<InGameTimeUI>((timeui) =>
                {
                    TimeUI = timeui;
                    timeui.Init(HpTr);
                    ProjectUtility.SetActiveCheck(TimeUI.gameObject, true);
                });
            }
            else
            {
                ProjectUtility.SetActiveCheck(TimeUI.gameObject, true);
            }
        }

        if(HpUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<InGameHpUI>((hpui) => {
                ProjectUtility.SetActiveCheck(hpui.gameObject, true);
                HpUI = hpui;
                hpui.Set(Hp);
                hpui.Init(HpTr);
            });
        }
        else
        {
            ProjectUtility.SetActiveCheck(HpUI.gameObject, true);
            HpUI.Set(Hp);
        }
    }

    public void SetInfo()
    {
        var td = Tables.Instance.GetTable<EnemyInfo>().GetData(UnitIdx);

        var movespeed = (float)td.movespeed / 100f;

        var slowdata = DebuffList.Find(x => x.Type == DebuffType.Slow);

        var slowvalue = slowdata == null ? 0 : slowdata.Value;

        var slowdebuffvalue = (movespeed * slowvalue) / 100;

        MoveSpeed = movespeed - (movespeed * slowdebuffvalue);




    }

    public void Clear()
    {
        if (HpUI != null)
            ProjectUtility.SetActiveCheck(HpUI.gameObject, false);
    }
    public void SetDirection(InGameBattle.Direction direction)
    {
        switch(direction)
        {
            case InGameBattle.Direction.LEFT:
                Anim.Play("Left_Move", 0, 0f);
                break;
            case InGameBattle.Direction.RIGHT:
                Anim.Play("Right_Move", 0, 0f);
                break;
            case InGameBattle.Direction.BOTTOM:
                Anim.Play("Bottom_Move", 0, 0f);
                break;
            case InGameBattle.Direction.TOP:
                Anim.Play("Top_Move", 0, 0f);
                break;
        }
    }

    public void SetState(State state)
    {
        CurState = state;
    }

    public void DebuffDamage(DebuffType debuff , int value , int time)
    {
        switch(debuff)
        {
            case DebuffType.Damage:
                {
                    Damage(value);
                }
                break;
            case DebuffType.Slow:
            case DebuffType.Sturn:
                {
                    var finddata = DebuffList.Find(x => x.Type == debuff);

                    if(finddata != null)
                    {
                        finddata.Time = time;
                    }
                    else
                    {
                        var adddebuff = new DebuffInfo(debuff, value, time);
                        DebuffList.Add(adddebuff);
                    }
                }
                break;
        }
    }

    public void Damage(int damage)
    {
        if (Hp <= 0) return;
        if (IsDeath) return;


        if(IsBoss)
        {
            var buffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.BOSSDAMAGE,false);

            buffvalue = ProjectUtility.GetPercentValue(damage, (float)buffvalue);

            double damagebuffvalue = damage + buffvalue;

            damage = (int)damagebuffvalue;
        }

        Hp -= damage;


        if(HpUI != null)
        {
            HpUI.SetSliderValue(damage);
        }

        if(Hp <= 0)
        {
            Dead();
            GameRoot.Instance.InGameSystem.DrawCardCheck();
        }

        Battle.SetDamageUI(GetHpTr, damage);
    }


    public void Dead()
    {
        if (HpUI != null)
            ProjectUtility.SetActiveCheck(HpUI.gameObject, false);

        if (TimeUI != null)
            ProjectUtility.SetActiveCheck(TimeUI.gameObject, false);

        SetState(State.Dead);
        GameRoot.Instance.UserData.CurMode.StageData.UnitCountPropety.Value -= 1;

        if (UnitIdx == GameRoot.Instance.InGameSystem.TicketEnemyIdx)
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin, 2);
        else
        {
            var buffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.ENEMYDEADCOINUP);

            var rewardvalue = 1 + (int)buffvalue;

            GameRoot.Instance.UserData.CurMode.EnergyMoney.Value += rewardvalue;
        }


        GameRoot.Instance.EffectSystem.MultiPlay<DeathEffect>(this.transform.position, effect =>
        {
                ProjectUtility.SetActiveCheck(effect.gameObject, true);
                effect.SetAutoRemove(true, 3f);
        });

        if (GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value == true && UnitIdx > 1000)
        {
            GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value = false;
        }


        if(UnitIdx > 1000)
        {
            GetGachaCoin();
        }


        if(IsDeathMoneyReward())
        {
            GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value += 1;
        }

        ProjectUtility.SetActiveCheck(this.gameObject, false);
    }



    public void GetGachaCoin()
    {
        var waveidx = GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value;

        var increase = waveidx < 10 ? 1 : (waveidx / 10) * 2;

        var rewardvalue = GameRoot.Instance.InGameBattleSystem.boss_gacha_coin_base * increase;

        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin,
            rewardvalue);

        //GameRoot.Instance.EffectSystem.MultiPlay<IconTextEffect>(HpTr.position, effect =>
        //{
        //    effect.SetText(rewardvalue.ToString(), (int)Config.CurrencyID.GachaCoin);
        //    effect.SetAutoRemove(true, 1f);
        //    effect.transform.SetParent(HpTr.transform);
        //});
    }

    public void DeadAnimEnd()
    {
        ProjectUtility.SetActiveCheck(this.gameObject, false);
    }

    public bool IsDeathMoneyReward()
    {
        var randvalue = Random.Range(0, 100);


        return randvalue <= GameRoot.Instance.InGameBattleSystem.unit_death_ratio;


    }


    void Update()
    {
        debuffdeltatime += Time.deltaTime;

        if(debuffdeltatime >= 1f)
        {
            debuffdeltatime = 0f;

            foreach(var debuff in DebuffList)
            {
                debuff.Time -= 1;
            }


            DebuffList.RemoveAll(debuff => debuff.Time <= 0);

            SetInfo();

        }
        if(UnitIdx == GameRoot.Instance.InGameSystem.TicketEnemyIdx && CurState != State.Dead)
        {
            if(TimeUI != null)
            {
                ticketdeltime += Time.deltaTime;


                var time = GameRoot.Instance.InGameBattleSystem.ticket_unit_time - ticketdeltime;

                TimeUI.SetTime((int)time);

                if(time <= 0)
                {
                    //end
                    Dead();
                }
            }
        }

        if (Target != null && DebuffList.Find(x=> x.Type == DebuffType.Sturn) == null)
        {
            // 현재 위치에서 목표 위치까지의 방향 벡터 계산
            Vector3 direction = Target.SpawnTr.position - transform.position;
            direction.Normalize();

            // 유닛을 목표 방향으로 이동
            transform.position += direction * MoveSpeed * Time.deltaTime;

            float distance = Vector3.Distance(transform.position, Target.SpawnTr.position);

            if(distance < 0.1f)
            {
                MoveSpawnCount += 1;
                if (MoveSpawnCount >= MoveEndSpawnCount)
                {
                    MoveSpawnCount = 0;
                    Target = Battle.GetSpawnTr(MoveSpawnCount);
                    SetDirection(Target.SpawnDirection);
                }
                Target = Battle.GetSpawnTr(MoveSpawnCount);
                SetDirection(Target.SpawnDirection);
            }
        }
    }
}
