using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class WaveClearComponent : MonoBehaviour
{
    [SerializeField]
    private Text WaveStartText;

    [SerializeField]
    private GameObject WaveClearObj;

    [SerializeField]
    private GameObject BossStartObj;

    [SerializeField]
    private Text WaveText;

    [SerializeField]
    private Text BossNameText;

    [SerializeField]
    private Text TimeText;

    [SerializeField]
    private Image BossImg;

    public void Set(int waveidx)
    {
        var td = Tables.Instance.GetTable<StageWaveInfo>().GetData(waveidx);

        if(td != null)
        {
            ProjectUtility.SetActiveCheck(WaveClearObj, td.unit_idx < 1000);
            ProjectUtility.SetActiveCheck(BossStartObj, td.unit_idx > 1000);

             
            if(BossStartObj.activeSelf)
            {
                GameRoot.Instance.WaitTimeAndCallback(1f, ()=> {
                    ProjectUtility.SetActiveCheck(BossStartObj, false);
                });
            }
        }

        WaveStartText.text = $"WAVE {waveidx}";
        WaveText.text = $"WAVE {waveidx}";

        var enemytd = Tables.Instance.GetTable<EnemyInfo>().GetData(td.unit_idx);

        if(enemytd != null)
        {
            BossImg.sprite = Config.Instance.GetUnitImg(enemytd.image);
            BossNameText.text = Tables.Instance.GetTable<Localize>().GetString(enemytd.name);
            TimeText.text = GameRoot.Instance.InGameBattleSystem.boss_unit_time.ToString();
        }
    }

}
