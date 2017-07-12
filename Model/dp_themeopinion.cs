using System;
namespace Maticsoft.Model
{
	/// <summary>
	/// 点评主题跟意见关系表
	/// </summary>
	[Serializable]
	public partial class dp_themeopinion
	{
		public dp_themeopinion()
		{}
		#region Model
		private int _themeid;
		private int _opinionid;
		/// <summary>
		/// 点评主题id
		/// </summary>
		public int themeid
		{
			set{ _themeid=value;}
			get{return _themeid;}
		}
		/// <summary>
		/// 点评项id
		/// </summary>
		public int opinionid
		{
			set{ _opinionid=value;}
			get{return _opinionid;}
		}
		#endregion Model

	}
}

