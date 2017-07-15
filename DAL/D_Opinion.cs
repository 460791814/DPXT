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
   public class D_Opinion
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<E_Opinion> GetList(E_Opinion model, ref int total)
        {
            if (model.PageIndex <= 0) { model.PageIndex = 1; }
         

            List<E_Opinion> list;
            StringBuilder strSql = new StringBuilder();
            StringBuilder whereSql = new StringBuilder(" where isdelete=0 ");
            strSql.Append(" select  ROW_NUMBER() OVER ( ORDER BY opinionid desc) AS RID, * from dp_opinion ");
            if (model.opiniontypeid > 0)
            {
                whereSql.Append(" and opiniontypeid=@opiniontypeid");
            }
            strSql.Append(whereSql);
            string CountSql = "SELECT COUNT(1) as RowsCount FROM (" + strSql.ToString() + ") AS CountList";
  
 
            string pageSqlStr = "select * from ( " + strSql.ToString() + " ) as Temp_PageData where Temp_PageData.RID BETWEEN {0} AND {1}";
            pageSqlStr = string.Format(pageSqlStr, (model.PageSize * (model.PageIndex - 1) + 1).ToString(), (model.PageSize * model.PageIndex).ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_Opinion>(pageSqlStr, model)?.ToList();
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
        public E_Opinion GetInfoById(E_Opinion model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_opinion where opinionid=@opinionid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_Opinion>(strSql.ToString(), model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(E_Opinion model)
        {
            string sql = "INSERT INTO dp_opinion(opiniontypeid,[opinioncontent]) VALUES (@opiniontypeid,@opinioncontent)";
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
        public bool Update(E_Opinion model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_opinion set opinioncontent=@opinioncontent,opiniontypeid=@opiniontypeid  where opinionid=@opinionid ");
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
        public bool DeleteById(E_Opinion model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_opinion set isdelete=1  where opinionid=@opinionid ");
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
