using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class SkillCardInfoData
    {
        [SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private int _skill_base_value;
		public int skill_base_value
		{
			get { return _skill_base_value;}
			set { _skill_base_value = value;}
		}
		[SerializeField]
		private int _skill_value;
		public int skill_value
		{
			get { return _skill_value;}
			set { _skill_value = value;}
		}
		[SerializeField]
		private string _sign_desc;
		public string sign_desc
		{
			get { return _sign_desc;}
			set { _sign_desc = value;}
		}
		[SerializeField]
		private string _image;
		public string image
		{
			get { return _image;}
			set { _image = value;}
		}
		[SerializeField]
		private string _desc_name;
		public string desc_name
		{
			get { return _desc_name;}
			set { _desc_name = value;}
		}

    }

    [System.Serializable]
    public class SkillCardInfo : Table<SkillCardInfoData, int>
    {
    }
}

