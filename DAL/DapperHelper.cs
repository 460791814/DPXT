using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL
{
    public class DapperHelper
    {

        public static IDbConnection OpenConnection(DB db)
        {
            string connStr;
            switch (db)
            {
                case DB.comment:
                    connStr = GetConStr("ConnString");
                    break;
                case DB.CMS_Catering:
                    connStr = GetConStr("CateringConnString");
                    break;
 
                default:
                    connStr = GetConStr("ConnString");
                    break;
            }
            return new SqlConnection(connStr);
        }
        public  static string GetConStr(string str = "ConnString")
        {

            return ConfigurationManager.ConnectionStrings[str].ToString();
        }


       
    }

    public enum DB
    {
        comment,
        CMS_Catering,
       
    }
}
