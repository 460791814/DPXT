using System;
using System.Collections.Generic;

namespace HtmlTableToExecl
{
    /// <summary>
    /// 单元格
    /// </summary>
    public class E_Cell
    {
        /// <summary>
        /// 单元格索引
        /// </summary>
        public int cellindex
        {
            get;
            set;
        }

        /// <summary>
        /// 单元格内容
        /// </summary>
        public string content
        {
            get;
            set;
        }

        /// <summary>
        /// 单元格宽度（所占用单元格个数）
        /// </summary>
        public int colspan
        {
            get;
            set;
        }

        /// <summary>
        /// 单元格高度（占单元格个数）
        /// </summary>
        public int rowspan
        {
            get;
            set;
        }
        
        public List<E_Row> RowS
        {
            get;
            set;
        }
    }
}
