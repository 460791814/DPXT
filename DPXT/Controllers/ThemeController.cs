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
    public class ThemeController : Controller
    {
        D_Theme dal = new D_Theme();
        D_ThemeCommentItem dThemeCommentItem = new D_ThemeCommentItem();
        D_ThemeOpinion dThemeOpinion = new D_ThemeOpinion();
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        // GET: CommentType
        // [AuthAttribute]
        public ActionResult List()
        {
            int pageIndex = Utils.GetInt(Request["page"]);
          //  ViewBag.page = Utils.ShowPage(100, 15, pageIndex, 5);
            ViewBag.list = dal.GetList();
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(E_Theme model)
        {
            if (model != null && model.themeid > 0)
            {
                return dal.Update(model);
            }
            return dal.Add(model);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(E_Theme model)
        {

            return dal.DeleteById(model);


        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Info(int id)
        {

            E_Theme model = new E_Theme();
            model.themeid = id;
            ViewBag.Info = dal.GetInfoById(model);
            return View();
        }
        public string GetDpx(int themeid)
        {
            E_Dpx dpx = new E_Dpx();
            dpx.ThemeOpinion = dThemeOpinion.GetList(new dp_themeopinion() {  themeid=themeid});
            dpx.ThemeItem = dThemeCommentItem.GetList(new dp_themecommentitem() {  themeid=themeid});
            return JsonConvert.SerializeObject(dpx);
        }
        public ActionResult Dpx(int id)
        {
            ViewBag.themeid =id;
            D_CommentType dal = new D_CommentType();
            D_CommentItem dalitem = new D_CommentItem();
            D_Opinion opiniondal = new D_Opinion();
            ViewBag.commentitemlist = dalitem.GetList(new E_CommentItem());
            ViewBag.commenttypelist = dal.GetList();
            int count = 0;
            ViewBag.opinionlist = opiniondal.GetList(new E_Opinion() {  opiniontypeid=1},ref count);

           
            return View();
        }
        public String SaveItem(E_Dpx model)
        {
              
            if (model == null) {
                
                return JsonConvert.SerializeObject(new { result = "请选择至少一项" });
            }
            if (model.ThemeItem==null) {
              
                return JsonConvert.SerializeObject(new { result = "请选择点评项" });
            }
            if (model.ThemeOpinion == null)
            {
                
                return JsonConvert.SerializeObject(new { result = "请选择巡检点评项" });
            }
            dThemeCommentItem.DeleteById(model.ThemeItem[0]);
            foreach (var item in model.ThemeItem)
            {
                dThemeCommentItem.Add(item);

            }
            dThemeOpinion.DeleteById(model.ThemeOpinion[0]);
            foreach (var item in model.ThemeOpinion)
            {
                dThemeOpinion.Add(item);

            }

            return JsonConvert.SerializeObject(new { result = "True" });
        }
    }
}