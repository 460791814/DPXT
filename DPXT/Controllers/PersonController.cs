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
    public class PersonController : Controller
    {
        D_Person dal = new D_Person();
        D_Area dalarea = new D_Area();
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [AuthAttribute]
        public ActionResult List(E_Person model)
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
        public bool Save(E_Person model)
        {
            if (model != null && model.personid > 0)
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
        public bool Delete(E_Person model)
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

            E_Person model = new E_Person();
            model.personid = id;
            ViewBag.Info = dal.GetInfoById(model);

            return View();
        }
    }
}