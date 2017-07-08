using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Statistics
{
    /// <summary>
    /// 项目总和平均统计
    /// </summary>
    public class E_ItemTotalAvg
    {
        /// <summary>
        /// 点评维度
        /// </summary>
        public string typename { get; set; }

        /// <summary>
        /// 期望均值(E)
        /// </summary>
        public float avgexpect { get; set; }

        /// <summary>
        /// 感知均值(P)
        /// </summary>
        public float avgreal { get; set; }

        /// <summary>
        /// 差值（P-E）均值
        /// </summary>
        public float avgdifferent { get; set; }

        /// <summary>
        /// 期望偏离度（（P-E）/E*100%）
        /// </summary>
        public float expectdeviation { get; set; }

        /// <summary>
        /// 顾客满意度（期望偏离度+100%）
        /// </summary>
        public float satisfaction { get; set; }
    }
}
