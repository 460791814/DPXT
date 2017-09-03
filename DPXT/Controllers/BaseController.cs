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
    /// <summary>
    /// 所有需要登录功能controller的父类
    /// </summary>
    public class BaseController : Controller
    {
        D_User dal = new D_User();
        /// <summary>
        /// 角色名称
        /// </summary>
 
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session["user"] == null)
            {
                E_User model = new E_User();
                model.username = Utils.GetCookies("username");
                model.password = Utils.GetCookies("password");

                E_User eUser = dal.GetInfoByName(model);
                if (eUser == null)
                {
                    filterContext.Result = new RedirectResult("/Login/Login/");
                    return;
                }
                if (eUser.password != model.password)
                {
                    filterContext.Result = new RedirectResult("/Login/Login/");
                    return;
                }
                filterContext.HttpContext.Session["user"] = eUser;
            }
        }
    }
}