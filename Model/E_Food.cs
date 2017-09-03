using System;
namespace Model
{
	/// <summary>
	/// 菜品字典
	/// </summary>
	[Serializable]
	public partial class E_Food:BaseModel
	{
	 
		#region Model
		private int _foodid;
		private int? _areaid;
		private int? _classinfoid;
		private string _foodname;
	 
		private string _pic;
		private int? _praise;
		private int? _bad;
		private int? _isdisplay;
		private DateTime? _updatetime=DateTime.Now;
		/// <summary>
		/// 菜品id
		/// </summary>
		public int foodid
		{
			set{ _foodid=value;}
			get{return _foodid;}
		}
		/// <summary>
		/// 作业区id
		/// </summary>
		public int? areaid
		{
			set{ _areaid=value;}
			get{return _areaid;}
		}
		/// <summary>
		/// 班组id
		/// </summary>
		public int? classinfoid
		{
			set{ _classinfoid=value;}
			get{return _classinfoid;}
		}
		/// <summary>
		/// 菜名
		/// </summary>
		public string foodname
		{
			set{ _foodname=value;}
			get{return _foodname;}
		}
		/// <summary>
		/// 厨师名称
		/// </summary>
		public string pname
		{
            get;set;
		}
		/// <summary>
		/// 图片名称
		/// </summary>
		public string pic
		{
			set{ _pic=value;}
			get{return _pic;}
		}
		/// <summary>
		/// 赞
		/// </summary>
		public int? praise
		{
			set{ _praise=value;}
			get{return _praise;}
		}
		/// <summary>
		/// 不好
		/// </summary>
		public int? bad
		{
			set{ _bad=value;}
			get{return _bad;}
		}
		/// <summary>
		/// 是否显示
		/// </summary>
		public int? isdisplay
		{
			set{ _isdisplay=value;}
			get{return _isdisplay;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? updatetime
		{
			set{ _updatetime=value;}
			get{return _updatetime;}
		}
		#endregion Model
        public DateTime? addtime { get; set; }
        public DateTime? startaddtime { get; set; }
        public int? pid { get; set; }
        public int? dishid { get; set; }

    }
}

