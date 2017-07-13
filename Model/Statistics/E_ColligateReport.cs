using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Statistics
{
    /// <summary>
    /// 综合报表
    /// </summary>
    public class E_ColligateReport
    {
        /// <summary>
        /// 评价维度
        /// </summary>
        public string typename { get; set; }
        
        /// <summary>
        /// 调查问项
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 男，期望值（E）
        /// </summary>
        public int sex_1_expect { get; set; }
        /// <summary>
        /// 女，期望值（E）
        /// </summary>
        public int sex_2_expect { get; set; }
        /// <summary>
        /// 男，感知值（P）
        /// </summary>
        public int sex_1_real { get; set; }
        /// <summary>
        /// 女，感知值（P）
        /// </summary>
        public int sex_2_real { get; set; }
        /// <summary>
        /// 18-30岁，期望值（E）
        /// </summary>
        public int ageround_1_expect { get; set; }
        /// <summary>
        /// 30-45岁，期望值（E）
        /// </summary>
        public int ageround_2_expect { get; set; }
        /// <summary>
        /// 45岁以上，期望值（E）
        /// </summary>
        public int ageround_3_expect { get; set; }
        /// <summary>
        /// 18-30岁，感知值（P）
        /// </summary>
        public int ageround_1_real { get; set; }
        /// <summary>
        /// 30-45岁，感知值（P）
        /// </summary>
        public int ageround_2_real { get; set; }
        /// <summary>
        /// 45岁以上，感知值（P）
        /// </summary>
        public int ageround_3_real { get; set; }
        /// <summary>
        /// 南方，期望值（E）
        /// </summary>
        public int domain_1_expect { get; set; }
        /// <summary>
        /// 北方，期望值（E）
        /// </summary>
        public int domain_2_expect { get; set; }
        /// <summary>
        /// 南方，感知值（P）
        /// </summary>
        public int domain_1_real { get; set; }
        /// <summary>
        /// 北方，感知值（P）
        /// </summary>
        public int domain_2_real { get; set; }
        /// <summary>
        /// 定员，期望值（E）
        /// </summary>
        public int job_1_expect { get; set; }
        /// <summary>
        /// 访客，期望值（E）
        /// </summary>
        public int job_2_expect { get; set; }
        /// <summary>
        /// 定员，感知值（P）
        /// </summary>
        public int job_1_real { get; set; }
        /// <summary>
        /// 访客，感知值（P）
        /// </summary>
        public int job_2_real { get; set; }
        /// <summary>
        /// 男差值(P-E)
        /// </summary>
        public int sex_1_different { get; set; }
        /// <summary>
        /// 女差值（P-E）
        /// </summary>
        public int sex_2_different { get; set; }
        /// <summary>
        /// 18-30岁 差值(P-E)
        /// </summary>
        public int ageround_1_different { get; set; }
        /// <summary>
        /// 30-45岁 差值（P-E）
        /// </summary>
        public int ageround_2_different { get; set; }
        /// <summary>
        /// 45岁以上 差值（P-E）
        /// </summary>
        public int ageround_3_different { get; set; }
        /// <summary>
        /// 南方 差值（P-E）
        /// </summary>
        public int domain_1_different { get; set; }
        /// <summary>
        /// 北方 差值（P-E）
        /// </summary>
        public int domain_2_different { get; set; }
        /// <summary>
        /// 定员 差值（P-E）
        /// </summary>
        public int job_1_different { get; set; }
        /// <summary>
        /// 访客 差值（P-E）
        /// </summary>
        public int job_2_different { get; set; }
    }
}
