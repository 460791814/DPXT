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
    /// 班组
    /// </summary>
    public class D_ClassInfo
    {
        /// <summary>
        /// 查询
        /// </summary>
        public List<classinfo> GetList()
        {
            List<classinfo> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from classinfo");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<classinfo>(strSql.ToString())?.ToList();
            }
            return list;
        }
    }
}
