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
   public  class D_Food
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<E_Food> GetList(E_Food model, ref int total)
        {
            if (model.PageIndex <= 0) { model.PageIndex = 1; }


            List<E_Food> list;
            StringBuilder strSql = new StringBuilder();
            StringBuilder whereSql = new StringBuilder(" where 1=1 ");
            strSql.Append(" select  ROW_NUMBER() OVER ( ORDER BY UpdateTime desc) AS RID, * from dp_food ");
            if (model.areaid > 0)
            {
                whereSql.Append(" and areaid=@areaid");
            }
            if (model.classinfoid > 0)
            {
                whereSql.Append(" and classinfoid=@classinfoid");
            }
            if (!String.IsNullOrEmpty(model.foodname))
            {
                whereSql.Append(" and foodname like '%'+@foodname +'%'");
            }
            if (!String.IsNullOrEmpty(model.pname))
            {
                whereSql.Append(" and pname like '%'+@pname +'%'");
            }
            if (model.addtime != null) {
                whereSql.Append(" and addtime=@addtime");
            }
            if (model.startaddtime != null)
            {
                whereSql.Append(" and addtime>=@startaddtime");
            }
            strSql.Append(whereSql);
            string CountSql = "SELECT COUNT(1) as RowsCount FROM (" + strSql.ToString() + ") AS CountList";


            string pageSqlStr = "select * from ( " + strSql.ToString() + " ) as Temp_PageData where Temp_PageData.RID BETWEEN {0} AND {1}";
            pageSqlStr = string.Format(pageSqlStr, (model.PageSize * (model.PageIndex - 1) + 1).ToString(), (model.PageSize * model.PageIndex).ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_Food>(pageSqlStr, model)?.ToList();
                total = conn.ExecuteScalar<int>(CountSql, model);
            }
            return list;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public E_Food GetInfoById(E_Food model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_food where foodid=@foodid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_Food>(strSql.ToString(), model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(E_Food model)
        {
            string sql = "INSERT INTO dp_food(areaid, classinfoid, foodname,pid, pname, pic,updatetime,addtime) VALUES (@areaid, @classinfoid, @foodname,@pid, @pname, @pic,getdate(),getdate())";
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
        public bool Update(E_Food model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_food set areaid=@areaid, classinfoid=@classinfoid,foodname=@foodname,pid=@pid,pname=@pname,pic=@pic,updatetime=getdate()  where foodid=@foodid ");
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
        public bool UpdateHits(E_Food model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder setSql = new StringBuilder();
           
            if (model.praise > 0) {
                setSql.Append("praise=praise+1");
            }
            if (model.bad > 0) {
                setSql.Append("bad=bad+1");
            }
            strSql.Append("update dp_food set  "+setSql+"  where foodid=@foodid ");
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
        public bool DeleteById(E_Food model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete dp_food   where foodid=@foodid ");
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
        /// 隐藏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DisplayById(E_Food model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_food set isdisplay=@isdisplay  where foodid=@foodid ");
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
