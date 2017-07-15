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
    public class OpinionController : Controller
    {
        D_Opinion dal = new D_Opinion();
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: CommentType
        [AuthAttribute]
        public ActionResult List(E_Opinion model)
        {
    
            model.PageIndex = Utils.GetInt(Request["page"]);
            int count = 0;
            ViewBag.list = dal.GetList(model,ref count);
            ViewBag.page = Utils.ShowPage(count, model.PageSize, model.PageIndex, 5);
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(E_Opinion model)
        {
            if (model != null && model.opinionid > 0)
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
        public bool Delete(E_Opinion model)
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

            E_Opinion model = new E_Opinion();
            model.opinionid = id;
            ViewBag.Info = dal.GetInfoById(model);
            return View();
        }
    }
}