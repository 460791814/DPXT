using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 评论表
    /// </summary>
    public class E_Comment
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public int commentid { get; set; }

        /// <summary>
        /// 巡检人用户ID
        /// </summary>
        public int userid { get; set; }

        /// <summary>
        /// 作业区ID
        /// </summary>
        public int areaid { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        public int classinfoid { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int ageround { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public int domain { get; set; }

        /// <summary>
        /// 人员类型
        /// </summary>
        public int job { get; set; }

        /// <summary>
        /// 其他意见
        /// </summary>
        public string contents { get; set; }

        /// <summary>
        /// 点评时间
        /// </summary>
        public DateTime addtime { get; set; }

        /// <summary>
        /// 主题ID
        /// </summary>
        public int themeid { get; set; }

        /// <summary>
        /// 点评项集合
        /// </summary>
        public List<E_CommentCommentItem> commentcommentitem { get; set; }

        /// <summary>
        /// 点评意见集合
        /// </summary>
        public List<E_CommentOpinion> commentopinion { get; set; }
    }
}
