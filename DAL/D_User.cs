using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DAL
{
    /// <summary>
    /// 系统人员
    /// </summary>
    public class D_User
    {
        /// <summary>
        /// 查询
        /// </summary>
        public List<user> GetList()
        {
            List<user> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_user");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<user>(strSql.ToString())?.ToList();
            }
            return list;
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int GetCount()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from dp_user with (nolock)");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                return (int)conn.ExecuteScalar(strSql.ToString());
            }
            return 0;
        }

        /// <summary>
        /// 查询
        /// </summary>
        public user GetInfoById(user model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_user where userid=@userid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<user>(strSql.ToString(), model)?.FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public bool Add(user model)
        {
            string sql = @"INSERT INTO [comment].[dbo].[dp_user]([username],[password],[areaid],[classinfoid],[updatetime]) 
                            VALUES (@username,@password,@areaid,@classinfoid,@updatetime)";
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
        public bool Update(user model)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"UPDATE [comment].[dbo].[dp_user]
                               SET [username] = @username
                                  ,[password] = @password
                                  ,[areaid] = @areaid
                                  ,[classinfoid] = @classinfoid
                                  ,[updatetime] = @updatetime
                             WHERE userid=@userid");

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
        public bool DeleteById(user model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("DELETE FROM [comment].[dbo].[dp_user] where userid=@userid");

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
