using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Model;

namespace DAL
{
   public class D_Catering
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string sql)
        {
            List<T> list = null;
         
            using (IDbConnection conn = DapperHelper.OpenConnection(DB.CMS_Catering))
            {
                list = conn.Query<T>(sql)?.ToList();

            }
            return list;
        }
        public List<Personnel> GetChuShi() {
            return GetList<Personnel>("select id,Name from Personnel where Professional='厨师'");
        }
        /// <summary>
        /// 获取当天菜
        /// </summary>
        /// <param name="date"></param>
        /// <param name="areaId"></param>
        /// <param name="classIds"></param>
        /// <returns></returns>
        public List<E_Dish> GetDishCurrDay(DateTime date) {
            string sql = @"select a.id,MID,CID,Name,Picture,b.AreaID,b.ClassID from (
                                    select id,MID,CID from dbo.RecipeInformation where Date='{0}'  
                                    ) a 
                                    inner join dbo.Recipes b on b.id=a.MID
                                    inner join Dish c on c.id=a.CID";
            sql = string.Format(sql, date );
            return GetList<E_Dish>(sql);
        }
    }
}
