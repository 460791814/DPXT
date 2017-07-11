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
        D_Area dalarea = new D_Area();

        // GET: Statistics
        [Route("/statistics/totalavg")]
        public ActionResult TotalAvg(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ItemTotal> itemtotallist = new List<E_ItemTotal>();
            List<E_ItemTotalAvg> itemtotalavglist = new List<E_ItemTotalAvg>();
            if (eStatisticsParameter.issearch > 0)
            {
                D_Statistics dStatistics = new D_Statistics();
                itemtotallist = dStatistics.GetItemTotal(eStatisticsParameter);
                itemtotalavglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
            }
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.itemtotalavglist = itemtotalavglist;
            ViewBag.eStatisticsParameter = eStatisticsParameter;
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View("/views/statistics/TotalAvg.cshtml");
        }
        
    }
}