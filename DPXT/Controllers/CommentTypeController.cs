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
        // GET: CommentType
        public ActionResult List()
        {
            int pageIndex = Utils.GetInt(Request["page"]);
            ViewBag.page = Utils.ShowPage(100, 15, pageIndex, 5);
            ViewBag.list = dal.GetList();
            return View();
        }

        public bool Save(commenttype model)
        {
            if (model != null && model.commenttypeid > 0)
            {
                return dal.Update(model);
            }
            return dal.Add(model);
        }


        public ActionResult Info(int id)
        {

            commenttype model = new commenttype();
            model.commenttypeid = id;
            ViewBag.Info = dal.GetInfoById(model);
            return View();
        }
    }
}