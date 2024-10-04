using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class PlayerUnitInfoData
    {
        [SerializeField]
		private int _unit_idx;
		public int unit_idx
		{
			get { return _unit_idx;}
			set { _unit_idx = value;}
		}
		[SerializeField]
		private int _attack;
		public int attack
		{
			get { return _attack;}
			set { _attack = value;}
		}
		[SerializeField]
		private int _attackrange;
		public int attackrange
		{
			get { return _attackrange;}
			set { _attackrange = value;}
		}
		[SerializeField]
		private int _criticalchance;
		public int criticalchance
		{
			get { return _criticalchance;}
			set { _criticalchance = value;}
		}
		[SerializeField]
		private int _attackspeed;
		public int attackspeed
		{
			get { return _attackspeed;}
			set { _attackspeed = value;}
		}
		[SerializeField]
		private int _grade;
		public int grade
		{
			get { return _grade;}
			set { _grade = value;}
		}
		[SerializeField]
		private string _prefab;
		public string prefab
		{
			get { return _prefab;}
			set { _prefab = value;}
		}
		[SerializeField]
		private string _icon;
		public string icon
		{
			get { return _icon;}
			set { _icon = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}
		[SerializeField]
		private List<int> _unit_skill;
		public List<int> unit_skill
		{
			get { return _unit_skill;}
			set { _unit_skill = value;}
		}
		[SerializeField]
		private List<int> _unit_skil_percent;
		public List<int> unit_skil_percent
		{
			get { return _unit_skil_percent;}
			set { _unit_skil_percent = value;}
		}
		[SerializeField]
		private List<int> _unit_skil_value;
		public List<int> unit_skil_value
		{
			get { return _unit_skil_value;}
			set { _unit_skil_value = value;}
		}

    }

    [System.Serializable]
    public class PlayerUnitInfo : Table<PlayerUnitInfoData, int>
    {
    }
}

