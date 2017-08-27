using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Comp;

namespace DAL
{
   public class D_Person
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<E_Person> GetList(E_Person model, ref int total)
        {
            if (model.PageIndex <= 0) { model.PageIndex = 1; }


            List<E_Person> list;
            StringBuilder strSql = new StringBuilder();
            StringBuilder whereSql = new StringBuilder(" where isdelete=0 ");
            strSql.Append(" select  ROW_NUMBER() OVER ( ORDER BY personid desc) AS RID, * from dp_person ");
            if (model.areaid > 0)
            {
                whereSql.Append(" and areaid=@areaid");
            }
            if (model.classinfoid > 0) {
                whereSql.Append(" and classinfoid=@classinfoid");
            }
            if (!String.IsNullOrEmpty(model.personname)) {
                whereSql.Append(" and personname like '%'+@personname +'%'");
            }
            strSql.Append(whereSql);
            string CountSql = "SELECT COUNT(1) as RowsCount FROM (" + strSql.ToString() + ") AS CountList";


            string pageSqlStr = "select * from ( " + strSql.ToString() + " ) as Temp_PageData where Temp_PageData.RID BETWEEN {0} AND {1}";
            pageSqlStr = string.Format(pageSqlStr, (model.PageSize * (model.PageIndex - 1) + 1).ToString(), (model.PageSize * model.PageIndex).ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_Person>(pageSqlStr, model)?.ToList();
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
        public E_Person GetInfoById(E_Person model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from dp_person where personid=@personid");
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                model = conn.Query<E_Person>(strSql.ToString(), model)?.FirstOrDefault();

            }
            return model;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(E_Person model)
        {
            string sql = "INSERT INTO dp_person(areaid, classinfoid, personname, jobtypeid) VALUES (@areaid, @classinfoid, @personname, @jobtypeid)";
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
        /// 评价
        /// </summary>
        /// <returns></returns>
        public bool SumLike(E_Person model) {
            StringBuilder strSql = new StringBuilder();
            StringBuilder setSql = new StringBuilder();

          

            if (model.perfect != null && model.perfect > 0) {
                setSql.Append("perfect=perfect+1,");
            }
            if (model.good != null && model.good > 0)
            {
                setSql.Append("good=good+1,");
            }
            if (model.bad != null && model.bad > 0)
            {
                setSql.Append("bad=bad+1,");
            }
            strSql.Append("update dp_person set "+setSql.ToString().TrimEnd(',')+"  where personid=@personid ");
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
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(E_Person model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_person set areaid=@areaid, classinfoid=@classinfoid, personname=@personname, jobtypeid=@jobtypeid  where personid=@personid ");
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
        public bool DeleteById(E_Person model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update dp_person set isdelete=1  where personid=@personid ");
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
