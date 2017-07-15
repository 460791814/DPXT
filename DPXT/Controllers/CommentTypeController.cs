using Comp;
using DAL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPXT.Controllers
{
    public class CommentTypeController : Controller
    {
        D_CommentType dal = new D_CommentType();
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [AuthAttribute]
        public ActionResult List()
        {
            int pageIndex = Utils.GetInt(Request["page"]);
            ViewBag.page = Utils.ShowPage(100, 15, pageIndex, 5);
            ViewBag.list = dal.GetList();
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(E_CommentType model)
        {
            if (model != null && model.commenttypeid > 0)
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
        public bool Delete(E_CommentType model)
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

            E_CommentType model = new E_CommentType();
            model.commenttypeid = id;
            ViewBag.Info = dal.GetInfoById(model);
            return View();
        }
    }
}