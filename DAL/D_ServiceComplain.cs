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
    public class D_ServiceComplain
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(dp_servicecomplain model)
        {
            string sql = @"INSERT INTO dp_servicecomplain
           ([areaid]
           ,[classinfoid]
           ,[contents]
           ,[addtime])
     VALUES
           (
            @areaid
           ,@classinfoid
           ,@contents
           ,@addtime);SELECT CAST(SCOPE_IDENTITY() as int)";
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                int identity = conn.Query<int>(sql, model).FirstOrDefault();
               
                return identity;
            }
        }
        public bool AddServiceComplainOpinion(dp_servicecomplainopinion model) {
            string sql = @"INSERT INTO dp_servicecomplainopinion
           ([servicecomplainid]
           ,[opinionid])
           VALUES
           (@servicecomplainid
           ,@opinionid)";
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
    }
}
