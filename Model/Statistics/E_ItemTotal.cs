using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Statistics
{
    /// <summary>
    /// 点评项总和统计
    /// </summary>
    public class E_ItemTotal
    {
        /// <summary>
        /// 点评维度
        /// </summary>
        public string typename { get; set; }

        /// <summary>
        /// 点评项标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 期望值总和 期望值（E）
        /// </summary>
        public float sumexpect { get; set; }

        /// <summary>
        /// 感知值总和 感知值（P）
        /// </summary>
        public float sumreal { get; set; }

        /// <summary>
        /// 总和差值 差值（P-E）
        /// </summary>
        public float sumdifferent { get; set; }
    }
}
