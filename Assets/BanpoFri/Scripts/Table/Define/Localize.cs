using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class LocalizeData
    {
        [SerializeField]
		private string _idx;
		public string idx
		{
			get { return _idx;}
			set { _idx = value;}
		}
		[SerializeField]
		private string _ko;
		public string ko
		{
			get { return _ko;}
			set { _ko = value;}
		}
		[SerializeField]
		private string _en;
		public string en
		{
			get { return _en;}
			set { _en = value;}
		}
		[SerializeField]
		private string _ja;
		public string ja
		{
			get { return _ja;}
			set { _ja = value;}
		}
		[SerializeField]
		private string _es;
		public string es
		{
			get { return _es;}
			set { _es = value;}
		}
		[SerializeField]
		private string _ptbr;
		public string ptbr
		{
			get { return _ptbr;}
			set { _ptbr = value;}
		}
		[SerializeField]
		private string _th;
		public string th
		{
			get { return _th;}
			set { _th = value;}
		}
		[SerializeField]
		private string _tw;
		public string tw
		{
			get { return _tw;}
			set { _tw = value;}
		}
		[SerializeField]
		private string _vi;
		public string vi
		{
			get { return _vi;}
			set { _vi = value;}
		}
		[SerializeField]
		private string _bg;
		public string bg
		{
			get { return _bg;}
			set { _bg = value;}
		}
		[SerializeField]
		private string _cn;
		public string cn
		{
			get { return _cn;}
			set { _cn = value;}
		}
		[SerializeField]
		private string _cs;
		public string cs
		{
			get { return _cs;}
			set { _cs = value;}
		}
		[SerializeField]
		private string _da;
		public string da
		{
			get { return _da;}
			set { _da = value;}
		}
		[SerializeField]
		private string _nl;
		public string nl
		{
			get { return _nl;}
			set { _nl = value;}
		}
		[SerializeField]
		private string _et;
		public string et
		{
			get { return _et;}
			set { _et = value;}
		}
		[SerializeField]
		private string _fi;
		public string fi
		{
			get { return _fi;}
			set { _fi = value;}
		}
		[SerializeField]
		private string _fr;
		public string fr
		{
			get { return _fr;}
			set { _fr = value;}
		}
		[SerializeField]
		private string _de;
		public string de
		{
			get { return _de;}
			set { _de = value;}
		}
		[SerializeField]
		private string _el;
		public string el
		{
			get { return _el;}
			set { _el = value;}
		}
		[SerializeField]
		private string _hu;
		public string hu
		{
			get { return _hu;}
			set { _hu = value;}
		}
		[SerializeField]
		private string _id;
		public string id
		{
			get { return _id;}
			set { _id = value;}
		}
		[SerializeField]
		private string _it;
		public string it
		{
			get { return _it;}
			set { _it = value;}
		}
		[SerializeField]
		private string _lv;
		public string lv
		{
			get { return _lv;}
			set { _lv = value;}
		}
		[SerializeField]
		private string _lt;
		public string lt
		{
			get { return _lt;}
			set { _lt = value;}
		}
		[SerializeField]
		private string _no;
		public string no
		{
			get { return _no;}
			set { _no = value;}
		}
		[SerializeField]
		private string _pl;
		public string pl
		{
			get { return _pl;}
			set { _pl = value;}
		}
		[SerializeField]
		private string _pt;
		public string pt
		{
			get { return _pt;}
			set { _pt = value;}
		}
		[SerializeField]
		private string _ro;
		public string ro
		{
			get { return _ro;}
			set { _ro = value;}
		}
		[SerializeField]
		private string _ru;
		public string ru
		{
			get { return _ru;}
			set { _ru = value;}
		}
		[SerializeField]
		private string _sk;
		public string sk
		{
			get { return _sk;}
			set { _sk = value;}
		}
		[SerializeField]
		private string _sl;
		public string sl
		{
			get { return _sl;}
			set { _sl = value;}
		}
		[SerializeField]
		private string _sv;
		public string sv
		{
			get { return _sv;}
			set { _sv = value;}
		}
		[SerializeField]
		private string _tr;
		public string tr
		{
			get { return _tr;}
			set { _tr = value;}
		}
		[SerializeField]
		private string _ua;
		public string ua
		{
			get { return _ua;}
			set { _ua = value;}
		}

    }

    [System.Serializable]
    public class Localize : Table<LocalizeData, string>
    {
    }
}

