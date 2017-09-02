using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 评论-评论项目
    /// </summary>
    public class E_CommentCommentItem
    {
        /// <summary>
        /// 点评记录ID
        /// </summary>
        public int commentid { get; set; }

        /// <summary>
        /// 点评项ID
        /// </summary>
        public int commentitemid { get; set; }

        /// <summary>
        /// 期望值
        /// </summary>
        public int expect { get; set; }

        /// <summary>
        /// 感知值
        /// </summary>
        public int real { get; set; }
    }
}
