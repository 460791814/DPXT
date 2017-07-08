using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 系统管理员表
    /// </summary>
    public class E_User
    {
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userid { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string realname { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 作业区ID
        /// </summary>
        public int areaid { get; set; }

        /// <summary>
        /// 作业区名称
        /// </summary>
        public string areaname { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        public int classinfoid { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        public string classinfoname { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updatetime { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageindex{get;set;}

        /// <summary>
        /// 每页显示数据条数
        /// </summary>
        public int pagesize { get; set; }
        
    }
}
