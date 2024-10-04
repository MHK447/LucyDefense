using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UpgradeInfoData
    {
        [SerializeField]
		private int _weapon_idx;
		public int weapon_idx
		{
			get { return _weapon_idx;}
			set { _weapon_idx = value;}
		}
		[SerializeField]
		private int _upgrade_type;
		public int upgrade_type
		{
			get { return _upgrade_type;}
			set { _upgrade_type = value;}
		}
		[SerializeField]
		private int _base_cost;
		public int base_cost
		{
			get { return _base_cost;}
			set { _base_cost = value;}
		}
		[SerializeField]
		private int _base_value;
		public int base_value
		{
			get { return _base_value;}
			set { _base_value = value;}
		}
		[SerializeField]
		private int _increase_cost;
		public int increase_cost
		{
			get { return _increase_cost;}
			set { _increase_cost = value;}
		}
		[SerializeField]
		private int _increase_value;
		public int increase_value
		{
			get { return _increase_value;}
			set { _increase_value = value;}
		}

    }

    [System.Serializable]
    public class UpgradeInfo : Table<UpgradeInfoData, KeyValuePair<int,int>>
    {
    }
}

