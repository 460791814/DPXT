using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Statistics
{
    /// <summary>
    /// 统计查询参数实体
    /// </summary>
    public class E_StatisticsParameter
    {
        /// <summary>
        /// 是否查询
        /// </summary>
        public int issearch { get; set; }

        /// <summary>
        /// 作业区ID
        /// </summary>
        public int areaid { get; set; }
        
        /// <summary>
        /// 班组ID
        /// </summary>
        public int classinfoid { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? starttime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endtime { get; set; }
    }
}
