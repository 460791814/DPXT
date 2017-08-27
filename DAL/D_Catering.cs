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
   public class D_Catering
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string sql)
        {
            List<T> list = null;
         
            using (IDbConnection conn = DapperHelper.OpenConnection(DB.CMS_Catering))
            {
                list = conn.Query<T>(sql)?.ToList();

            }
            return list;
        }
    }
}
