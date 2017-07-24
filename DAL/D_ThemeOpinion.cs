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
   public class D_ThemeOpinion
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<dp_themeopinion> GetList(dp_themeopinion model)
        {
            List<dp_themeopinion> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_themeopinion where themeid=@themeid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<dp_themeopinion>(strSql.ToString(),model)?.ToList();

            }
            return list;
        }
        public bool Add(dp_themeopinion model)
        {
            string sql = "INSERT INTO dp_themeopinion(themeid,opinionid) VALUES (@themeid,@opinionid)";
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                int count = conn.Execute(sql, model);
                if (count > 0)//如果更新失败
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteById(dp_themeopinion model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete dp_themeopinion    where themeid=@themeid ");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                int count = conn.Execute(strSql.ToString(), model);
                if (count > 0)//如果更新失败
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
