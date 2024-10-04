using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;




public class WeaponGachaUpgradeSystem
{

   public float GetValue(int weaponidx, int level)
    {
        float value = 0f;

        var weapondata = GameRoot.Instance.UserData.CurMode.PlayerWeapon.WeaponList.ToList().Find(x => x.WeaponIdx == weaponidx);

        if(weapondata != null)
        {
            var td = Tables.Instance.GetTable<WeaponGachaUpgrade>().GetData(new KeyValuePair<int, int>(weaponidx, (int)level));

            if(td != null)
            {
                value = (float)td.upgrade_value / 100f;
            }
        }

        return value;
    }



}
