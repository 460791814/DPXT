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
                    new E_OpinionType(){  OpinionTypeId=2, OpinionName="保洁服务投诉意见"}
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
    }
}