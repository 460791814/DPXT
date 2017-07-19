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
    //[RoutePrefix("statistics")]
    public class StatisticsController : Controller
    {
        D_Area dalarea = new D_Area();

        /// <summary>
        /// 综合平均统计
        /// </summary>
        /// <param name="eStatisticsParameter">统计查询参数</param>
        /// <returns></returns>
        [Route("statistics/totalavg")]
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

        /// <summary>
        /// 性别口径统计
        /// </summary>
        /// <param name="eStatisticsParameter">统计查询参数</param>
        /// <returns></returns>
        [Route("statistics/sex")]
        public ActionResult SexReport(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ColligateReport> itemtotallist = new List<E_ColligateReport>();
            List<E_ItemTotalAvg> sex_1_avglist = new List<E_ItemTotalAvg>();
            List<E_ItemTotalAvg> sex_2_avglist = new List<E_ItemTotalAvg>();

            if (eStatisticsParameter.issearch > 0)
            {
                D_Statistics dStatistics = new D_Statistics();
                itemtotallist = dStatistics.GetColligateReport(eStatisticsParameter);
                eStatisticsParameter.sex = 1;
                sex_1_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
                eStatisticsParameter.sex = 2;
                sex_2_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
            }
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.sex_1_avglist = sex_1_avglist;
            ViewBag.sex_2_avglist = sex_2_avglist;
            ViewBag.eStatisticsParameter = eStatisticsParameter;
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View("/views/statistics/sex.cshtml");
        }

        /// <summary>
        /// 年龄口径统计
        /// </summary>
        /// <param name="eStatisticsParameter">统计查询参数</param>
        /// <returns></returns>
        [Route("statistics/ageround")]
        public ActionResult AgeroundReport(E_StatisticsParameter eStatisticsParameter)
        { 
            List<E_ColligateReport> itemtotallist = new List<E_ColligateReport>();
            List<E_ItemTotalAvg> ageround_1_avglist = new List<E_ItemTotalAvg>();
            List<E_ItemTotalAvg> ageround_2_avglist = new List<E_ItemTotalAvg>();
            List<E_ItemTotalAvg> ageround_3_avglist = new List<E_ItemTotalAvg>();
            if (eStatisticsParameter.issearch > 0)
            {
                D_Statistics dStatistics = new D_Statistics();
                itemtotallist = dStatistics.GetColligateReport(eStatisticsParameter);
                eStatisticsParameter.ageround = 1;
                ageround_1_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
                eStatisticsParameter.ageround = 2;
                ageround_2_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
                eStatisticsParameter.ageround = 3;
                ageround_3_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
            }
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.ageround_1_avglist = ageround_1_avglist;
            ViewBag.ageround_2_avglist = ageround_2_avglist;
            ViewBag.ageround_3_avglist = ageround_3_avglist;
            ViewBag.eStatisticsParameter = eStatisticsParameter;
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View("/views/statistics/ageround.cshtml");
        }

        /// <summary>
        /// 饮食习惯口径统计
        /// </summary>
        /// <param name="eStatisticsParameter">统计查询参数</param>
        /// <returns></returns>
        [Route("statistics/domain")]
        public ActionResult DomainReport(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ColligateReport> itemtotallist = new List<E_ColligateReport>();
            List<E_ItemTotalAvg> domain_1_avglist = new List<E_ItemTotalAvg>();
            List<E_ItemTotalAvg> domain_2_avglist = new List<E_ItemTotalAvg>();
            if (eStatisticsParameter.issearch > 0)
            {
                D_Statistics dStatistics = new D_Statistics();
                itemtotallist = dStatistics.GetColligateReport(eStatisticsParameter);
                eStatisticsParameter.domain = 1;
                domain_1_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
                eStatisticsParameter.domain = 2;
                domain_2_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
            }
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.domain_1_avglist = domain_1_avglist;
            ViewBag.domain_2_avglist = domain_2_avglist;
            ViewBag.eStatisticsParameter = eStatisticsParameter;
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View("/views/statistics/domain.cshtml");
        }

        /// <summary>
        /// 饮食习惯口径统计
        /// </summary>
        /// <param name="eStatisticsParameter">统计查询参数</param>
        /// <returns></returns>
        [Route("statistics/job")]
        public ActionResult JobReport(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ColligateReport> itemtotallist = new List<E_ColligateReport>();
            List<E_ItemTotalAvg> job_1_avglist = new List<E_ItemTotalAvg>();
            List<E_ItemTotalAvg> job_2_avglist = new List<E_ItemTotalAvg>();
            if (eStatisticsParameter.issearch > 0)
            {
                D_Statistics dStatistics = new D_Statistics();
                itemtotallist = dStatistics.GetColligateReport(eStatisticsParameter);
                eStatisticsParameter.job = 1;
                job_1_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
                eStatisticsParameter.job = 2;
                job_2_avglist = dStatistics.GetItemTotalAvg(eStatisticsParameter);
            }
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.job_1_avglist = job_1_avglist;
            ViewBag.job_2_avglist = job_2_avglist;
            ViewBag.eStatisticsParameter = eStatisticsParameter;
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View("/views/statistics/job.cshtml");
        }

        /// <summary>
        /// 饮食习惯口径统计
        /// </summary>
        /// <param name="eStatisticsParameter">统计查询参数</param>
        /// <returns></returns>
        [Route("statistics/person")]
        public ActionResult PersonReport(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_PersonReport> itemtotallist = new List<E_PersonReport>();
            if (eStatisticsParameter.issearch > 0)
            {
                D_Statistics dStatistics = new D_Statistics();
                itemtotallist = dStatistics.GetPersonReport(eStatisticsParameter);
            }
            ViewBag.itemtotallist = itemtotallist;
            ViewBag.eStatisticsParameter = eStatisticsParameter;
            ViewBag.arealist = dalarea.GetList();                      //作业区列表
            return View("/views/statistics/person.cshtml");
        }
    }
}