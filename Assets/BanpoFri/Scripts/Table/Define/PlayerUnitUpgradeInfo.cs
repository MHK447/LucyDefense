using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class PlayerUnitUpgradeInfoData
    {
        [SerializeField]
		private int _upgrade_idx;
		public int upgrade_idx
		{
			get { return _upgrade_idx;}
			set { _upgrade_idx = value;}
		}
		[SerializeField]
		private int _upgrade_type;
		public int upgrade_type
		{
			get { return _upgrade_type;}
			set { _upgrade_type = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}
		[SerializeField]
		private string _desc_type;
		public string desc_type
		{
			get { return _desc_type;}
			set { _desc_type = value;}
		}
		[SerializeField]
		private int _base_value;
		public int base_value
		{
			get { return _base_value;}
			set { _base_value = value;}
		}
		[SerializeField]
		private int _value_increase;
		public int value_increase
		{
			get { return _value_increase;}
			set { _value_increase = value;}
		}
		[SerializeField]
		private int _base_cost;
		public int base_cost
		{
			get { return _base_cost;}
			set { _base_cost = value;}
		}
		[SerializeField]
		private int _cost_increase_1;
		public int cost_increase_1
		{
			get { return _cost_increase_1;}
			set { _cost_increase_1 = value;}
		}
		[SerializeField]
		private int _cost_increase_2;
		public int cost_increase_2
		{
			get { return _cost_increase_2;}
			set { _cost_increase_2 = value;}
		}
		[SerializeField]
		private int _order;
		public int order
		{
			get { return _order;}
			set { _order = value;}
		}

    }

    [System.Serializable]
    public class PlayerUnitUpgradeInfo : Table<PlayerUnitUpgradeInfoData, int>
    {
    }
}

