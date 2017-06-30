using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace GaoKaoXML.Data
{
    public class DataUtils
    {
        public static string _connection = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public  static bool Update(object model)
        {
            Type modelType = model.GetType();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update "+ modelType.Name + " set ");

        
            var properties = model.GetType().GetProperties();
            foreach (var item in properties)
            {
                if (item.Name == "id" ) { continue; }

                //if (item.GetValue(model) != null)
                //{
                //    if (item.PropertyType.Name == "String")
                //    {
                     
                //        var val = item.GetValue(model);
                //        if (val == null)
                //        {
                //            continue;
                //        }
                     
                //    }
                //    strSql.AppendFormat(" {0} = @{0},", item.Name);
                //}
                strSql.AppendFormat(" {0} = @{0},", item.Name);
            }



            string sql = strSql.ToString().TrimEnd(',');

            sql += " where id=@id ";
           
            using (IDbConnection conn = new SqlConnection(_connection))
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
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Add(object model)
        {
            Type modelType = model.GetType();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into " + modelType.Name + " ({0}) values ({1}) ");


            var properties = model.GetType().GetProperties();
            StringBuilder sqlKey = new StringBuilder();
            StringBuilder sqlValue = new StringBuilder();
            foreach (var item in properties)
            {
                if (item.Name == "id") { continue; }


                sqlKey.AppendFormat(" {0},", item.Name);
                sqlValue.AppendFormat(" @{0},", item.Name);

            }

            string sql = "insert into " + modelType.Name + " ("+ sqlKey.ToString().TrimEnd(',') + ") values ("+ sqlValue.ToString().TrimEnd(',') + ") "; 

        

            using (IDbConnection conn = new SqlConnection(_connection))
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
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<T> Select<T>(T model) where T : new()
        {
            List<T> list ;
            Type modelType = model.GetType();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from "+ modelType.Name); //+" order by ID DESC"
            object obj= modelType.GetProperty("id").GetValue(model, null);
            int id = GetInt(obj);
            if (id > 0)
            {
                strSql.Append(" where id= " + id);
            }
            else {
                strSql.Append(" order by ID DESC");
            }
            using (IDbConnection conn = new SqlConnection(_connection))
            {
                list = conn.Query<T>(strSql.ToString())?.ToList();
          
            }
            return list;
        }
        /// <summary>
        /// 将字符串转换为数字
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static int GetInt(object Obj)
        {
            if (Obj == null)
            {
                return 0;
            }
            return GetInt(Obj.ToString());
        }
        /// <summary>
        /// 将字符串转换为数字
        /// </summary>
        /// <param name="Str">待转换字符串</param>
        /// <returns>转换后的数字</returns>
        public static int GetInt(string Str)
        {
            if (String.IsNullOrEmpty(Str)) return 0;
            try
            {
                if (Str.IndexOf('.') > -1)
                {
                    return Convert.ToInt32(Convert.ToSingle(Str));
                }
                else
                {
                    return Convert.ToInt32(Str);
                }
            }
            catch { return 0; }
        }
    }
}