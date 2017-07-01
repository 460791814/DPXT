using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPXT.Controllers
{
    public class UserController : Controller
    {
        //D_CommentType dal = new D_CommentType();
        ///// <summary>
        ///// 列表
        ///// </summary>
        ///// <returns></returns>
        //// GET: CommentType
        //public ActionResult List()
        //{
        //    int pageIndex = Utils.GetInt(Request["page"]);
        //    ViewBag.page = Utils.ShowPage(100, 15, pageIndex, 5);
        //    ViewBag.list = dal.GetList();
        //    return View();
        //}
        ///// <summary>
        ///// 保存
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public bool Save(commenttype model)
        //{
        //    if (model != null && model.commenttypeid > 0)
        //    {
        //        return dal.Update(model);
        //    }
        //    return dal.Add(model);
        //}
        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public bool Delete(commenttype model)
        //{

        //    return dal.DeleteById(model);


        //}
        ///// <summary>
        ///// 详情
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult Info(int id)
        //{

        //    commenttype model = new commenttype();
        //    model.commenttypeid = id;
        //    ViewBag.Info = dal.GetInfoById(model);
        //    return View();
        //}
    }
}
