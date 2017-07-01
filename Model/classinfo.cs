using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 班组
    /// </summary>
    public class classinfo
    {
        /// <summary>
        /// 班组ID
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 作业区ID
        /// </summary>
        public int areaid { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        public string cname { get; set; }
    }
}
