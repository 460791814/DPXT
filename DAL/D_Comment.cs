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
    /// 点评记录
    /// </summary>
    public class D_Comment
    {
        /// <summary>
        /// 添加
        /// </summary>
        public int Add(E_Comment model)
        {
            string sql = @"INSERT INTO [comment].[dbo].[dp_comment]([userid],[areaid],[classinfoid],[sex],[ageround],[domain],[job],[contents],[addtime])
                            VALUES(@userid,@areaid,@classinfoid,@sex,@ageround,@domain,@job,@contents,@addtime);SELECT @@IDENTITY;";
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                return Convert.ToInt32(conn.ExecuteScalar(sql, model));
            }
        }

    }
}
