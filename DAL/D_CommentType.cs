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
   public class D_CommentType
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public  List<commenttype> GetList() 
        {
            List<commenttype> list;
       
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from commenttype where isdelete=0"); 

            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<commenttype>(strSql.ToString())?.ToList();

            }
            return list;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public commenttype GetInfoById(commenttype model)
        {
          

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from commenttype where commenttypeid=@commenttypeid");

            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<commenttype>(strSql.ToString(),model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public  bool Add(commenttype model)
        {
          


            string sql = "INSERT INTO [comment].[dbo].[CommentType]([typename]) VALUES (@typename)";



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
        public  bool Update(commenttype model)
        {
 
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CommentType set typename=@typename  where commenttypeid=@commenttypeid ");

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
        public bool DeleteById(commenttype model)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CommentType set isdelete=1  where commenttypeid=@commenttypeid ");

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
