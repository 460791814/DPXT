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
    public class UserController : Controller
    {
        D_User daluser = new D_User();
        D_Area dalarea = new D_Area();
        D_ClassInfo dalclassinfo = new D_ClassInfo();

        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult List()
        {
            int pageIndex = Utils.GetInt(Request["page"]);
            
            user searchmodel = new user();
            searchmodel.pagesize = 15;
            searchmodel.pageindex = pageIndex;
            ViewBag.userlist = daluser.GetList(searchmodel);           //用户列表
            
            int count = daluser.GetCount();
            ViewBag.page = Utils.ShowPage(count, searchmodel.pagesize, pageIndex, 5);

            return View();
        }

        /// <summary>
        /// 获取班组列表
        /// </summary>
        public JsonResult GetClassInfoList(int areaid)
        {
            classinfo model = new classinfo();
            model.areaid = areaid;
            return Json(JsonConvert.SerializeObject(dalclassinfo.GetList(model)));
        }

        /// <summary>
        /// 保存
        /// </summary>
        public bool Save(user model)
        {
            model.updatetime = DateTime.Now;
            if (model.userid > 0)//若存在数据，执行更新
            {
                return daluser.Update(model);
            }
            return daluser.Add(model);//若不存在数据，直接插入
        }

        /// <summary>
        /// 删除
        /// </summary>
        public bool Delete(int id)
        {
            user model = new user();
            model.userid = id;
            return daluser.DeleteById(model);
        }

        /// <summary>
        /// 详情
        /// </summary>
        public ActionResult Info(int id)
        {
            user model = new user();
            model.userid = id;
            ViewBag.Info = daluser.GetInfoById(model);
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View();
        }
    }
}
