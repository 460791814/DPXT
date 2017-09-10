using Comp;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPXT.App_Start
{
    public class PCAuthAttribute : ActionFilterAttribute
    {
        D_User dal = new D_User();

        /// <summary>
        /// 验证权限（action执行前会先执行这里）
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["user_pc"] == null)
            {
                E_User model = new E_User();
                model.username = Utils.GetCookies("username");
                model.password = Utils.GetCookies("password");

                E_User eUser = dal.GetInfoByName(model);
                if (eUser == null)
                {
                    filterContext.Result = new RedirectResult("/PC/Login/");
                    return;
                }
                if (eUser.password != model.password)
                {
                    filterContext.Result = new RedirectResult("/PC/Login/");
                    return;
                }
                filterContext.HttpContext.Session["user_pc"] = eUser;
            }

        }

    }
}