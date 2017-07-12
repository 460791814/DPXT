using System;
namespace Model
{
	/// <summary>
	/// 点评项
	/// </summary>
	[Serializable]
	public partial class E_CommentItem
	{
	 
		#region Model
		private int _commentitemid;
		private int? _commenttypeid;
		private string _title;
		private DateTime? _updatetime;
		private int? _isdelete;
		/// <summary>
		/// 点评项id
		/// </summary>
		public int commentitemid
		{
			set{ _commentitemid=value;}
			get{return _commentitemid;}
		}
		/// <summary>
		/// 点评维度ID
		/// </summary>
		public int? commenttypeid
		{
			set{ _commenttypeid=value;}
			get{return _commenttypeid;}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string title
		{
			set{ _title=value;}
			get{return _title;}
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

