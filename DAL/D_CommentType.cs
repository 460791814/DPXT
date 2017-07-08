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
        public  List<E_CommentType> GetList() 
        {
            List<E_CommentType> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_commenttype where isdelete=0"); 
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_CommentType>(strSql.ToString())?.ToList();

            }
            return list;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public E_CommentType GetInfoById(E_CommentType model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_commenttype where commenttypeid=@commenttypeid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_CommentType>(strSql.ToString(),model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public  bool Add(E_CommentType model)
        {
            string sql = "INSERT INTO dp_commenttype([typename]) VALUES (@typename)";
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
        public  bool Update(E_CommentType model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_commenttype set typename=@typename  where commenttypeid=@commenttypeid ");
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
        public bool DeleteById(E_CommentType model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_commenttype set isdelete=1  where commenttypeid=@commenttypeid ");
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
