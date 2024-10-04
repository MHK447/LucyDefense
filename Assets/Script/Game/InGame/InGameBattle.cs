using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.AddressableAssets;
using System.Linq;

public class InGameBattle : MonoBehaviour
{
    [System.Serializable]
    public class SpawnTrPos
    {
        public Transform SpawnTr;
        public Direction SpawnDirection;
    }

    public enum Direction
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
    }

    [SerializeField]
    private List<SpawnTrPos> SpawnList = new List<SpawnTrPos>();

    [SerializeField]
    private List<UnitTileComponent> TileComponentList = new List<UnitTileComponent>();

    private List<UnitTileComponent> TileComponentOrderList = new List<UnitTileComponent>();

    private List<InGameEnemyBase> EnemyList = new List<InGameEnemyBase>();

    public List<InGameEnemyBase> GetEnemyList { get { return EnemyList; } }

    private List<InGameUnitBase> PlayerUnitList = new List<InGameUnitBase>();

    private List<InGameDamageUI> DamageUIList = new List<InGameDamageUI>();

    private int WaveEnemyCount = 0;

    private float UnitSpawnDelayTime = 0f;

    private float WaveCoolTime = 0f;

    private int UnitIdx = 0;

    private float Spawndeltime = 0f;

    private bool IsEndWave = true;

    private float waveonesecondtime = 0f;

    private float wavedeltime = 0f;

    public void Init()
    {

        TileComponentOrderList = TileComponentList.OrderBy(x => x.GetTileSpawnOrder).ToList();
    }
        
    public void StartBattle(int waveidx)
    {
        IsEndWave = false;

        GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value = waveidx;

        var td = Tables.Instance.GetTable<StageWaveInfo>().GetData(waveidx);

        if(td != null)
        {
            WaveEnemyCount = td.wave_count;

            UnitSpawnDelayTime = (float)td.unit_delaytime / 100f;

            WaveCoolTime = td.spawn_cooltime;

            UnitIdx = td.unit_idx;


            var coinbuffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.WAVEADDCOIN, false);

            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, (int)coinbuffvalue);

            var highscorewave = GameRoot.Instance.UserData.CurMode.StageData.StageHighWave;

            if(waveidx > highscorewave)
            {
                GameRoot.Instance.UserData.CurMode.StageData.StageHighWave = waveidx;
            }
        }
    }

    public SpawnTrPos GetSpawnTr(int spawncount)
    {
        return SpawnList[spawncount];
    }

    public InGameUnitBase FindPlayerUnit(int unitidx)
    {
        return PlayerUnitList.Find(x => x.GetUnitIdx == unitidx && x.gameObject.activeSelf == true);
    }

    public void InitClear()
    {
        IsEndWave = true;
        GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle = false;
        WaveEnemyCount = 0;
        WaveCoolTime = 0f;
        waveonesecondtime = 0f;
        GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.Clear();

        foreach (var player in PlayerUnitList)
        {
            ProjectUtility.SetActiveCheck(player.gameObject, false);
        }

        foreach(var enemy in EnemyList)
        {
            enemy.Clear();
            ProjectUtility.SetActiveCheck(enemy.gameObject, false);
        }

        foreach(var damageui in DamageUIList)
        {
            ProjectUtility.SetActiveCheck(damageui.gameObject, false);
        }
    }

    public void SpawnEnemy(int unitidx)
    {
        var td = Tables.Instance.GetTable<EnemyInfo>().GetData(unitidx);

        if (unitidx != GameRoot.Instance.InGameSystem.TicketEnemyIdx)
        {
            GameRoot.Instance.UserData.CurMode.StageData.UnitCountPropety.Value += 1;
        }

        if (td != null)
        {
            var finddata = EnemyList.Find(x => x.IsDeath == true && x.GetUnitIdx == unitidx);

            if(finddata != null)
            {
                ProjectUtility.SetActiveCheck(finddata.gameObject, true);
                finddata.transform.position = SpawnList[0].SpawnTr.position;
                finddata.Set(unitidx,this);
            }
            else
            {
                Addressables.InstantiateAsync(td.prefab).Completed += (handle) =>
                {
                    var getobj = handle.Result.gameObject.GetComponent<InGameEnemyBase>();

                    if(getobj != null)
                    {
                        EnemyList.Add(getobj);
                        ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                        getobj.transform.position = SpawnList[0].SpawnTr.position;
                        getobj.Set(unitidx,this);
                    }
                };
            }
        }
    }

    public void NextWave()
    {
        GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value += 1;
        var td = Tables.Instance.GetTable<StageWaveInfo>().GetData(GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value);

        if(td != null)
        {
            Spawndeltime = 0f;

            wavedeltime = 0;

            WaveEnemyCount = td.wave_count;

            UnitSpawnDelayTime = (float)td.unit_delaytime / 100f;

            WaveCoolTime = td.spawn_cooltime;

            UnitIdx = td.unit_idx;
        }
        else
        {
            IsEndWave = true;
        }

        var highscorewave = GameRoot.Instance.UserData.CurMode.StageData.StageHighWave;

        if (GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value > highscorewave)
        {   
            GameRoot.Instance.UserData.CurMode.StageData.StageHighWave = GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value;
        }   
    }


    public void Update()
    {
        if (IsEndWave) return;
        if (!GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle) return;
            
        if(WaveEnemyCount > 0)
        {
            Spawndeltime += Time.deltaTime;

            if(Spawndeltime >= UnitSpawnDelayTime)
            {
                WaveEnemyCount -= 1;
                Spawndeltime = 0f;
                SpawnEnemy(UnitIdx);
            }
        }

        if (waveonesecondtime >= 1f) // one seconds updates;
        {
            wavedeltime += 1;
            var wavetime = WaveCoolTime - wavedeltime;
            GameRoot.Instance.UserData.CurMode.StageData.WaveTimeProperty.Value = (int)wavetime;
            if (wavedeltime >= WaveCoolTime)
            {
                wavedeltime = 0;
                NextWave();
            }
            waveonesecondtime -= 1f;
        }
        waveonesecondtime += Time.deltaTime;
    }



    public void SetDamageUI(Transform damageuitr , int damage)
    {
        var finddamageui = DamageUIList.Find(x => x.gameObject.activeSelf == false);

        if(finddamageui != null)
        {
            ProjectUtility.SetActiveCheck(finddamageui.gameObject, true);
            finddamageui.Init(damageuitr);
            finddamageui.SetDamage(damage);
        }
        else
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<InGameDamageUI>((damageui) => {
                ProjectUtility.SetActiveCheck(damageui.gameObject, true);
                damageui.Init(damageuitr);
                damageui.SetDamage(damage);
                DamageUIList.Add(damageui);
            });
        }
    }


    public void RandGachaUnit()
    {
        var level = GameRoot.Instance.UnitUpgradeSystem.FindUnitUpgradeData(UpgradeComponentType.SpawnPercent).Level;

        var grade = ProjectUtility.GetRandGachaCard(level);

        GachaGradeUnit(grade);
    }


    public void MergeAddUnit(int unitidx , UnitTileComponent unittile)
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if (td != null)
        {
            var findunit = PlayerUnitList.Find(x => x.gameObject.activeSelf == false && x.GetUnitIdx == unitidx);

            if (findunit != null)
            {
                ProjectUtility.SetActiveCheck(findunit.gameObject, true);
                findunit.Set(unitidx, this);

                var finddata = TileComponentList.Find(x => x.GetTileUnitIdx == unitidx && x.IsTileMax == false);

                if (finddata != null)
                {
                    finddata.SpawnTileUnit(findunit);
                }
                else
                    unittile.SpawnTileUnit(findunit);

                CurUnitCountCheck();
            }
            else
            {
                Addressables.InstantiateAsync(td.prefab).Completed += (handle) =>
                {
                    var getobj = handle.Result.gameObject.GetComponent<InGameUnitBase>();

                    if (getobj != null)
                    {
                        PlayerUnitList.Add(getobj);

                        ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                        getobj.Set(unitidx, this);

                        var finddata = TileComponentList.Find(x => x.GetTileUnitIdx == unitidx && x.IsTileMax == false);

                        if (finddata != null)
                        {
                            finddata.SpawnTileUnit(getobj);
                        }
                        else
                            unittile.SpawnTileUnit(getobj);
                    }
                    CurUnitCountCheck();
                };
            }
        }
    }

    public void CurUnitCountCheck()
    {
        GameRoot.Instance.InGameBattleSystem.CurUnitCountProperty.Value = PlayerUnitList.FindAll(x => x.gameObject.activeSelf).Count;
    }

    public void AddUnit(int unitidx)
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if (td != null)
        {
            var findunit = PlayerUnitList.Find(x => x.gameObject.activeSelf == false && x.GetUnitIdx == unitidx);

            if (findunit != null)
            {
                ProjectUtility.SetActiveCheck(findunit.gameObject, true);
                findunit.Set(unitidx, this);

                var finddata = TileComponentList.Find(x => x.GetTileUnitIdx == unitidx && x.IsTileMax == false);

                if (finddata != null)
                {
                    finddata.SpawnTileUnit(findunit);
                }
                else
                {
                    var findemptytile = TileComponentOrderList.Find(x => x.GetTileUnitIdx == -1);

                    if (findemptytile != null)
                    {
                        findemptytile.SpawnTileUnit(findunit);
                    }
                }

                CurUnitCountCheck();
            }
            else
            {
                Addressables.InstantiateAsync(td.prefab).Completed += (handle) =>
                {
                    var getobj = handle.Result.gameObject.GetComponent<InGameUnitBase>();

                    if (getobj != null)
                    {
                        PlayerUnitList.Add(getobj);

                        ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                        getobj.Set(unitidx, this);

                        var finddata = TileComponentList.Find(x => x.GetTileUnitIdx == unitidx && x.IsTileMax == false);

                        if (finddata != null)
                        {
                            finddata.SpawnTileUnit(getobj);
                        }
                        else
                        {
                            var findemptytile = TileComponentOrderList.Find(x => x.GetTileUnitIdx == -1);

                            if (findemptytile != null)
                            {
                                findemptytile.SpawnTileUnit(getobj);
                            }
                        }
                        CurUnitCountCheck();
                    }
                };
            }
        }
    }



    public InGameEnemyBase FindClosestEnemy(float attackrange , Transform unittr)
    {
        if (unittr == null) return null;

        InGameEnemyBase closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (InGameEnemyBase enemy in EnemyList)
        {
            if (enemy.IsDeath) continue;
            if (enemy.gameObject.activeSelf == false) continue;

            float distance = Vector3.Distance(unittr.position, enemy.transform.position);
            if (distance < closestDistance && distance <= attackrange)
            {
                closestDistance = distance;
                closestEnemy = enemy;
              
            }
        }

        return closestEnemy;
    }   


    public void GachaGradeUnit(int grade = 0)
    {
        var tdlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.Where(x => x.grade == grade).ToList();

        if (tdlist.Count == 0) return;

        var randunit = Random.Range(0, tdlist.Count);

        AddUnit(tdlist[randunit].unit_idx);
    }

}
