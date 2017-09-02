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
    /// 点评-点评意见
    /// </summary>
    public class D_CommentOpinion
    {
        /// <summary>
        /// 添加
        /// </summary>
        public bool Add(E_CommentOpinion model)
        {
            string sql = @"INSERT INTO [comment].[dbo].[dp_commentopinion]([commentid],[opinionid])
                          VALUES(@commentid,@opinionid);";
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                return conn.Execute(sql, model) > 0;
            }
        }
    }
}
