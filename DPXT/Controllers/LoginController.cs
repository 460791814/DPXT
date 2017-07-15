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
    public class LoginController : Controller
    {
        D_User dal = new D_User();
        // GET: Login
        public ActionResult Login()
        {
            Session["user"] = null;
            Utils.ClearCookies("password");
            return View();
        }
        

        public String DoLoginAjax(E_User model)
        {
            E_User eUser = dal.GetInfoByName(model);
            String result = "";
            if (eUser != null)
            {
                if (eUser.password == model.password)
                {
                    Utils.AddCookies("username", model.username);
                    Utils.AddCookies("password", model.password);
                    Session["user"] = model;
                    return "True" ;
                }
                else
                {
                    result = "密码错误";

                }
            }
            else
            {
                result = "账号不存在";
            }
            
            return result;
        }
    }
}