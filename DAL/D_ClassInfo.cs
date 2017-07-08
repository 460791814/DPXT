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
        public List<E_ClassInfo> GetList(E_ClassInfo model)
        {
            List<E_ClassInfo> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from classinfo where areaid=@areaid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_ClassInfo>(strSql.ToString(), model)?.ToList();
            }
            return list;
        }
    }
}
