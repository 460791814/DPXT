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
    /// <summary>
    /// WebAPI接口控制器
    /// </summary>
    public class WebApiController : Controller
    {
        D_ClassInfo dalclassinfo = new D_ClassInfo();
        
        /// <summary>
        /// 获取班组列表
        /// </summary>
        public JsonResult GetClassInfoList(int areaid)
        {
            E_ClassInfo model = new E_ClassInfo();
            model.areaid = areaid;
            return Json(JsonConvert.SerializeObject(dalclassinfo.GetList(model)));
        }
    }
}