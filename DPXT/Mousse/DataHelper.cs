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

    }
}