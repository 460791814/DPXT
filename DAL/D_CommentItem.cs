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
    public class D_CommentItem
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<E_CommentItem> GetList(E_CommentItem model)
        {
            List<E_CommentItem> list;
            StringBuilder strSql = new StringBuilder();
            StringBuilder whereSql = new StringBuilder(" where isdelete=0 ");
            strSql.Append("select * from dp_commentitem ");
            if (model.commenttypeid > 0) {
                whereSql.Append(" and commenttypeid=@commenttypeid");
            }
            strSql.Append(whereSql);
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_CommentItem>(strSql.ToString(),model)?.ToList();

            }
            return list;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public E_CommentItem GetInfoById(E_CommentItem model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_commentitem where commentitemid=@commentitemid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_CommentItem>(strSql.ToString(), model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(E_CommentItem model)
        {
            string sql = "INSERT INTO dp_commentitem(commenttypeid,[title]) VALUES (@commenttypeid,@title)";
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
        public bool Update(E_CommentItem model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_commentitem set title=@title,commenttypeid=@commenttypeid  where commentitemid=@commentitemid ");
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
        public bool DeleteById(E_CommentItem model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_commentitem set isdelete=1  where commentitemid=@commentitemid ");
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
