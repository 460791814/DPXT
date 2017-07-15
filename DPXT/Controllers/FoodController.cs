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
    public class FoodController : Controller
    {
        D_Food dal = new D_Food();

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [AuthAttribute]
        public ActionResult List(E_Food model)
        {

            model.PageIndex = Utils.GetInt(Request["page"]);
            int count = 0;
            ViewBag.list = dal.GetList(model, ref count);
            ViewBag.page = Utils.ShowPage(count, model.PageSize, model.PageIndex, 5);
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(E_Food model)
        {
            if (model != null && model.foodid > 0)
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
        public bool Delete(E_Food model)
        {

            return dal.DeleteById(model);


        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Display(E_Food model)
        {

            return dal.DisplayById(model);


        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Info(int id)
        {

            E_Food model = new E_Food();
            model.foodid = id;
            ViewBag.Info = dal.GetInfoById(model);

            return View();
        }
        
    }
}