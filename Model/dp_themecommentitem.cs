using System;
namespace Maticsoft.Model
{
	/// <summary>
	/// 点评主题
	/// </summary>
	[Serializable]
	public partial class dp_themecommentitem
	{
		public dp_themecommentitem()
		{}
		#region Model
		private int _themeid;
		private int _commentitemid;
		/// <summary>
		/// 点评主题id
		/// </summary>
		public int themeid
		{
			set{ _themeid=value;}
			get{return _themeid;}
		}
		/// <summary>
		/// 意见id
		/// </summary>
		public int commentitemid
		{
			set{ _commentitemid=value;}
			get{return _commentitemid;}
		}
		#endregion Model

	}
}

