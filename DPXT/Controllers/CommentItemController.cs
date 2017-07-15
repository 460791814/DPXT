using Comp;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPXT.Controllers
{
    public class CommentItemController : Controller
    {
        D_CommentItem dal = new D_CommentItem();
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [AuthAttribute]
        public ActionResult List(E_CommentItem model)
        {
           
        
            ViewBag.list = dal.GetList(model);
            
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(E_CommentItem model)
        {
            if (model != null && model.commentitemid > 0)
            {
                return dal.Update(model);
            }
            return dal.Add(model);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(E_CommentItem model)
        {

            return dal.DeleteById(model);


        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Info(int id)
        {

            E_CommentItem model = new E_CommentItem();
            model.commentitemid = id;
            ViewBag.Info = dal.GetInfoById(model);
            return View();
        }
    }
}