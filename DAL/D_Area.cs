using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DAL
{
    /// <summary>
    /// 作业区
    /// </summary>
    public class D_Area
    {
        /// <summary>
        /// 查询
        /// </summary>
        public List<area> GetList()
        {
            List<area> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from area");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<area>(strSql.ToString())?.ToList();
            }
            return list;
        }
    }
}
