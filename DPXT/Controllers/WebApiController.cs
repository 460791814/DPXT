using DAL;
using HtmlTableToExecl;
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

        /// <summary>
        /// 依据tr对应json数据，导出相应Execl
        /// </summary>
        /// <param name="trjson">trjson数据</param>
        /// <param name="filename">文件地址</param>
        /// <returns>生成文件下载地址</returns>
        public string ExportTable(string filename, string trjson)
        {
            string name = "/upfile/"+filename + DateTime.Now.ToString("yyyyMMddmmss") + ".xls";
            bool Result=HtmlTableExport.RenderToExcel(trjson, base.Server.MapPath("~/" + name));
            return Result ? name : "NO:生成Execl失败！";
        }
    }
}