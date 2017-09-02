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

        /// <summary>
        /// 手机端登陆页
        /// </summary>
        [Route("app/login")]
        public ActionResult AppLogin()
        {
            return View("~/views/App/Login.cshtml");
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
                    Session["user"] = eUser;
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