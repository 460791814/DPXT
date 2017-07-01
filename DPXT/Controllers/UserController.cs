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
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            ViewBag.classinfolist = dalclassinfo.GetList();            //班组列表
            int count = daluser.GetCount();
            ViewBag.page = Utils.ShowPage(count, searchmodel.pagesize, pageIndex, 5);

            return View();
        }
        
        /// <summary>
        /// 保存
        /// </summary>
        public bool Save(user model)
        {
            if (model != null && model.userid > 0)//若存在数据，执行更新
            {
                return daluser.Update(model);
            }
            return daluser.Add(model);//若不存在数据，直接插入
        }

        /// <summary>
        /// 删除
        /// </summary>
        public bool Delete(user model)
        {
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
            return View();
        }
    }
}
