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
        public List<E_User> GetList(E_User model)
        {
            List<E_User> list;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from
                        (
                            select row_number()over(order by userid asc) as rowid, dp_user.*, Area.Name as areaname, ClassInfo.CName as classinfoname from dp_user with (nolock)
                            left join dbo.Area on dp_user.areaid = dbo.Area.id
                            left join dbo.ClassInfo on dp_user.classinfoid = dbo.ClassInfo.id
                        ) as T where t.rowid between @pageindex*@pagesize and (@pageindex+1)*@pagesize");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_User>(strSql.ToString(), model)?.ToList();
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
        public E_User GetInfoById(E_User model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_user where userid=@userid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_User>(strSql.ToString(), model)?.FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// 查询
        /// </summary>
        public E_User GetInfoByName(E_User model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select *,Name as areaname,CName as classinfoname  from (
                            select * from dp_user  where username =@username and password=@password) a
                              left join dbo.Area b on a.areaid = b.id
                            left join dbo.ClassInfo c on a.classinfoid = c.id");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_User>(strSql.ToString(), model)?.FirstOrDefault();
            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        public bool Add(E_User model)
        {
            string sql = @"INSERT INTO [comment].[dbo].[dp_user]([realname],[username],[password],[areaid],[classinfoid],[updatetime]) 
                            VALUES (@realname,@username,@password,@areaid,@classinfoid,@updatetime)";
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
        public bool Update(E_User model)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"UPDATE [comment].[dbo].[dp_user]
                               SET [realname] = @realname 
                                  ,[username] = @username
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
        public bool DeleteById(E_User model)
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
