using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageInfoData
    {
        [SerializeField]
		private int _stage_idx;
		public int stage_idx
		{
			get { return _stage_idx;}
			set { _stage_idx = value;}
		}
		[SerializeField]
		private string _stage_prefab;
		public string stage_prefab
		{
			get { return _stage_prefab;}
			set { _stage_prefab = value;}
		}
		[SerializeField]
		private int _start_hp;
		public int start_hp
		{
			get { return _start_hp;}
			set { _start_hp = value;}
		}
		[SerializeField]
		private string _planet_img;
		public string planet_img
		{
			get { return _planet_img;}
			set { _planet_img = value;}
		}

    }

    [System.Serializable]
    public class StageInfo : Table<StageInfoData, int>
    {
    }
}

