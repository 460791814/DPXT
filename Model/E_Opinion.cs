using System;
namespace Model
{
	/// <summary>
	/// 意见表
	/// </summary>
	[Serializable]
	public partial class E_Opinion : BaseModel
    {
	
		#region Model
		private int _opinionid;
		private int? _opiniontypeid;
		private string _opinioncontent;
		private DateTime? _updatetime;
		private int? _isdelete;
		/// <summary>
		/// 意见id
		/// </summary>
		public int opinionid
		{
			set{ _opinionid=value;}
			get{return _opinionid;}
		}
		/// <summary>
		/// 意见类型id
		/// </summary>
		public int? opiniontypeid
		{
			set{ _opiniontypeid=value;}
			get{return _opiniontypeid;}
		}
		/// <summary>
		/// 意见内容
		/// </summary>
		public string opinioncontent
		{
			set{ _opinioncontent=value;}
			get{return _opinioncontent;}
		}
		/// <summary>
		/// 更新时间
		/// </summary>
		public DateTime? updatetime
		{
			set{ _updatetime=value;}
			get{return _updatetime;}
		}
		/// <summary>
		/// 是否删除
		/// </summary>
		public int? isdelete
		{
			set{ _isdelete=value;}
			get{return _isdelete;}
		}
		#endregion Model

	}
}

