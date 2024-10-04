using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UniRx;

public class InGameBattleSystem
{

    public int end_unit_count = 0;

    public int unit_max_count = 0;

    public int initial_cost_start = 0;

    public int inital_increase_count = 0;

    public int start_battle_energymoney = 0;

    public int ticket_monster_cooltime = 0;

    public int ticket_unit_time = 0;

    public int boss_unit_time = 0;

    public int enemy_normal_base_hp = 0;

    public double enemy_wave_increase_hp = 0;

    public int boss_gacha_coin_base = 0;

    public int boss_gacha_coin_increaes = 0;

    public int unit_death_ratio = 0;

    public IReactiveProperty<int> CurUnitCountProperty = new ReactiveProperty<int>();

    public void Create()
    {
        unit_max_count = Tables.Instance.GetTable<Define>().GetData("unit_max_count").value;
        end_unit_count = Tables.Instance.GetTable<Define>().GetData("end_unit_count").value;
        initial_cost_start = Tables.Instance.GetTable<Define>().GetData("initial_cost_start").value;
        inital_increase_count = Tables.Instance.GetTable<Define>().GetData("inital_increase_count").value;
        start_battle_energymoney = Tables.Instance.GetTable<Define>().GetData("start_battle_energymoney").value;
        ticket_monster_cooltime = Tables.Instance.GetTable<Define>().GetData("ticket_monster_cooltime").value;
        ticket_unit_time = Tables.Instance.GetTable<Define>().GetData("ticket_unit_time").value;
        boss_unit_time = Tables.Instance.GetTable<Define>().GetData("boss_unit_time").value;
        enemy_normal_base_hp = Tables.Instance.GetTable<Define>().GetData("enemy_normal_base_hp").value;
        enemy_wave_increase_hp = Tables.Instance.GetTable<Define>().GetData("enemy_wave_increase_hp").value;
        boss_gacha_coin_base = Tables.Instance.GetTable<Define>().GetData("boss_gacha_coin_base").value;
        boss_gacha_coin_increaes = Tables.Instance.GetTable<Define>().GetData("boss_gacha_coin_increaes").value;
        unit_death_ratio = Tables.Instance.GetTable<Define>().GetData("unit_death_ratio").value;
    }   




}
