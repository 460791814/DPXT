
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Model;

namespace DAL
{
   public class D_Theme
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<E_Theme> GetList()
        {
            List<E_Theme> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_theme where isdelete=0");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_Theme>(strSql.ToString())?.ToList();

            }
            return list;
        }
        
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public E_Theme GetInfoById(E_Theme model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_theme where themeid=@themeid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_Theme>(strSql.ToString(), model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(E_Theme model)
        {
            string sql = "INSERT INTO dp_theme([themename]) VALUES (@themename)";
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
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(E_Theme model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_theme set themename=@themename  where themeid=@themeid ");
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
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteById(E_Theme model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_theme set isdelete=1  where themeid=@themeid ");
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
