using Comp;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPXT
{
    public class DataHelper
    {
       static  D_CommentType dCommentType = new D_CommentType();
        static D_Area dalArea = new D_Area();
        static D_ClassInfo dalclassinfo = new D_ClassInfo();
        static D_Catering dCatering = new D_Catering();
        public static List<E_CommentType> GetCommentTypeList()
        {
          return  dCommentType.GetList();
        }
        /// <summary>
        /// 根据ID获取评论维度名称
        /// </summary>
        /// <param name="CommentTypeId"></param>
        /// <returns></returns>
        public static string GetCommentTypeName(int? CommentTypeId) {
            return GetCommentTypeList().Find(a => a.commenttypeid == CommentTypeId).typename;
        }

        public static List<E_OpinionType> GetOpinionTypeList()
        {
            List<E_OpinionType> list = new List<E_OpinionType>() {
                 new E_OpinionType(){  OpinionTypeId=1, OpinionName="巡检点评意见"},
                   new E_OpinionType(){  OpinionTypeId=2, OpinionName="配餐服务投诉意见"},
                    //new E_OpinionType(){  OpinionTypeId=3, OpinionName="保洁服务投诉意见"}
            }; 
            return list;
        }

        public static string GetOpinionTypeName(int? id)
        {
            return GetOpinionTypeList().Find(a => a.OpinionTypeId == id).OpinionName;
        }

        public static Dictionary<int,String> GetJobTypeList()
        {
            Dictionary<int, String> dic = new Dictionary<int, string>();
            dic.Add(1, "厨师");
            dic.Add(2, "打餐");
            dic.Add(3, "保洁");
            return dic;
        }

        public static String GetJobTypeName(int id) {
            return GetJobTypeList()[id];
        }
        public static List<E_ClassInfo> GetClassInfoList()
        {
            return dalclassinfo.GetList(new E_ClassInfo());
        }
        public static String GetClassInfoName(int? id) {
            return GetClassInfoList().Find(a => a.id == id)?.cname;
        }
        public static List<E_Area> GetAreaList() {
            return dalArea.GetList();
        }
        public static string GetCurrentTime()
        {
            string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return DateTime.Now.ToString("yyyy-MM-dd")+"  "+ Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();
        }
        public static List<Personnel> GetChuShiList()
        {
            List<Personnel> list = null;
            object obj = Utils.Cache("chushi");
            if (obj != null)
            {
                list = (List<Personnel>)obj;
            }
            else {
                list = dCatering.GetList<Personnel>("select id,Name from Personnel where Professional='厨师'");
                Utils.Cache("chushi", list, DateTime.Now.AddDays(3));
            }
            return list;

        }
    }
}