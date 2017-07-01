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
    public class user
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userid { get; set; }

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
        /// 班组ID
        /// </summary>
        public int classinfoid { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updatetime { get; set; }
    }
}
