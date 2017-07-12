using System;
namespace Model
{
	/// <summary>
	/// 点评主题
	/// </summary>
	[Serializable]
	public partial class E_Theme
	{
		public E_Theme()
		{}
		#region Model
		private int _themeid;
		private string _themename;

		/// <summary>
		/// 点评主题ID
		/// </summary>
		public int themeid
		{
			set{ _themeid=value;}
			get{return _themeid;}
		}
		/// <summary>
		/// 主题名称
		/// </summary>
		public string themename
		{
			set{ _themename=value;}
			get{return _themename;}
		}

		#endregion Model

	}
}

