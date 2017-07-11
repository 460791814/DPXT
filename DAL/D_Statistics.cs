using Model.Statistics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Comp;

namespace DAL
{
    /// <summary>
    /// 统计
    /// </summary>
    public class D_Statistics
    {
        /// <summary>
        /// 点评项综合统计
        /// </summary>
        /// <returns></returns>
        public List<E_ItemTotal> GetItemTotal(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ItemTotal> list = new List<E_ItemTotal>();

            StringBuilder strwhere =new StringBuilder();
            if (eStatisticsParameter.areaid > 0)
            {
                strwhere.AddWhere($"D.areaid={eStatisticsParameter.areaid}");
            }
            if (eStatisticsParameter.classinfoid > 0)
            {
                strwhere.AddWhere($"D.classinfoid={eStatisticsParameter.classinfoid}");
            }
            if (eStatisticsParameter.starttime != null)
            {
                strwhere.AddWhere($"D.addtime>cast('{Convert.ToDateTime(eStatisticsParameter.starttime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            if (eStatisticsParameter.endtime != null)
            {
                strwhere.AddWhere($"D.addtime<cast('{Convert.ToDateTime(eStatisticsParameter.endtime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select 
                            typename,
                            title,											--点评维度、点评项标题
                            sum(B.expect) as sumexpect,						--期望值总和
                            sum(B.real) as sumreal,							--感知值总和
                            (sum(B.real)-sum(B.expect)) as sumdifferent     --差值 
                            from dp_commentitem as A 
                            left join dp_commentcommentitem as B on A.commentitemid=B.commentitemid
                            left join dp_commenttype as C on A.commenttypeid=C.commenttypeid 
                            left join dp_comment as D on D.commentid=B.commentid {0}
                            group by A.commentitemid,A.title,C.typename", strwhere.ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_ItemTotal>(strSql.ToString())?.ToList();
            }
            return list;
        }

        /// <summary>
        /// 点评项综合平均统计
        /// </summary>
        /// <returns></returns>
        public List<E_ItemTotalAvg> GetItemTotalAvg(E_StatisticsParameter eStatisticsParameter)
        {
            List<E_ItemTotalAvg> list = new List<E_ItemTotalAvg>();

            StringBuilder strwhere = new StringBuilder();
            if (eStatisticsParameter.areaid > 0)
            {
                strwhere.AddWhere($"D.areaid={eStatisticsParameter.areaid}");
            }
            if (eStatisticsParameter.classinfoid > 0)
            {
                strwhere.AddWhere($"D.classinfoid={eStatisticsParameter.classinfoid}");
            }
            if (eStatisticsParameter.starttime != null)
            {
                strwhere.AddWhere($"D.addtime>cast('{Convert.ToDateTime(eStatisticsParameter.starttime).ToString("yyyy-MM-dd")}' as datetime)");
            }
            if (eStatisticsParameter.endtime != null)
            {
                strwhere.AddWhere($"D.addtime<cast('{Convert.ToDateTime(eStatisticsParameter.endtime).ToString("yyyy-MM-dd")}' as datetime)");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select 
                            C.typename,																									                        --点评维度
                            cast(avg(cast(B.expect as float)) as decimal(18,2)) as avgexpect,																	--期望均值(E)
                            cast(avg(cast(B.real as float)) as decimal(18,2))  as avgreal,																		--感知均值(P)
                            cast(avg(cast(B.real as float))-avg(cast(B.expect as float))as decimal(18,2)) as avgdifferent,									    --差值（P-E）均值
                            cast((avg(cast(B.real as float))-avg(cast(B.expect as float)))/avg(cast(B.expect as float))as decimal(18,2)) as expectdeviation,	--期望偏离度（（P-E）/E*100%）
                            cast((avg(cast(B.real as float))-avg(cast(B.expect as float)))/avg(cast(B.expect as float))+1 as decimal(18,2)) as satisfaction	    --顾客满意度（期望偏离度+100%）
                            from dp_commentitem as A 
                            left join dp_commentcommentitem as B on A.commentitemid=B.commentitemid
                            left join dp_commenttype as C on A.commenttypeid=C.commenttypeid 
                            left join dp_comment as D on D.commentid=B.commentid {0} 
                            group by C.commenttypeid,C.typename", strwhere.ToString());
            using (IDbConnection conn = new SqlConnection(DapperHelper.GetConStr()))
            {
                list = conn.Query<E_ItemTotalAvg>(strSql.ToString())?.ToList();
            }
            return list;
        }
    }
}
