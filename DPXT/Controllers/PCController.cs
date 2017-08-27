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
    public class PCController : Controller
    {
        D_Opinion dOpinion = new D_Opinion();
        D_ServiceComplain dServiceComplain = new D_ServiceComplain();
        D_User dUser = new D_User();
        D_Person dPerson = new D_Person();
        D_Food dFood = new D_Food();

        // GET: PC
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChengGuo()
        {
            return View();
        }
        /// <summary>
        /// 配餐服务意见
        /// </summary>
        /// <returns></returns>
        public ActionResult Opinion()
        {
            int total = 0;
            var opionList = dOpinion.GetList(new E_Opinion()
            {
                opiniontypeid = 2,
                 PageIndex=1,
                  PageSize=10000
            }, ref total);
            ViewBag.list = opionList;
            return View();
        }
        public String SaveOpinion(dp_servicecomplain model)
        {

            int servicecomplainid = dServiceComplain.Add(new dp_servicecomplain()
            {
                classinfoid = 1,
                areaid = 1,
                contents = model.contents,
                addtime = DateTime.Now
            });
            if (model.opinionlist != null)
            {
                foreach (var item in model.opinionlist)
                {
                    dServiceComplain.AddServiceComplainOpinion(new dp_servicecomplainopinion()
                    {
                        opinionid = item.opinionid,
                        servicecomplainid = servicecomplainid
                    });
                }
            }
            return JsonConvert.SerializeObject(new { result = "True" });
        }
        public String SavePerson(List<E_Person> list) {

            if (list != null) {
                foreach (var item in list)
                {
                    dPerson.SumLike(item);
                }
            }
            return JsonConvert.SerializeObject(new { result = "True" });
        }
        public ActionResult UserService()
        {
            int total = 0;
            var list = dPerson.GetList(new E_Person() {

                PageSize = 10000
            },ref total);
            ViewBag.list = list;
            return View();

        }
        public ActionResult FoodLike()
        {
            int total = 0;
            var list=  dFood.GetList(new E_Food() {
                PageSize=10000,
                 startaddtime= Convert.ToDateTime( DateTime.Now.ToString("D"))
            }, ref total);
            ViewBag.list = list;
            return View();
        }
        public bool UpdateFoodHit(E_Food model)
        {
          return  dFood.UpdateHits(model);
            
        }
    }
}