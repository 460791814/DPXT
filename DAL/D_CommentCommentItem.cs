using Dapper;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// 点评-点评项
    /// </summary>
    public class D_CommentCommentItem
    {
        /// <summary>
        /// 添加
        /// </summary>
        public bool Add(E_CommentCommentItem model)
        {
            string sql = @"INSERT INTO [comment].[dbo].[dp_commentcommentitem]([commentid],[commentitemid],[expect],[real])
                           VALUES (@commentid,@commentitemid,@expect,@real);";
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                return conn.Execute(sql, model) > 0;
            }
        }
    }
}
