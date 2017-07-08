using DAL;
using Model.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPXT.Controllers
{
    /// <summary>
    /// 数据统计控制器
    /// </summary>
    public class StatisticsController : Controller
    {
        // GET: Statistics
        [Route("/statistics/totalavg")]
        public ActionResult TotalAvg(int? areaid,int? classinfoid,DateTime? starttime,DateTime? endtime)
        {
            D_Statistics dStatistics = new D_Statistics();
            List<E_ItemTotal> itemtotallist = dStatistics.GetItemTotal();
            List<E_ItemTotalAvg> itemtotalavglist = dStatistics.GetItemTotalAvg();
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.itemtotalavglist = itemtotalavglist;
            return View("/views/statistics/TotalAvg.cshtml");
        }
    }
}