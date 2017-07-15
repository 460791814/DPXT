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
    public class ThemeController : Controller
    {
        D_Theme dal = new D_Theme();
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: CommentType
      [AuthAttribute]
        public ActionResult List()
        {
            int pageIndex = Utils.GetInt(Request["page"]);
          //  ViewBag.page = Utils.ShowPage(100, 15, pageIndex, 5);
            ViewBag.list = dal.GetList();
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(E_Theme model)
        {
            if (model != null && model.themeid > 0)
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
        public bool Delete(E_Theme model)
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

            E_Theme model = new E_Theme();
            model.themeid = id;
            ViewBag.Info = dal.GetInfoById(model);
            return View();
        }
    }
}