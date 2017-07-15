using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class E_Person:BaseModel
    {
        #region Model
        private int? _personid;
        private int? _areaid;
        private int? _classinfoid;
        private string _personname;
        private int? _jobtypeid;
        private int? _perfect;
        private int? _good;
        private int? _bad;
        private int? _isdelete = 0;
        /// <summary>
        /// 
        /// </summary>
        public int? personid
        {
            set { _personid = value; }
            get { return _personid; }
        }
        /// <summary>
        /// 作业区id
        /// </summary>
        public int? areaid
        {
            set { _areaid = value; }
            get { return _areaid; }
        }
        /// <summary>
        /// 班组id
        /// </summary>
        public int? classinfoid
        {
            set { _classinfoid = value; }
            get { return _classinfoid; }
        }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string personname
        {
            set { _personname = value; }
            get { return _personname; }
        }
        /// <summary>
        /// 工作岗位
        /// </summary>
        public int? jobtypeid
        {
            set { _jobtypeid = value; }
            get { return _jobtypeid; }
        }
        /// <summary>
        /// 优秀
        /// </summary>
        public int? perfect
        {
            set { _perfect = value; }
            get { return _perfect; }
        }
        /// <summary>
        /// 一般
        /// </summary>
        public int? good
        {
            set { _good = value; }
            get { return _good; }
        }
        /// <summary>
        /// 差的
        /// </summary>
        public int? bad
        {
            set { _bad = value; }
            get { return _bad; }
        }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? isdelete
        {
            set { _isdelete = value; }
            get { return _isdelete; }
        }
        #endregion Model
    }
}
