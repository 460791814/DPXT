using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Statistics
{
    /// <summary>
    /// 员工统计 报表
    /// </summary>
    public class E_PersonReport
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public int personid { get; set; }

        /// <summary>
        /// 作业区ID
        /// </summary>
        public int areaid { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        public int classinfoid { get; set; }

        /// <summary>
        /// 员工名册
        /// </summary>
        public string personname { get; set; }

        /// <summary>
        /// 工作岗位
        /// </summary>
        public int jobtypeid { get; set; }

        /// <summary>
        /// 优秀
        /// </summary>
        public int perfect { get; set; }

        /// <summary>
        /// 一般
        /// </summary>
        public int good { get; set; }

        /// <summary>
        /// 差的
        /// </summary>
        public int bad { get; set; }

        /// <summary>
        /// 好评率
        /// </summary>
        public float perfectrate { get; set; }

        /// <summary>
        /// 差评率
        /// </summary>
        public float badrate { get; set; }
    }
}
