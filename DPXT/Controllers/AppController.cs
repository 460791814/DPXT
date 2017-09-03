using DAL;
using DPXT.App_Start;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPXT.Controllers
{
    /// <summary>
    /// 巡检点评APP
    /// </summary>
    public class AppController : Controller
    {
        D_Theme dTheme = new D_Theme();
        D_Opinion dOpinion = new D_Opinion();
        D_CommentType dCommentType = new D_CommentType();
        D_CommentItem dCommentItem = new D_CommentItem();
        D_Comment dComment = new D_Comment();
        D_CommentCommentItem dCommentCommentItem = new D_CommentCommentItem();
        D_CommentOpinion dCommentOpinion = new D_CommentOpinion();
        
        /// <summary>
        /// 选择主题
        /// </summary>
        [AppAuth]
        [Route("app/selecttheme")]
        public ActionResult SelectTheme()
        {
            E_User eUser = HttpContext.Session["user"] as E_User;
            ViewBag.userinfo = eUser;
            ViewBag.listtheme = dTheme.GetList();
            return View("~/views/App/SelectTheme.cshtml");
        }

        /// <summary>
        /// 评论内容
        /// </summary>
        [AppAuth]
        [Route("app/commentcontent")]
        public ActionResult CommentContent(E_Comment eComment)
        {
            E_User eUser = HttpContext.Session["user"] as E_User;
            ViewBag.userinfo = eUser;
            ViewBag.listCommentType = dCommentType.GetListByThemeid(new E_CommentType() { themeid = eComment.themeid });
            ViewBag.listCommentItem = dCommentItem.GetCommentItemByThemeID(eComment.themeid);
            ViewBag.listopinion = dOpinion.GetList(new E_Opinion() { themeid = eComment.themeid });
            ViewBag.CommentInfo = eComment;
            return View("~/views/App/CommentContent.cshtml");
        }
        
        /// <summary>
        /// 保存评论内容
        /// </summary>
        [Route("app/savecommentcontent")]
        public JsonResult SaveCommentContent(E_Comment eComment)
        {
            string msg = "提交失败！";
            bool result = false;
            eComment.addtime = DateTime.Now;
            eComment.commentid = dComment.Add(eComment);
            if (eComment.commentid > 0)
            {
                foreach (var item in eComment.commentcommentitem)
                {
                    item.commentid = eComment.commentid;
                    dCommentCommentItem.Add(item);
                }
                foreach (var item in eComment.commentopinion)
                {
                    item.commentid = eComment.commentid;
                    dCommentOpinion.Add(item);
                }
                msg = "提交成功！";
                result = true;
            }
            return Json(new { result = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}