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
                case DB.Log:
                    connStr = GetConStr("LogConnString");
                    break;
                case DB.Zxxk:
                    connStr = GetConStr("ConnString");
                    break;
                case DB.WXT:
                    connStr = GetConStr("WxtConnString");
                    break;
                case DB.Sales:
                    connStr = GetConStr("SalesConnString");
                    break;
                default:
                    connStr = GetConStr("UserConnString");
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
        User,
        Zxxk,
        Log,
        WXT, //王学通
        Sales //销售
    }
}
